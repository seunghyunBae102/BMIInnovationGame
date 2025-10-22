using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// GetCompoParent�� �ڽ� ������Ʈ�� ����
public class SimRunner : GetCompoableBase
{
    public NeuralNetSO neuralNet;
    public SO_CoefficientTable coeffTable;
    public string[] nnOutputKeys;

    public List<SO_Media> allMedia;
    public List<SO_UserBase> allUsers;
    public SimulationLogger logger;

    [Tooltip("���ø� �� ������ ���� �ν��Ͻ� ��")]
    public int instancesPerTemplate = 1;

    private List<UserEntity> userEntities = new List<UserEntity>();
    private BMWeightParamLoader weightLoader; // ���� �θ� �Ʒ��� �ٸ� ������Ʈ

    public override void Init(GetCompoParent mom)
    {
        base.Init(mom);
        weightLoader = Mom.GetCompo<BMWeightParamLoader>();
    }

    private void Start()
    {
        userEntities.Clear();
        if (allUsers != null)
        {
            foreach (var uTemplate in allUsers)
            {
                if (uTemplate == null) continue;
                for (int i = 0; i < Math.Max(1, instancesPerTemplate); i++)
                {
                    var id = (instancesPerTemplate > 1) ? $"{uTemplate.name}_{i}" : uTemplate.name;
                    userEntities.Add(new UserEntity(uTemplate, id));
                }
            }
        }

        // weightLoader���� �ε�� ������ Ű�� ������� featureWeights �ʱ�ȭ
        foreach (var ue in userEntities)
        {
            var rs = ue.runtimeState;
            if (rs == null) continue;
            if (rs.featureWeights == null) rs.featureWeights = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
            
            // weightLoader���� �ε�� ������ Ű�� ���
            if (weightLoader != null && weightLoader.soDict != null)
            {
                foreach (var nodeKey in weightLoader.soDict.Keys)
                {
                    if (!rs.featureWeights.ContainsKey(nodeKey))
                        rs.featureWeights[nodeKey] = 1f;
                }
            }
            
            // coeffTable keys�� �߰��� �ʱ�ȭ
            if (coeffTable != null && coeffTable.keys != null)
            {
                foreach (var k in coeffTable.keys)
                {
                    if (string.IsNullOrWhiteSpace(k)) continue;
                    if (!rs.featureWeights.ContainsKey(k))
                        rs.featureWeights[k] = 1f;
                }
            }
        }

        if (neuralNet != null) neuralNet.Prepare();
    }

    public void RunSlot(float slotTimePressure, float slotMinutes)
    {
        if (userEntities == null || allMedia == null || weightLoader == null) return;

        foreach (var ue in userEntities)
        {
            var uState = ue.runtimeState;
            var candidates = ue.template.GetCandidateMedia(allMedia, slotTimePressure);
            if (candidates == null || candidates.Count == 0) continue;

            var utilities = new List<float>(candidates.Count);

            foreach (var m in candidates)
            {
                // �⺻ ����� �� ��ųʸ��� ����
                var coeffs = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

                // 1. weightLoader�� ��� ��Ʈ��ũ ��� ����� coeffs�� �ݿ�
                foreach (var node in weightLoader.soList)
                {
                    if (node == null || string.IsNullOrEmpty(node.Key)) continue;
                    
                    float nodeValue = EvaluateNode(node, uState, m, slotTimePressure);
                    coeffs[node.Key] = nodeValue;
                }

                // 2. coeffTable�� �ִٸ� ����
                if (coeffTable != null)
                {
                    var baseCoeffs = coeffTable.GetBaseDictionary();
                    foreach (var kv in baseCoeffs)
                    {
                        if (!coeffs.ContainsKey(kv.Key))
                            coeffs[kv.Key] = kv.Value;
                        else
                            coeffs[kv.Key] *= kv.Value; // �Ǵ� �ٸ� ���� ��� ���
                    }
                }

                // 3. NN ����
                if (neuralNet != null && neuralNet.layerSizes != null && neuralNet.layerSizes.Length > 0)
                {
                    var ctx = m.BuildContextVector(uState.mood, uState.fatigue, slotTimePressure, uState.desire);
                    int outLen = neuralNet.layerSizes[neuralNet.layerSizes.Length - 1];
                    var outBuf = new float[outLen];
                    if (neuralNet.TryEvaluate(ctx, 0, outBuf, 0))
                    {
                        if (coeffTable != null)
                        {
                            var nnCoeffs = coeffTable.ApplyNNOutputs(outBuf, nnOutputKeys);
                            foreach (var kv in nnCoeffs)
                            {
                                if (!coeffs.ContainsKey(kv.Key))
                                    coeffs[kv.Key] = kv.Value;
                                else
                                    coeffs[kv.Key] += kv.Value;
                            }
                        }
                    }
                }

                float utility = ComputeUtility(uState, m, coeffs);
                utilities.Add(utility);
            }

            int chosen = SampleIndexBySoftmax(utilities.ToArray(), Mathf.Clamp(1f / (1f + slotTimePressure), 0.01f, 10f));
            var chosenMedia = candidates[Mathf.Clamp(chosen, 0, candidates.Count - 1)];
            float sessionMinutes = Mathf.Min(slotMinutes, chosenMedia.avgSessionMinutes);
            float revenue = chosenMedia.EstimateSessionRevenue(sessionMinutes);
            var effects = chosenMedia.ComputeSessionEffects(sessionMinutes);
            
            uState.ApplySessionEffects(effects.Item1, sessionMinutes * 0.01f, 
                Math.Max(0f, revenue - chosenMedia.adRevenuePerMinute * sessionMinutes * chosenMedia.adFillRate));
            
            logger?.LogEvent(uState.userId, chosenMedia.mediaId ?? chosenMedia.name, 
                sessionMinutes, revenue, effects.Item1, effects.Item2);
        }
    }

    private float EvaluateNode(SO_NodeParent node, UserRuntimeState u, SO_Media m, float timePressure)
    {
        if (node == null) return 0f;

        float inputSum = 0f;
        if (node.input != null)
        {
            foreach (var combo in node.input)
            {
                if (combo.node == null) continue;
                float inputValue = EvaluateNode(combo.node, u, m, timePressure);
                inputSum += inputValue * combo.weight.value;
            }
        }

        float biasValue = node.bias.value;
        
        // ����� ��� �޼��尡 ������ ���, ������ �ܼ� ��
        return node.caculMethod != null 
            ? node.caculMethod.GetValue(node.input, biasValue + inputSum, null)
            : biasValue + inputSum;
    }

    private float ComputeUtility(UserRuntimeState u, SO_Media m, Dictionary<string, float> coeffs)
    {
        if (u == null || m == null) return 0f;

        float sum = 0f;
        foreach (var kv in coeffs)
        {
            // ������ ����ġ�� ���Ͽ� ���� ��ƿ��Ƽ ���
            float userWeight = u.GetFeatureWeight(kv.Key);
            sum += kv.Value * userWeight;
        }

        return Sigmoid(sum);
    }

    private float Sigmoid(float x) => 1f / (1f + Mathf.Exp(-x));

    private int SampleIndexBySoftmax(float[] utilities, float temperature)
    {
        if (utilities == null || utilities.Length == 0) return -1;
        
        int n = utilities.Length;
        float max = utilities.Max();
        var exp = new float[n];
        float sum = 0f;
        
        for (int i = 0; i < n; i++)
        {
            float v = (utilities[i] - max) / Math.Max(1e-6f, temperature);
            exp[i] = Mathf.Exp(v);
            sum += exp[i];
        }

        float r = UnityEngine.Random.value * sum;
        float acc = 0f;
        
        for (int i = 0; i < n; i++)
        {
            acc += exp[i];
            if (r <= acc) return i;
        }
        
        return n - 1;
    }
}