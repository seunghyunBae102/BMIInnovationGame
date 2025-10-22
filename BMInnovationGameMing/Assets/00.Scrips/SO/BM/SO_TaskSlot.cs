using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_TaskSlot", menuName = "SO/BM/SO_TaskSlot")]
public class SO_TaskSlot : ScriptableObject
{
    public string slotId;
    [Tooltip("���� ����(��)")]
    public float slotMinutes = 30f;
    [Tooltip("�ð� �й�(�������� ���� ������ �پ��). SimRunner���� temperature�� �ݿ�")]
    public float timePressure = 1f;

    //0:����� ���� ���� ���� ������� ���ϴ� ��Ȳ (��ȭ, �ִ�, IDleGame ��)
    //1:��� ����ö���� ���� ������ҿ��� «�ð��� �ϴ°�(�ð��� ������ ���� �ʿ�� ���� �ʴ� ��) <- �������� ��ȣ
    //2:���� �ð��� ���� ª����, �����ο� �ð��� �ϴ°�(����(���ͳ� ��)�� ���� �ʿ�� �ϸ�, �ð��� ���� �ʿ�� ���� �ʴ°�) <- �¶���/�������� ����, ������ �¶��� ��ȣ(Ŀ��, �ܼ��� ���̺� ���� ���� ��)
    //3:������ �ð��� ������� �ʿ�� �ϴ� ��(���׿��� ������, ��Ʋ�׶��� �� - PC�� �����Ѵٴ� �����̱⿡ -> ������...?)
    //4:G-Star �������� ��� ���� �ð��� ���� ��� �ʿ���ϴ� ���� �� �� �ִ� �ð�.
    //5:���̳�, ���� ��� ���� �ٸ� ���� �ƿ� ���ϴ� �̱۽����� �ϸ� �ϴ� �ð�. <- 

    [Tooltip("���� �뷮 ����ġ(�Ϻ� Task�� �����ϴ� �뷮�� ��)")]
    public float capacityWeight = 1f;
    public bool requiresConnection = true;
    public bool allowOffline = false;

    [Header("Optional tags to constrain tasks (Ȯ�� ����Ʈ)")]
    public string[] allowedTaskTags;

    // Task ��� ���� �˻�(�⺻: ����/�������� ���, �뷮 �˻� ��)
    public bool AllowsTask(SO_Task task)
    {
        if (task == null) return false;
        if (requiresConnection && !task.offlineOk && !task.media.requiresConnection) return true; // slot �䱸�� �����̸� �⺻ ���
        if (!requiresConnection && task.offlineOk && !allowOffline) return false;
        // �±� ���Ͱ� ������ ��� ���� ��Ī(Ȯ�� �ʿ�)
        if (allowedTaskTags != null && allowedTaskTags.Length > 0)
        {
            // ���� Task�� �±װ� ������ �ź�(Ȯ�� �� Task�� �±� �ʵ� �߰�)
            // �⺻ ������� ��(������ Ȯ��)
        }
        return true;
    }
}
