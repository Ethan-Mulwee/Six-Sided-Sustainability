using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class StructuresSO : ScriptableObject
{
    public List<Structure> Structures;
}

[Serializable]
public class Structure
{
    [field: SerializeField] public int StructureID { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public List<GameObject> Prefabs { get; private set; }
    [field: Header("Visual Stuff")]
    [field: SerializeField] public bool InhereitRotation { get; private set; }
    [field: SerializeField] public bool DeleteTrees { get; private set; }
    [field: SerializeField] public int Inventory { get; set; }

    [Serializable]
    public class UIElements
    {
        [field: SerializeField] public bool CreateUI;
        [field: SerializeField] public string ButtonName;
        [field: SerializeField] public string Tooltip;
        [field: SerializeField] public Texture2D ButtonIconDefault { get; private set; }
        [field: SerializeField] public Texture2D ButtonIconHover { get; private set; }
        [field: SerializeField] public Texture2D ButtonIconSelected { get; private set; }
    }

    [Serializable]
    public class Attributes
    {
        public enum StructureType
        {
            None,
            Residential,
            Office,
            Power,
            Coal,
            Eco,
            Farm
        }
        [field: SerializeField] public StructureType Type { get; private set; }
        [field: Header("Placement")]
        [field: SerializeField] public int PlacementCost { get; private set; }
        [field: SerializeField] public int ConstructionTime { get; private set; }

        [field: Header("Points")]
        [field: SerializeField] public int PointsGenerated { get; private set; }
        [field: SerializeField] public int ResidentalNeighborPoints { get; private set; }
        [field: SerializeField] public int OfficeNeighborPoints { get; private set; }

        [field: Header("Power")]
        [field: SerializeField] public int PowerGenerated { get; private set; }
        [field: SerializeField] public int PowerConsumed { get; private set; }

        [field: SerializeField] public int NeighborPowerBonus { get; private set; }
        [field: Header("Pollution")]
        [field: SerializeField] public bool GeneratesPollution { get; private set; }
        [field: SerializeField] public float PollutionGenerated { get; set; }
        [field: SerializeField] public int NeighborPollutionBonus { get; private set; }
        [field: SerializeField] public int CoalNeighborPollution { get; private set; }
    }
    public UIElements UI;
    public Attributes attributes;

}