using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Task", menuName = "SO/BM/SO_Task")]
public class SO_Task : ScriptableObject
{
    [Header("Identity")]
    public string taskId;
    public string displayName;

    [Header("Link to media (task delegates monetization/effects)")]
    public SO_Media media; // �۾��� ������ �Һ��ϴ� �̵�� �ڿ�

    [Header("Task parameters")]
    [Tooltip("���Կ��� �����ϴ� ����ġ(�뷮)")]
    public float capacityWeight = 1f;
    [Tooltip("�������ο��� ��� ����. media.offlineOk�� AND ó�� ����")]
    public bool offlineOk = false;
    [Tooltip("���� �⺻ ����(��). 0�̸� media.avgSessionMinutes ���")]
    public float sessionMinutes = 0f;
    [Tooltip("���� ���� ����(�̵�� �⺻�� * multiplier)")]
    public float sessionMinutesMultiplier = 1f;

    [Header("Metadata")]
    public string description;

    // ��Ÿ�� ���ؽ�Ʈ ����(NeuralNet �Է�) ����: SO_Media.BuildContextVector�� �����ϵ� task ���� ���� ���
    // ��ȯ ������ SimRunner���� ����ϴ� ���ؽ�Ʈ ���̿� ��ġ�ؾ� �մϴ�.
    public float[] BuildContextVector(UserRuntimeState userState, MediaRuntimeState mediaState, float slotTimePressure)
    {
        if (media == null)
            return new float[] { userState.mood, userState.fatigue, 0f, 0f, slotTimePressure, 0f, userState.desire };

        // media�� �⺻ ������ ����� ������ �� ��Ÿ�� ���·� �����.
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

    // �� Task�� ����Ǵ� ���� ����(��)
    public float EstimateSessionMinutes()
    {
        float baseMinutes = sessionMinutes > 0f ? sessionMinutes : (media != null ? media.avgSessionMinutes : 0f);
        return Math.Max(0f, baseMinutes * sessionMinutesMultiplier);
    }

    // ���� ����(�ܼ� ����)
    public float EstimateSessionRevenue(float minutes)
    {
        return media != null ? media.EstimateSessionRevenue(minutes) : 0f;
    }

    // ���� ȿ��(����/�귣�� ��) ���(�ܼ� ����)
    public (float deltaHabit, float deltaBrand) ComputeSessionEffects(float minutes)
    {
        return media != null ? media.ComputeSessionEffects(minutes) : (0f, 0f);
    }

    public bool Validate(out string problem)
    {
        if (media == null) { problem = "Task�� ����� media�� �����ϴ�."; return false; }
        if (capacityWeight < 0f) { problem = "capacityWeight�� ������ �� �����ϴ�."; return false; }
        problem = null; return true;
    }
}
