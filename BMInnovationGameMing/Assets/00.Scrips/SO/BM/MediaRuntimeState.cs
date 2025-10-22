using System;
using UnityEngine;

[Serializable]
public class MediaRuntimeState
{
    public string mediaId;
    public float novelty = 1f;       // ��Ÿ�� �ż���(0..1)
    public float brandStrength = 0.5f; // ��Ÿ�� �귣��(0..1)
    public float cumulativeRevenue = 0f; // �ùķ��̼� �Ⱓ ���� ����

    public MediaRuntimeState() { }

    public MediaRuntimeState(string id, float noveltyInit, float brandInit)
    {
        mediaId = id;
        novelty = Mathf.Clamp01(noveltyInit);
        brandStrength = Mathf.Clamp01(brandInit);
    }

    // �Ϻ� ����/���� ����: SimRunner���� SO�� ���ǵ� ������ ȣ��
    public void ApplyDailyDecay(float noveltyDecayPerDay, float brandDecayPerDay, float days = 1f)
    {
        novelty = Mathf.Clamp01(novelty - Mathf.Abs(noveltyDecayPerDay) * days);
        brandStrength = Mathf.Clamp01(brandStrength - Mathf.Abs(brandDecayPerDay) * days);
    }

    public void AddRevenue(float revenue)
    {
        cumulativeRevenue += Mathf.Max(0f, revenue);
    }
}