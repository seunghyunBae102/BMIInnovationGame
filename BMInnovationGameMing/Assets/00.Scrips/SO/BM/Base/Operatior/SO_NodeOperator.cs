using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
//[Serializable]
//public struct NodeOperator
//{
//    public List<NodeOperatorComboSet> input;
//    public NodeParam bias;
//    public SO_NodeOperator caculMethod;

//    public NodeOperator ( List<NodeOperatorComboSet> list, NodeParam bias, SO_NodeOperator cacul)
//    {
//        input = list;
//        this.bias = bias;
//        this.caculMethod = cacul;
//    }

//    public float GetValue()
//    {
//        return caculMethod.GetValue(input,bias.GetValue());
//    }

//}

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
        return node? user.GetNode(node).GetValue() : value; //null 이면 value 반환!
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
[CreateAssetMenu(fileName = "SO_NodeOperator", menuName = "SO/BM/SO_Operator")]

public class SO_NodeOperator : ScriptableObject
{

    public virtual float GetValue(List<NodeOperatorComboSet> list, float bias, UserInstance user)
    {
        float a = 0;
        foreach (NodeOperatorComboSet combo in list)
        {
            a += user.GetNode(combo.node).GetValue() * combo.weight.GetValue(user);
        }

        bias += list.Count>0 ? (a / list.Count):0; // 다른 노드들에서 온 값들의 평균 + 편향

        return bias;
    }
}
//파생으로 곱하고 나누고 평균구하는 기괴한거 만드려 했는데 AI 퍼셉트론 연산에서는 그럴 필요까진 없었노; 특수 노드연산의 확장 가능성은 있는
