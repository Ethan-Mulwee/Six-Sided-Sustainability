using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class MilestonesSO : ScriptableObject
{
    public List<Milestone> milestones;
}
[Serializable]
public class Milestone
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public Texture2D Image { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int Action { get; private set; }
}
