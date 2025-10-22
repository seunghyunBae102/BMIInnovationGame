using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BM/SO_CoefficientTable")]
public class SO_CoefficientTable : ScriptableObject
{
    [Tooltip("��� Ű(��: appeal, brand, desire, habit, fatigue, context, novelty, relevance ��)")]
    public string[] keys = new string[0];

    [Tooltip("�� Ű�� ���� �⺻��(���� ����)")]
    public float[] baseValues = new float[0];


    public enum MappingMode { Additive, Multiplicative, GlobalScale }
    [Tooltip("NN ��°� �����ϴ� ���")]
    public MappingMode mapping = MappingMode.Additive;

    // �⺻ ��ųʸ� ��ȯ(�б� ���� ����)
    public Dictionary<string, float> GetBaseDictionary()
    {
        var d = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        int n = Math.Min(keys.Length, baseValues.Length);
        for (int i = 0; i < n; i++)
        {
            if (string.IsNullOrWhiteSpace(keys[i])) continue;
            if (!d.ContainsKey(keys[i])) d[keys[i]] = baseValues[i];
        }
        return d;
    }

    // NN ���(nnOutputs)�� ���-Ű ����(nnOutputKeys)�� �޾� ���� ��� ��ųʸ��� ��ȯ
    // - nnOutputKeys: nnOutputs �ε����� 1:1 �����Ǵ� Ű�� (���� ���ų� ���� �� ����)
    // - mapping: Additive => base + out, Multiplicative => base * (1 + out), GlobalScale => scale = out[0]
    public Dictionary<string, float> ApplyNNOutputs(float[] nnOutputs, string[] nnOutputKeys)
    {
        var result = GetBaseDictionary();

        if (nnOutputs == null || nnOutputs.Length == 0) return result;

        switch (mapping)
        {
            case MappingMode.Additive:
                ApplyAdditive(result, nnOutputs, nnOutputKeys);
                break;
            case MappingMode.Multiplicative:
                ApplyMultiplicative(result, nnOutputs, nnOutputKeys);
                break;
            case MappingMode.GlobalScale:
                ApplyGlobalScale(result, nnOutputs);
                break;
        }

        return result;
    }

    private void ApplyAdditive(Dictionary<string, float> d, float[] outs, string[] outKeys)
    {
        int m = Math.Min(outs.Length, (outKeys != null ? outKeys.Length : 0));
        for (int i = 0; i < m; i++)
        {
            var key = outKeys[i];
            if (string.IsNullOrWhiteSpace(key)) continue;
            if (!d.ContainsKey(key)) d[key] = 0f;
            d[key] += outs[i];
        }
        // if no keys provided, use first output as a global add to all
        if (m == 0 && outs.Length > 0)
        {
            float v = outs[0];
            var keys = new List<string>(d.Keys);
            foreach (var k in keys) d[k] += v;
        }
    }

    private void ApplyMultiplicative(Dictionary<string, float> d, float[] outs, string[] outKeys)
    {
        int m = Math.Min(outs.Length, (outKeys != null ? outKeys.Length : 0));
        for (int i = 0; i < m; i++)
        {
            var key = outKeys[i];
            if (string.IsNullOrWhiteSpace(key)) continue;
            if (!d.ContainsKey(key)) d[key] = 0f;
            d[key] *= (1f + outs[i]);
        }
        if (m == 0 && outs.Length > 0)
        {
            float v = 1f + outs[0];
            var keys = new List<string>(d.Keys);
            foreach (var k in keys) d[k] *= v;
        }
    }

    private void ApplyGlobalScale(Dictionary<string, float> d, float[] outs)
    {
        if (outs.Length == 0) return;
        float s = 1f + outs[0];
        var keys = new List<string>(d.Keys);
        foreach (var k in keys) d[k] *= s;
    }

    public bool Validate(out string problem)
    {
        if (keys == null || baseValues == null) { problem = "keys �Ǵ� baseValues�� null�Դϴ�."; return false; }
        if (keys.Length != baseValues.Length) { problem = "keys�� baseValues�� ���̰� ���� �ʽ��ϴ�."; return false; }
        problem = null; return true;
    }
}