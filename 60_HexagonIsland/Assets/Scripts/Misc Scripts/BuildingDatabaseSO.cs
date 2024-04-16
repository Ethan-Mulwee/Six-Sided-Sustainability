using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class BuildingDatabaseSO : ScriptableObject
{
    public List<BuildingData> buildingsData;
}

[Serializable]
public class BuildingData
{
    [field: SerializeField] public string Name { get; private set; }
   // [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }

    public Attributes attributes;

}
[Serializable]
public class Attributes
{
    [field: Header("Placement")]
    [field: SerializeField] public int PlacementCost { get; private set; }
    [field: Header("Money Cost/Generation")]
    [field: SerializeField] public bool GeneratesMoney { get; private set; }
    [field: SerializeField] public int MoneyGenerated { get; private set; }
    [field: SerializeField] public int NeighborMoneybonus { get; private set; }
    [field: Header("Population")]
    [field: SerializeField] public bool hasPopulation { get; private set; }
    [field: SerializeField] public int Population { get; private set; }
    public int NeighborPopulationBonus { get; private set; }
    [field: Header("Power")]
    [field: SerializeField] public bool GeneratesPower { get; private set; }
    [field: SerializeField] public int PowerGenerated { get; private set; }
    [field: SerializeField] public bool ConsumesPower { get; private set; }
    [field: SerializeField] public int PowerConsumed { get; private set; }

    [field: SerializeField] public int NeighborPowerBonus { get; private set; }
    [field: Header("Pollution")]
    [field: SerializeField] public bool GeneratesPollution { get; private set; }
    [field: SerializeField] public int PollutionGenerated { get; private set; }
    [field: SerializeField] public int NeighborPollutionBonus { get; private set; }
}