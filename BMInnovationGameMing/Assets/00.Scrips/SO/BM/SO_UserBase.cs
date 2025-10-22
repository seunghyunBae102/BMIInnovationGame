using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_UserBase", menuName = "SO/BM/SO_UserBase")]
public class SO_UserBase : ScriptableObject
{
    // ���� �迭(������Ʈ �䱸�� ���� 2D �迭 �Ǵ� 1D�� �ٲ� ���)
    public SO_TaskSlot[,] taskSlots;

    // ��� �ý��� ������ ����: ���ø��� ����
    [ContextMenu("sourceSO2InstanceClass")]
    public void SOurce2nodes()
    {
        // ��� �ý����� ������� �ʴ� ��� ����Ӵϴ�.
    }

    // ���� SO�� ��Ÿ�� ���¸� �������� �ʽ��ϴ�.
    // CreateRuntimeState�� �� UserRuntimeState �ν��Ͻ��� ��ȯ�ϴ� ���丮 ���Ҹ� �մϴ�.
    public UserRuntimeState CreateRuntimeState(string userId = null)
    {
        return new UserRuntimeState
        {
            userId = userId ?? name
            ,mood = 0.5f,
            fatigue = 0f,
            desire = 0.5f,
            habit = 0f,
            balance = 10f
        };
    }

    // ���� media �ĺ� ��ȯ(����)
    public List<SO_Media> GetCandidateMedia(List<SO_Media> allMedia, float slotTimePressure)
    {
        var result = new List<SO_Media>();
        if (allMedia == null) return result;
        foreach (var m in allMedia)
        {
            if (m == null) continue;
            if (m.requiresConnection && !IsConnected()) continue;
            result.Add(m);
        }
        return result;
    }

    // Task �ĺ� ��ȯ(����)
    public List<SO_Task> GetCandidateTasks(List<SO_Task> allTasks, SO_TaskSlot slot)
    {
        var result = new List<SO_Task>();
        if (allTasks == null) return result;
        foreach (var t in allTasks)
        {
            if (t == null) continue;
            if (slot != null && slot.requiresConnection)
            {
                if (t.media == null || !t.media.requiresConnection) continue;
            }
            if (slot != null)
            {
                if (t.capacityWeight > Mathf.Max(0.0001f, slot.capacityWeight * 2f)) continue;
            }
            result.Add(t);
        }
        return result;
    }

    private bool IsConnected()
    {
        // ��Ʈ��ũ ���뼺 üũ�� ������Ʈ ���ؿ��� �����ϼ���.
        return true;
    }
}

