using UnityEngine;

[CreateAssetMenu(fileName = "SO_Task", menuName = "SO/BM/SO_Task")]
public class SO_Task : ScriptableObject
{
    [Range(0, 5)]
    public int heavyLevel = 1;

    [Range(0, 5f)]
    public float tiredDamage = 1f;
    //�� Task���� ���� �� �߻��ϴ� �Ƿε��� ��.

    //public �ൿ SO

    [SerializeField]
    protected string _description = "";
}
