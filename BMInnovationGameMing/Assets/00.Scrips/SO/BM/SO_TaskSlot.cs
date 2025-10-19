using UnityEngine;

[CreateAssetMenu(fileName = "SO_TaskSlot", menuName = "SO/BM/SO_TaskSlot")]
public class SO_TaskSlot : ScriptableObject
{
    [Range(0, 5)]
    public int heavyLevel = 1;

    //0:밥먹을 때와 같이 손을 사용하지 못하는 상황 (영화, 애니, IDleGame 등)
    //1:출근 지하철같은 좁은 공공장소에서 짬시간에 하는거(시간과 공간을 별로 필요로 하지 않는 것) <- 오프라인 선호
    //2:쉬는 시간과 같은 짧지만, 자유로운 시간에 하는거(공간(인터넷 등)을 조금 필요로 하며, 시간을 별로 필요로 하지 않는것) <- 온라인/오프라인 동등, 오히려 온라인 선호(커뮤, 단순한 라이브 서비스 게임 등)
    //3:공간과 시간을 어느정도 필요로 하는 것(리그오브 레전드, 배틀그라운드 등 - PC를 제외한다는 가정이기에 -> 뭐있지...?)
    //4:G-Star 축제참여 등과 같이 시간과 공간 모두 필요로하는 것을 할 수 있는 시간.
    //5:일이나, 공부 등과 같이 다른 일을 아예 못하는 싱글스레드 일만 하는 시간. <- 

    //[Range(0, 5f)]
    //public float tiredDamage = 1f;
    ////한 Task에서 했을 때 발생하는 피로도의 양.

    [SerializeField]
    protected string _description = "";
}
