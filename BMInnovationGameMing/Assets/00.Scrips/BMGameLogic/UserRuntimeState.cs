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

    // 유저별 키 -> 가중치(선호). SO의 키에 대응해서 값 보유.
    // 예: { "appeal": 1.2f } 는 유저가 appeal을 1.2배로 반응함을 뜻함.
    public Dictionary<string, float> featureWeights = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

    // 실험용 소규모 오버라이드(기존 사용성 보존)
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

    // 세션 효과 반영(편의)
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