using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "BM/NeuralNetSO")]
public class NeuralNetSO : ScriptableObject
{
    public int[] layerSizes = new int[0]; // [input, hidden..., output]
    public List<float> weights = new List<float>(); // flattened
    public List<float> biases = new List<float>();  // flattened
    public enum Activation { Linear, ReLU, Tanh, Sigmoid }
    public Activation hiddenActivation = Activation.Tanh;

    // prepared offsets for fast indexing (computed in editor or at runtime)
    [NonSerialized] private int[] weightOffsets;
    [NonSerialized] private int[] biasOffsets;
    [NonSerialized] private bool prepared = false;

    public void Prepare()
    {
        if (layerSizes == null || layerSizes.Length < 2) { prepared = false; return; }
        int layers = layerSizes.Length;
        weightOffsets = new int[layers];
        biasOffsets = new int[layers];
        int wIdx = 0, bIdx = 0;
        for (int l = 1; l < layers; l++)
        {
            weightOffsets[l] = wIdx;
            biasOffsets[l] = bIdx;
            wIdx += layerSizes[l] * layerSizes[l - 1];
            bIdx += layerSizes[l];
        }
        prepared = true;
    }

    public bool Validate()
    {
        if (layerSizes == null || layerSizes.Length < 2) return false;
        int expectedW = 0, expectedB = 0;
        for (int l = 1; l < layerSizes.Length; l++)
        {
            expectedW += layerSizes[l] * layerSizes[l - 1];
            expectedB += layerSizes[l];
        }
        return weights != null && biases != null && weights.Count == expectedW && biases.Count == expectedB;
    }

    // Evaluate single input -> writes into outBuf[outIndex .. outIndex+outSize-1]
    public bool TryEvaluate(float[] inBuf, int inIndex, float[] outBuf, int outIndex)
    {
        if (!Validate()) { Debug.LogWarning("NeuralNetSO: invalid shapes"); return false; }
        if (!prepared) Prepare();
        int layers = layerSizes.Length;
        // allocate small working buffers on stack-like arrays (reused array recommended by caller)
        float[] cur = new float[layerSizes[0]];
        Array.Copy(inBuf, inIndex, cur, 0, layerSizes[0]);

        int wBase = 0, bBase = 0;
        for (int l = 1; l < layers; l++)
        {
            int inSize = layerSizes[l - 1];
            int outSize = layerSizes[l];
            float[] next = new float[outSize];
            int wOff = weightOffsets[l];
            int bOff = biasOffsets[l];
            for (int o = 0; o < outSize; o++)
            {
                float sum = biases[bOff + o];
                int wRow = wOff + o * inSize;
                for (int i = 0; i < inSize; i++) sum += weights[wRow + i] * cur[i];
                bool isOutputLayer = (l == layers - 1);
                next[o] = ApplyActivation(sum, isOutputLayer ? Activation.Linear : hiddenActivation);
            }
            cur = next;
        }

        // write outputs
        Array.Copy(cur, 0, outBuf, outIndex, cur.Length);
        return true;
    }

    private float ApplyActivation(float x, Activation act)
    {
        switch (act)
        {
            case Activation.ReLU: return Mathf.Max(0f, x);
            case Activation.Tanh: return (float)Math.Tanh(x);
            case Activation.Sigmoid: return 1f / (1f + (float)Math.Exp(-x));
            case Activation.Linear:
            default: return x;
        }
    }
}