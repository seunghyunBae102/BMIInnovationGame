using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BMNodeParamLoader : GetCompoableBase
{
    public string labelName = "Node"; // Addressables
    public List<SO_NodeBase> soList = new List<SO_NodeBase>();
    public Dictionary<string, SO_NodeBase> soDict = new Dictionary<string, SO_NodeBase>();

    public override void Init(GetCompoParent mom)
    {
        base.Init(mom);
        LoadAllSO();
    }
    [ContextMenu("LoadAllSO")]
    private void LoadAllSO()
    {
        Addressables.LoadAssetsAsync<SO_NodeBase>(labelName, null).Completed += OnSOAssetsLoaded;
    }

    private void OnSOAssetsLoaded(AsyncOperationHandle<IList<SO_NodeBase>> handle)
    {
        soDict.Clear();
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            soList = new List<SO_NodeBase>(handle.Result);
            Debug.Log($"Loaded {soList.Count} ScriptableObjects!");
        }
        else
        {
            Debug.LogError("Failed to load ScriptableObjects!");
        }

        foreach (var so in soList)
        {
            if (so != null && !soDict.ContainsKey(so.Key))
            {
                soDict.Add(so.Key, so);
            }
        }

    }

    public SO_NodeBase GetNodeByKey(string key)
    {
        if (soDict.TryGetValue(key, out var node))
        {
            return node;
        }
        Debug.LogWarning($"Node with key {key} not found.");
        return null;
    }
}
