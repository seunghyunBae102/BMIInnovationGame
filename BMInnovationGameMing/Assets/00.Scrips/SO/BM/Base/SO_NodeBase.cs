using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_NodeBse", menuName = "SO/BM/SO_NodeBse")]

public class SO_NodeBase : SO_NodeParent
{
    [SerializeField]
    private List<SO_NodeParent> _nodeSources_Ref;

    [SerializeField,Multiline]
    private string _description;

    [ContextMenu("Ref2SOurce")]
    public virtual void Ref2Source()
    {
        foreach (var item in _nodeSources_Ref)
        {
            input.Add(new(item, new(item,0.5f)));


        }
        if(bias.node ==null)
        {
            bias.node = this;
        }
        _nodeSources_Ref.Clear();
        var a = this.ToString().Replace(" (SO_NodeBase)", "");
        Key = a;
    }
}