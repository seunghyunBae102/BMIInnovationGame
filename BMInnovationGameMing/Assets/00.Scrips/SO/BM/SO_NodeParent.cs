using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SO_NodeBse", menuName = "SO/BM/SO_NodeBse")]
public class SO_NodeParent : ScriptableObject
{
    public List<NodeOperatorComboSet> input;

    public NodeParam bias;

    public SO_NodeOperator caculMethod;

    public string Key;

    //public virtual float GetValue()
    //{
    //    return caculMethod.GetValue(input, bias.GetValue());
    //}

    //public virtual void AffectsBias(float value, float amount = 0.1f)
    //{
    //    bias.value = Mathf.Lerp(bias.value, value, amount);
    //}

}
