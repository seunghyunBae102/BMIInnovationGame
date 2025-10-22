using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Task", menuName = "SO/BM/SO_Task")]
public class SO_Task : ScriptableObject
{
    [Range(0, 5)]
    public int heavyLevel = 1;
    //� ȯ�濡�� �� �� �ִ��� ����

    [Range(0, 5f)]
    public float tiredDamage = 1f;
    //�� Task���� ���� �� �߻��ϴ� �Ƿε��� ��.

    public List<SO_NodeBase> input;
    //��ǲ ������Ƽ
    //public �ൿ SO

    [SerializeField]
    protected string _description = "";
}
