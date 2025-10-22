using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserRuntimeState
{
    public string userId;
    public float mood = 0.5f;
    public float fatigue = 0f;
    public float desire = 0.5f;
    public float habit = 0f;
    public float balance = 10f;
    public string lastSelectedMediaId = null;
    public float timeAvailableMinutes = 0f;

    // ������ Ű -> ����ġ(��ȣ). SO�� Ű�� �����ؼ� �� ����.
    // ��: { "appeal": 1.2f } �� ������ appeal�� 1.2��� �������� ����.
    public Dictionary<string, float> featureWeights = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

    // ����� �ұԸ� �������̵�(���� ��뼺 ����)
    public Dictionary<string, float> coefficientOverrides = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

    public float GetFeatureWeight(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return 1f;
        if (featureWeights != null && featureWeights.TryGetValue(key, out var v)) return v;
        return 1f;
    }

    public void SetFeatureWeight(string key, float value)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (featureWeights == null) featureWeights = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        featureWeights[key] = value;
    }

    public void AddFeatureWeight(string key, float value = 0.1f)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (featureWeights == null) featureWeights = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        featureWeights[key] += value;
    }
    public void AffectFeatureWeight(string key, float value = 0.5f,float power =0.1f)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (featureWeights == null) featureWeights = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        featureWeights[key] = Mathf.Lerp(featureWeights[key], value, power);
    }

    // ���� ȿ�� �ݿ�(����)
    public void ApplySessionEffects(float deltaHabit, float deltaFatigue, float spend)
    {
        habit = Mathf.Clamp01(habit + deltaHabit);
        fatigue = Mathf.Clamp01(fatigue + deltaFatigue);
        balance = Mathf.Max(0f, balance - Math.Abs(spend));
    }

    public void RecoverEndOfDay(float recoveryAmount)
    {
        fatigue = Mathf.Clamp01(fatigue - Mathf.Abs(recoveryAmount));
        mood = Mathf.Clamp01(mood + Mathf.Abs(recoveryAmount) * 0.1f);
    }
}