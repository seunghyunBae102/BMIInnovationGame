using System;
using UnityEngine;

[Serializable]
public class MediaRuntimeState
{
    public string mediaId;
    public float novelty = 1f;       // 런타임 신선도(0..1)
    public float brandStrength = 0.5f; // 런타임 브랜드(0..1)
    public float cumulativeRevenue = 0f; // 시뮬레이션 기간 누적 수익

    public MediaRuntimeState() { }

    public MediaRuntimeState(string id, float noveltyInit, float brandInit)
    {
        mediaId = id;
        novelty = Mathf.Clamp01(noveltyInit);
        brandStrength = Mathf.Clamp01(brandInit);
    }

    // 일별 감쇠/증가 적용: SimRunner에서 SO에 정의된 값으로 호출
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