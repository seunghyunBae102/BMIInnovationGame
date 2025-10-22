using System;
using System.Collections.Generic;

// 유틸: baseCoeffs (이미 NN 보정 포함) + user overrides 병합
//public static class CoefficientUtils
//{
//    // 병합 규칙: 기본값에 유저 오버라이드를 additive 방식으로 더함.
//    // 필요하면 mapping 모드를 UserRuntimeState에 추가하여 다르게 처리하도록 확장하세요.
//    public static Dictionary<string, float> MergeUserOverrides(UserRuntimeState user, Dictionary<string, float> baseCoeffs)
//    {
//        if (baseCoeffs == null) baseCoeffs = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
//        var result = new Dictionary<string, float>(baseCoeffs, StringComparer.OrdinalIgnoreCase);

//        if (user == null || user.coefficientOverrides == null || user.coefficientOverrides.Count == 0)
//            return result;

//        foreach (var kv in user.coefficientOverrides)
//        {
//            if (string.IsNullOrWhiteSpace(kv.Key)) continue;
//            if (result.ContainsKey(kv.Key)) result[kv.Key] = result[kv.Key] + kv.Value;
//            else result[kv.Key] = kv.Value;
//        }

//        return result;
//    }
//}