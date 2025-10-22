using System;
using System.Collections.Generic;
using UnityEngine;



// NetNodeInstance 스텁
[Serializable]
public class NetNodeInstance
{
    public SO_NodeParent node;
    private UserInstance mom;
    public List<NodeOperatorComboSet> input;
    public NodeParam bias;
    public SO_NodeOperator caculMethod;

    public NetNodeInstance(SO_NodeParent node, UserInstance user, List<NodeOperatorComboSet> input, NodeParam bias, SO_NodeOperator caculMethod)
    {
        this.node = node;
        this.mom = user;
        this.input = input ?? new List<NodeOperatorComboSet>();
        this.bias = bias;
        this.caculMethod = caculMethod;
    }

    public virtual void Init(UserInstance mom)
    {
        this.mom = mom;
    }

    public virtual float GetValue()
    {
        float b = bias.GetValue(mom);
        if (caculMethod != null)
            return caculMethod.GetValue(input, b, mom);
        return b;
    }

    public virtual void AffectsBias(float value, float amount = 0.1f)
    {
        bias.value = Mathf.Clamp(bias.value + value * amount, -1f, 1f);
    }

    [ContextMenu("GetFromSource")]
    public void GetFromSource()
    {
        if (node != null)
        {
            input = node.input ?? new List<NodeOperatorComboSet>();
            bias = node.bias;
            caculMethod = node.caculMethod;
        }
    }
}



// NodeParam, NodeOperatorComboSet, UserInstance
[Serializable]
public struct NodeParam
{
    public SO_NodeParent node;
    [Range(-1f, 1f)]
    public float value;

    public NodeParam(SO_NodeParent node, float value)
    {
        this.node = node;
        this.value = value;
    }

    public float GetValue(UserInstance user)
    {
        // 만약 node 참조가 있으면 그 노드의 값을 참조하는 규칙을 넣을 수 있음.
        return value;
    }
}

[Serializable]
public struct NodeOperatorComboSet
{
    public SO_NodeParent node;
    public NodeParam weight;

    public NodeOperatorComboSet(SO_NodeParent node, NodeParam weight)
    {
        this.node = node;
        this.weight = weight;
    }
}

[Serializable]
public class UserInstance
{
    public SO_TaskSlot[] taskSlots;
    public List<SO_NodeParent> nodeSources_NoUse;
    public List<NetNodeInstance> nodes;
    private Dictionary<SO_NodeParent, NetNodeInstance> _nodesDic = new Dictionary<SO_NodeParent, NetNodeInstance>();

    public UserInstance(SO_TaskSlot[] taskSlots, List<SO_NodeParent> nodeSources_NoUse, List<NetNodeInstance> nodes, Dictionary<SO_NodeParent, NetNodeInstance> nodesDic)
    {
        this.taskSlots = taskSlots;
        this.nodeSources_NoUse = nodeSources_NoUse ?? new List<SO_NodeParent>();
        this.nodes = nodes ?? new List<NetNodeInstance>();
        _nodesDic = nodesDic ?? new Dictionary<SO_NodeParent, NetNodeInstance>();
    }

    public void Init()
    {
        if (nodes == null) nodes = new List<NetNodeInstance>();
        foreach (var item in nodes)
        {
            item.Init(this);
            if (item.node != null && !_nodesDic.ContainsKey(item.node))
                _nodesDic.Add(item.node, item);
        }
    }

    public NetNodeInstance GetNode(SO_NodeParent node)
    {
        if (node == null) return null;
        _nodesDic.TryGetValue(node, out var a);
        if (a == null) UnityEngine.Debug.LogAssertion("Node not found: " + node.name);
        return a;
    }
}