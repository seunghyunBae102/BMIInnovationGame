using System;
using UnityEngine;

[Serializable]
public class UserEntity
{
    public SO_UserBase template;           // SO 템플릿(데이터)
    public UserRuntimeState runtimeState;  // 각 유저별 런타임 상태

    public UserEntity(SO_UserBase template, string id = null)
    {
        this.template = template;
        this.runtimeState = template.CreateRuntimeState(id);
    }

    // 편의 접근자(기존 호출을 최소 변경하려는 경우)
    public string Id => runtimeState?.userId;
}