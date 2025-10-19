using System.Collections.Generic;
using UnityEngine;

public class Multiply_NodeOperator : SO_NodeOperator
{
    public override float GetValue(List<NodeOperatorComboSet> list, float bias, UserInstance user)
    {
        float a = 1;
        foreach (NodeOperatorComboSet combo in list)
        {
            a *= user.GetNode(combo.node).GetValue() * combo.weight.GetValue(user);
        }

        bias += list.Count > 0 ? (a / list.Count) : 0; // �ٸ� ���鿡�� �� ������ ��� + ����

        return bias;
    }
}
