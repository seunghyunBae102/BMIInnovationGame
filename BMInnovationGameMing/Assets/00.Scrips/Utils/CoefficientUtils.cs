using System;
using System.Collections.Generic;

// ��ƿ: baseCoeffs (�̹� NN ���� ����) + user overrides ����
//public static class CoefficientUtils
//{
//    // ���� ��Ģ: �⺻���� ���� �������̵带 additive ������� ����.
//    // �ʿ��ϸ� mapping ��带 UserRuntimeState�� �߰��Ͽ� �ٸ��� ó���ϵ��� Ȯ���ϼ���.
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