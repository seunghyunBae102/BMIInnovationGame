using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_NodeBse", menuName = "SO/BM/SO_NodeBse")]

public class SO_NodeBase : SO_NodeParent
{

}

[Serializable]
public class NetNodeIntance
{
    public SO_NodeParent node;

    private UserInstance mom;

    public List<NodeOperatorComboSet> input;
    public NodeParam bias;
    public SO_NodeOperator caculMethod;

    public NetNodeIntance(SO_NodeParent node, UserInstance user, List<NodeOperatorComboSet> input, NodeParam bias, SO_NodeOperator caculMethod)
    {
        this.node = node;
        this.mom = user;
        this.input = input;
        this.bias = bias;
        this.caculMethod = caculMethod;
    }

    public virtual void Init(UserInstance mom)
    {
        this.mom = mom;
    }

    public virtual float GetValue()
    {
        return caculMethod.GetValue(input, bias.GetValue(mom), mom);
    }

    public virtual void AffectsBias(float value, float amount = 0.1f)
    {
        bias.value = Mathf.Lerp(bias.value, value, amount);
    }

    [ContextMenu("GetFromSource")]
    public void GetFromSource()
    {
        input = node.input;
        bias = node.bias;
        caculMethod = node.caculMethod;
    }
}