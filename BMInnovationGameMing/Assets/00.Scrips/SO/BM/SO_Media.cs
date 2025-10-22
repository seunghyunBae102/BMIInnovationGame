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

    [Header("Content metrics (0..1) - �����ͷ� ����")]
    [Range(0f, 1f)] public float appeal = 0.5f;       // ����ڰ� �ŷ������� ������ ����
    [Range(0f, 1f)] public float novelty = 0.5f;      // �ż���(���Ǵ� ����)
    [Range(0f, 1f)] public float relevance = 0.5f;    // ����/���缺
    [Range(0f, 1f)] public float brandStrength = 0.5f;// �귣�� ģ����
    [Range(0f, 1f)] public float habitability = 0.1f; // �ݺ� ��� �� ������ �Ǳ� ���� ����
    public ContentMode mode = ContentMode.Video;
    public bool offlineOk = false; // �������ο��� �Һ� ���� ����

    [Header("Slot / Time")]
    public float avgSessionMinutes = 20f;
    [Tooltip("���Կ��� �����ϴ� �ð� ����ġ(����)")]
    public float slotCapacityWeight = 1f;
    public bool requiresConnection = true;

    [Header("Monetization (�����ͷ� ����)")]
    [Tooltip("�д� ����� ��� ��� ����(USD ��)")]
    public float adRevenuePerMinute = 0.05f;
    [Range(0f,1f)] public float adFillRate = 0.8f;
    [Tooltip("�д� ���� ��ȯ Ȯ��(���)")]
    public float purchaseConversionPerMinute = 0.0005f;
    [Tooltip("��� ���� �ݾ�")]
    public float avgPurchaseValue = 2.0f;
    [Tooltip("������ �� ��� ARPU")]
    public float subscriptionMonthlyARPU = 0.0f;
    [Tooltip("�̵�� ���(����) ? ���� ��Ģ�� ���")]
    public float monthlyCost = 0.0f;

    [Header("Dynamics / ��ȭ")]
    [Tooltip("�Ϸ�� �ż��� ����")]
    public float noveltyDecayPerDay = 0.01f;
    [Tooltip("�Ϸ�� �귣�� ��ȭ")]
    public float brandDecayPerDay = 0.001f;
    [Tooltip("���Ǵ� ���� ������(���� �� ���¿� ����)")]
    public float habitIncreasePerSession = 0.01f;

    [Header("Metadata")]
    public string version = "1.0";

    // ��Ÿ�ӿ��� NN/SimRunner�� ȣ���� �ؼ� ������ ���ؽ�Ʈ ���͸� �����ϵ��� ����.
    // ���� �׸��� ����ȭ�Ǿ�� �ϸ�, NeuralNetSO�� �Է� ���ΰ� ��ġ��ų ��.
    // �⺻ ��ȯ: [userMood, userFatigue, novelty, relevance, slotTimePressure, brandStrength]
    public float[] BuildContextVector(float userMood, float userFatigue, float slotTimePressure, float userDesire)
    {
        return new float[]
        {
            Clamp01(userMood),
            Clamp01(userFatigue),
            Clamp01(novelty),
            Clamp01(relevance),
            slotTimePressure,       // �ð� �й��� �״�� (0..�� ����)
            Clamp01(brandStrength),
            Clamp01(userDesire)
        };
    }

    // �־��� ���� ����(��)�� ��� ������ �����Ѵ�.
    // NN/SimRunner�� �� ���� ����� ���� �� ���� ���� �ùķ��̼��� ���� ����.
    public float EstimateSessionRevenue(float sessionMinutes)
    {
        if (sessionMinutes <= 0f) return 0f;
        // ���� ����(�д�) * �� * ��������
        float adRev = adRevenuePerMinute * sessionMinutes * Clamp01(adFillRate);

        // ��� ���� �� = �д� ��ȯ�� * �� -> ��� ���ż� * ��ձ��Ű�
        float expectedPurchases = purchaseConversionPerMinute * sessionMinutes;
        float purchaseRev = expectedPurchases * Math.Max(0f, avgPurchaseValue);

        // �ܼ� �ջ�. ������ ���� �ܺο��� SO�� �Ķ���͸� �̿��� Ȯ��.
        return adRev + purchaseRev;
    }

    // ���� ���� �̵������ ���� ��ȭ ���(���� ������ ��길 ����).
    // ��ȯ��: (deltaUserHabit, deltaBrandStrength)
    public (float deltaHabit, float deltaBrand) ComputeSessionEffects(float sessionMinutes)
    {
        // ���� ������ ���� ���̿� habitability�� ���.
        float deltaHabit = habitIncreasePerSession * Mathf.Sqrt(Mathf.Max(0f, sessionMinutes)) * habitability;
        // �귣��� �������� ���� ��ȭ (����� ���迡 ����)
        float deltaBrand = 0.001f * Mathf.Log10(1f + sessionMinutes) * (appeal + relevance) * 0.5f;
        return (deltaHabit, deltaBrand);
    }

    // �Ϻ� ��ȭ: �ż������귣�� �ڿ�����
    public void ApplyDailyDecay(ref float inoutNovelty, ref float inoutBrand)
    {
        inoutNovelty = Mathf.Clamp01(inoutNovelty - noveltyDecayPerDay);
        inoutBrand = Mathf.Clamp01(inoutBrand - brandDecayPerDay);
    }

    public bool Validate(out string problem)
    {
        if (string.IsNullOrEmpty(mediaId)) { problem = "mediaId�� ����ֽ��ϴ�."; return false; }
        if (avgSessionMinutes < 0f) { problem = "avgSessionMinutes�� �����Դϴ�."; return false; }
        if (adRevenuePerMinute < 0f) { problem = "adRevenuePerMinute�� �����Դϴ�."; return false; }
        problem = null; return true;
    }

    private static float Clamp01(float v) => Mathf.Clamp01(v);
}
