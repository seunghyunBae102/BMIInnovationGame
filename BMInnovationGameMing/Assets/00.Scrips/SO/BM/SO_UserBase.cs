using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_UserBase", menuName = "SO/BM/SO_UserBase")]
public class SO_UserBase : ScriptableObject
{
    public List<SerializeNodeSO> nodes;
}

[Serializable]
public class SerializeNodeSO
{
    public SO_NodeParent node;

    public bool bIsParmOverride = false;

#if UNITY_EDITOR
    [ConditionalField("bIsParmOverride")]
# endif
    public SerializeableNodeParams overrideNode;

    //public SO_NodeParent GetNode()
    //{
    //    SO_NodeParent newNode = node.GetInstance();
        
    //    if(bIsParmOverride)
    //    {
    //        newNode.input = overrideNode.input;
    //        newNode.bias = overrideNode.bias;
    //        newNode.caculMethod = overrideNode.caculMethod;
    //    }

        
    //    return newNode;

    //}

    [ContextMenu("GetFromSource")]
    public void GetFromSource()
    {
        overrideNode.input = node.input;
        overrideNode.bias = node.bias;
        overrideNode.caculMethod = node.caculMethod;
    }
}
[Serializable]
public class SerializeableNodeParams
{
    public List<NodeOperatorComboSet> input;

    public NodeParam bias;
    public SO_NodeOperator caculMethod;
}