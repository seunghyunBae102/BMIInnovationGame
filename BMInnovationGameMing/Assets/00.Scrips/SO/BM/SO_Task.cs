using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Task", menuName = "SO/BM/SO_Task")]
public class SO_Task : ScriptableObject
{
    [Header("Identity")]
    public string taskId;
    public string displayName;

    [Header("Link to media (task delegates monetization/effects)")]
    public SO_Media media; // 작업이 실제로 소비하는 미디어 자원

    [Header("Task parameters")]
    [Tooltip("슬롯에서 차지하는 가중치(용량)")]
    public float capacityWeight = 1f;
    [Tooltip("오프라인에서 허용 여부. media.offlineOk와 AND 처리 가능")]
    public bool offlineOk = false;
    [Tooltip("세션 기본 길이(분). 0이면 media.avgSessionMinutes 사용")]
    public float sessionMinutes = 0f;
    [Tooltip("세션 길이 비율(미디어 기본값 * multiplier)")]
    public float sessionMinutesMultiplier = 1f;

    [Header("Metadata")]
    public string description;

    // 런타임 컨텍스트 벡터(NeuralNet 입력) 생성: SO_Media.BuildContextVector를 재사용하되 task 수준 조정 허용
    // 반환 형식은 SimRunner에서 기대하는 컨텍스트 길이와 일치해야 합니다.
    public float[] BuildContextVector(UserRuntimeState userState, MediaRuntimeState mediaState, float slotTimePressure)
    {
        if (media == null)
            return new float[] { userState.mood, userState.fatigue, 0f, 0f, slotTimePressure, 0f, userState.desire };

        // media의 기본 빌더를 사용해 생성한 뒤 런타임 상태로 덮어쓴다.
        var ctx = media.BuildContextVector(userState.mood, userState.fatigue, slotTimePressure, userState.desire);
        // Ensure length safety (caller expects fixed length, typically 7)
        if (ctx == null || ctx.Length < 7)
        {
            var safe = new float[7];
            for (int i = 0; i < Math.Min(ctx?.Length ?? 0, 7); i++) safe[i] = ctx[i];
            ctx = safe;
        }

        // override novelty/brand with runtime values if provided
        ctx[2] = mediaState != null ? mediaState.novelty : media.novelty;
        ctx[5] = mediaState != null ? mediaState.brandStrength : media.brandStrength;

        return ctx;
    }

    // 이 Task로 예상되는 세션 길이(분)
    public float EstimateSessionMinutes()
    {
        float baseMinutes = sessionMinutes > 0f ? sessionMinutes : (media != null ? media.avgSessionMinutes : 0f);
        return Math.Max(0f, baseMinutes * sessionMinutesMultiplier);
    }

    // 수익 추정(단순 위임)
    public float EstimateSessionRevenue(float minutes)
    {
        return media != null ? media.EstimateSessionRevenue(minutes) : 0f;
    }

    // 세션 효과(습관/브랜드 등) 계산(단순 위임)
    public (float deltaHabit, float deltaBrand) ComputeSessionEffects(float minutes)
    {
        return media != null ? media.ComputeSessionEffects(minutes) : (0f, 0f);
    }

    public bool Validate(out string problem)
    {
        if (media == null) { problem = "Task에 연결된 media가 없습니다."; return false; }
        if (capacityWeight < 0f) { problem = "capacityWeight는 음수일 수 없습니다."; return false; }
        problem = null; return true;
    }
}
