using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Aloha Birds/Bird Prefab Registry")]
public class BirdPrefabRegistry : ScriptableObject
{
    [Serializable] public struct Entry { public BirdType type; public GameObject prefab; }
    public Entry[] entries;

    private Dictionary<BirdType, GameObject> _dict;
    private void OnEnable()
    {
        _dict = entries.ToDictionary(e => e.type, e => e.prefab);
    }
    public GameObject Get(BirdType t) => _dict[t];
}