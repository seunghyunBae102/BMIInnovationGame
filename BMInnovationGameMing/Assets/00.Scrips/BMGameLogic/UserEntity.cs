using System;
using UnityEngine;

[Serializable]
public class UserEntity
{
    public SO_UserBase template;           // SO ���ø�(������)
    public UserRuntimeState runtimeState;  // �� ������ ��Ÿ�� ����

    public UserEntity(SO_UserBase template, string id = null)
    {
        this.template = template;
        this.runtimeState = template.CreateRuntimeState(id);
    }

    // ���� ������(���� ȣ���� �ּ� �����Ϸ��� ���)
    public string Id => runtimeState?.userId;
}