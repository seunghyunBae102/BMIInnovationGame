using System.Collections.Generic;
using UnityEngine;

public class Multiply_NodeOperator : SO_NodeOperator
{
    public override float GetValue(List<NodeOperatorComboSet> list, float bias)
    {
        foreach (NodeOperatorComboSet combo in list)
        {
            bias *= combo.node.GetValue() * combo.weight.GetValue();
        }

        return bias;
    }
}
