using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BM/SO_CoefficientTable")]
public class SO_CoefficientTable : ScriptableObject
{
    [Tooltip("계수 키(예: appeal, brand, desire, habit, fatigue, context, novelty, relevance 등)")]
    public string[] keys = new string[0];

    [Tooltip("각 키에 대한 기본값(같은 길이)")]
    public float[] baseValues = new float[0];


    public enum MappingMode { Additive, Multiplicative, GlobalScale }
    [Tooltip("NN 출력과 결합하는 방식")]
    public MappingMode mapping = MappingMode.Additive;

    // 기본 딕셔너리 반환(읽기 전용 복사)
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

    // NN 출력(nnOutputs)과 출력-키 매핑(nnOutputKeys)을 받아 최종 계수 딕셔너리를 반환
    // - nnOutputKeys: nnOutputs 인덱스와 1:1 대응되는 키들 (길이 같거나 작을 수 있음)
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
        if (keys == null || baseValues == null) { problem = "keys 또는 baseValues가 null입니다."; return false; }
        if (keys.Length != baseValues.Length) { problem = "keys와 baseValues의 길이가 맞지 않습니다."; return false; }
        problem = null; return true;
    }
}