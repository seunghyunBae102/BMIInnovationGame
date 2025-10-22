using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Task", menuName = "SO/BM/SO_Task")]
public class SO_Task : ScriptableObject
{
    [Range(0, 5)]
    public int heavyLevel = 1;

    [Range(0, 5f)]
    public float tiredDamage = 1f;
    //한 Task에서 했을 때 발생하는 피로도의 양.

    public List<NodeOperatorComboSet> input;
    //public 행동 SO

    [SerializeField]
    protected string _description = "";
}
