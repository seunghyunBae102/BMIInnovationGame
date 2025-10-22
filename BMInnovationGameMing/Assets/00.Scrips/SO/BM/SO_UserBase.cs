using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_UserBase", menuName = "SO/BM/SO_UserBase")]
public class SO_UserBase : ScriptableObject
{

    public Dictionary<string, SO_TaskSlot[]> taskSlots; //주말-평일 -> 하루 일과 비는 시간 및 일하거나 학습하는 시간

    public List<NetNodeInstance> nodes;

    public List<SO_NodeParent> nodeSources_Reference;

    [ContextMenu("sourceSO2InstanceClass")]
    public void SOurce2nodes()
    {
        foreach (var item in nodeSources_Reference)
        {
            nodes.Add(new NetNodeInstance(item, null, item.input, item.bias, item.caculMethod));
        }
    }
}

[Serializable]
public class UserInstance
{
    public SO_TaskSlot[] taskSlots;

    public List<SO_NodeParent> nodeSources_NoUse;

    public List<NetNodeInstance> nodes;

    private Dictionary<SO_NodeParent,NetNodeInstance> _nodesDic;

    public UserInstance(SO_TaskSlot[] taskSlots, List<SO_NodeParent> nodeSources_NoUse, List<NetNodeInstance> nodes, Dictionary<SO_NodeParent, NetNodeInstance> nodesDic)
    {
        this.taskSlots = taskSlots;
        this.nodeSources_NoUse = nodeSources_NoUse;
        this.nodes = nodes;
        _nodesDic = nodesDic;
    }

    public void Init()
    {
        foreach(var item in nodes)
        {
            item.Init(this);
            _nodesDic.Add(item.node,item);
        }
    }

    public NetNodeInstance GetNode(SO_NodeParent node)
    {
        NetNodeInstance a = null;
        _nodesDic.TryGetValue(node,out a);
        if (a == null)
            UnityEngine.Debug.LogAssertion("Shit!");
        return a;
    }
}

