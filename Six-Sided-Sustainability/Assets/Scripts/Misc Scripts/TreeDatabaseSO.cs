using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class TreeDatabase : ScriptableObject
{
    public List<TreeData> treesData;
}

[Serializable]
public class TreeData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public int Type { get; private set; }
}