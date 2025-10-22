using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_UserBase", menuName = "SO/BM/SO_UserBase")]
public class SO_UserBase : ScriptableObject
{
    // 슬롯 배열(프로젝트 요구에 따라 2D 배열 또는 1D로 바꿔 사용)
    public SO_TaskSlot[,] taskSlots;

    // 노드 시스템 의존성 제거: 템플릿만 유지
    [ContextMenu("sourceSO2InstanceClass")]
    public void SOurce2nodes()
    {
        // 노드 시스템을 사용하지 않는 경우 비워둡니다.
    }

    // 이제 SO는 런타임 상태를 저장하지 않습니다.
    // CreateRuntimeState는 새 UserRuntimeState 인스턴스를 반환하는 팩토리 역할만 합니다.
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

    // 기존 media 후보 반환(보존)
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

    // Task 후보 반환(보존)
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
        // 네트워크 가용성 체크는 프로젝트 수준에서 구현하세요.
        return true;
    }
}

