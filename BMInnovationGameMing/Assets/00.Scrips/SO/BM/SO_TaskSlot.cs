using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_TaskSlot", menuName = "SO/BM/SO_TaskSlot")]
public class SO_TaskSlot : ScriptableObject
{
    public string slotId;
    [Tooltip("슬롯 길이(분)")]
    public float slotMinutes = 30f;
    [Tooltip("시간 압박(높을수록 선택 여지가 줄어듦). SimRunner에서 temperature에 반영")]
    public float timePressure = 1f;

    //0:밥먹을 때와 같이 손을 사용하지 못하는 상황 (영화, 애니, IDleGame 등)
    //1:출근 지하철같은 좁은 공공장소에서 짬시간에 하는거(시간과 공간을 별로 필요로 하지 않는 것) <- 오프라인 선호
    //2:쉬는 시간과 같은 짧지만, 자유로운 시간에 하는거(공간(인터넷 등)을 조금 필요로 하며, 시간을 별로 필요로 하지 않는것) <- 온라인/오프라인 동등, 오히려 온라인 선호(커뮤, 단순한 라이브 서비스 게임 등)
    //3:공간과 시간을 어느정도 필요로 하는 것(리그오브 레전드, 배틀그라운드 등 - PC를 제외한다는 가정이기에 -> 뭐있지...?)
    //4:G-Star 축제참여 등과 같이 시간과 공간 모두 필요로하는 것을 할 수 있는 시간.
    //5:일이나, 공부 등과 같이 다른 일을 아예 못하는 싱글스레드 일만 하는 시간. <- 

    [Tooltip("슬롯 용량 가중치(일부 Task가 차지하는 용량과 비교)")]
    public float capacityWeight = 1f;
    public bool requiresConnection = true;
    public bool allowOffline = false;

    [Header("Optional tags to constrain tasks (확장 포인트)")]
    public string[] allowedTaskTags;

    // Task 허용 여부 검사(기본: 연결/오프라인 요건, 용량 검사 등)
    public bool AllowsTask(SO_Task task)
    {
        if (task == null) return false;
        if (requiresConnection && !task.offlineOk && !task.media.requiresConnection) return true; // slot 요구가 연결이면 기본 허용
        if (!requiresConnection && task.offlineOk && !allowOffline) return false;
        // 태그 필터가 지정된 경우 간단 매칭(확장 필요)
        if (allowedTaskTags != null && allowedTaskTags.Length > 0)
        {
            // 현재 Task에 태그가 없으면 거부(확장 시 Task에 태그 필드 추가)
            // 기본 허용으로 둠(유연성 확보)
        }
        return true;
    }
}
