using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "SO_Event", menuName = "SO/BM/SO_Event", order = 1)]
public class SO_Event : ScriptableObject
{
    public UnityEvent Event;

    //이벤트 데이터 컨테이너인데, 다양한 파라미터 가지고 있어야 하노...? 유니티 이벤트식으로 개짬뽕 프로퍼티 저장하는거 개짜치는데 ㅇㅇ ㅗㅗ
}
