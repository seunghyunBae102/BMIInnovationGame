using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Media", menuName = "SO/BM/SO_Media")]
public class SO_Media : ScriptableObject
{
    public enum ContentMode { Video, LiveStream, Article, Game, Social, Event }

    [Header("Identity")]
    public string mediaId;
    public string displayName;
    [TextArea(2, 6)]
    public string description;

    [Header("Content metrics (0..1) - 데이터로 조정")]
    [Range(0f, 1f)] public float appeal = 0.5f;       // 사용자가 매력적으로 느끼는 정도
    [Range(0f, 1f)] public float novelty = 0.5f;      // 신선도(세션당 감소)
    [Range(0f, 1f)] public float relevance = 0.5f;    // 문맥/유사성
    [Range(0f, 1f)] public float brandStrength = 0.5f;// 브랜드 친숙도
    [Range(0f, 1f)] public float habitability = 0.1f; // 반복 사용 시 습관이 되기 쉬운 정도
    public ContentMode mode = ContentMode.Video;
    public bool offlineOk = false; // 오프라인에서 소비 가능 여부

    [Header("Slot / Time")]
    public float avgSessionMinutes = 20f;
    [Tooltip("슬롯에서 차지하는 시간 가중치(유닛)")]
    public float slotCapacityWeight = 1f;
    public bool requiresConnection = true;

    [Header("Monetization (데이터로 관리)")]
    [Tooltip("분당 광고로 얻는 평균 수익(USD 등)")]
    public float adRevenuePerMinute = 0.05f;
    [Range(0f,1f)] public float adFillRate = 0.8f;
    [Tooltip("분당 구매 전환 확률(기댓값)")]
    public float purchaseConversionPerMinute = 0.0005f;
    [Tooltip("평균 구매 금액")]
    public float avgPurchaseValue = 2.0f;
    [Tooltip("구독형 월 평균 ARPU")]
    public float subscriptionMonthlyARPU = 0.0f;
    [Tooltip("미디어 운영비(월별) ? 생존 규칙에 사용")]
    public float monthlyCost = 0.0f;

    [Header("Dynamics / 변화")]
    [Tooltip("하루당 신선도 감소")]
    public float noveltyDecayPerDay = 0.01f;
    [Tooltip("하루당 브랜드 약화")]
    public float brandDecayPerDay = 0.001f;
    [Tooltip("세션당 습관 증가량(유저 측 상태에 더함)")]
    public float habitIncreasePerSession = 0.01f;

    [Header("Metadata")]
    public string version = "1.0";

    // 런타임에서 NN/SimRunner가 호출해 해석 가능한 컨텍스트 벡터를 생성하도록 제공.
    // 벡터 항목은 문서화되어야 하며, NeuralNetSO의 입력 매핑과 일치시킬 것.
    // 기본 반환: [userMood, userFatigue, novelty, relevance, slotTimePressure, brandStrength]
    public float[] BuildContextVector(float userMood, float userFatigue, float slotTimePressure, float userDesire)
    {
        return new float[]
        {
            Clamp01(userMood),
            Clamp01(userFatigue),
            Clamp01(novelty),
            Clamp01(relevance),
            slotTimePressure,       // 시간 압박은 그대로 (0..∞ 가능)
            Clamp01(brandStrength),
            Clamp01(userDesire)
        };
    }

    // 주어진 세션 길이(분)로 기대 수익을 추정한다.
    // NN/SimRunner는 이 값을 사용해 선택 후 실제 수익 시뮬레이션을 수행 가능.
    public float EstimateSessionRevenue(float sessionMinutes)
    {
        if (sessionMinutes <= 0f) return 0f;
        // 광고 수익(분당) * 분 * 광고노출률
        float adRev = adRevenuePerMinute * sessionMinutes * Clamp01(adFillRate);

        // 기대 구매 수 = 분당 전환율 * 분 -> 기대 구매수 * 평균구매값
        float expectedPurchases = purchaseConversionPerMinute * sessionMinutes;
        float purchaseRev = expectedPurchases * Math.Max(0f, avgPurchaseValue);

        // 단순 합산. 복잡한 모델은 외부에서 SO의 파라미터를 이용해 확장.
        return adRev + purchaseRev;
    }

    // 세션 이후 미디어·사용자 상태 변화 계산(순수 데이터 계산만 수행).
    // 반환값: (deltaUserHabit, deltaBrandStrength)
    public (float deltaHabit, float deltaBrand) ComputeSessionEffects(float sessionMinutes)
    {
        // 습관 증가는 세션 길이와 habitability에 비례.
        float deltaHabit = habitIncreasePerSession * Mathf.Sqrt(Mathf.Max(0f, sessionMinutes)) * habitability;
        // 브랜드는 세션으로 소폭 강화 (사용자 경험에 따라)
        float deltaBrand = 0.001f * Mathf.Log10(1f + sessionMinutes) * (appeal + relevance) * 0.5f;
        return (deltaHabit, deltaBrand);
    }

    // 일별 변화: 신선도·브랜드 자연감소
    public void ApplyDailyDecay(ref float inoutNovelty, ref float inoutBrand)
    {
        inoutNovelty = Mathf.Clamp01(inoutNovelty - noveltyDecayPerDay);
        inoutBrand = Mathf.Clamp01(inoutBrand - brandDecayPerDay);
    }

    public bool Validate(out string problem)
    {
        if (string.IsNullOrEmpty(mediaId)) { problem = "mediaId가 비어있습니다."; return false; }
        if (avgSessionMinutes < 0f) { problem = "avgSessionMinutes가 음수입니다."; return false; }
        if (adRevenuePerMinute < 0f) { problem = "adRevenuePerMinute가 음수입니다."; return false; }
        problem = null; return true;
    }

    private static float Clamp01(float v) => Mathf.Clamp01(v);
}
