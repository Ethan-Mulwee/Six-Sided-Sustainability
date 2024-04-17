using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }
    public static List<Tile> tiles = new();
    public static List<Tile> PowerList1 = new();
    public static List<Tile> PowerList2 = new();

    //Assets
    public GameObject TestObject;
    public Material PreviewMaterial;
    public GameObject StatsPrefab;
    public GameObject hexagonPrefab1_Mountain;
    public GameObject hexagonPrefab2_Field;
    public TreeDatabase treeDatabase;
    public GameObject IconPrefab;
    public GameObject PointsAddedPrefab;
    public GameObject PollutionAddedPrefab;
    public GameObject Particles;

    [field: SerializeField] public StructuresSO StructuresData { get; private set; }

    private GameObject TilesParent;

    //Tile Stats
    public float PowerUse;
    public float PowerCap;
    public float Power;
    public int Points;
    public int PointsGoal;
    public float Pollution;

    public AudioClip Placesound;
    public AudioSource PlacementPlayer;

    public void RaiseMilestone()
    {
        PointsGoal += PointsGoal / 2;
    }

    void Start()
    {
        Instance = this;
        Intialize();
        IslandGenerator.instance.Generate();
        PointsGoal = 100;
    }

    private void Update()
    {
        if (TitleScript.playing == true)
        {
            PowerUse = 0;
            foreach (var tile in tiles)
            {
                if (tile.TileStructure != null)
                {
                    PowerUse += tile.PowerUse;
                }
            }
            foreach (var tile in tiles)
            {
                if (tile.TileStructure != null)
                {
                    PowerCap += tile.PowerCap;
                }
            }
            Power = PowerUse;
            EvalulatePower(PowerList1);
            EvalulatePower(PowerList2);
            //Power on all buildings
            foreach (Tile tile in tiles)
            {
                if (tile.PowerUse != 0)
                {
                    tile.Powered = true;
                }
            }
            //Power off structures with insufficent power
            if (Power > 0)
            {
                foreach (var tile in tiles)
                {
                    if (tile.PowerUse != 0)
                    {
                        tile.Powered = false;
                        Power -= tile.PowerUse;
                        if (Power <= 0)
                        {
                            break;
                        }
                    }
                }
            }
            UIScript.displayUpdate?.Invoke();
            }
    }
    float PowerMultiplier;
    private void EvalulatePower(List<Tile> PowerList)
    {
        PowerMultiplier = 1f; //((Mathf.Sin(Time.time)+1)/2)+0.5f;
        foreach (var tile in PowerList)
        {
            tile.Use = 0;
            if (Power > 0 && tile.PowerProduction *PowerMultiplier != 0)
            {
                if (tile.PowerProduction*PowerMultiplier < Power)
                {
                    Power -= tile.PowerProduction*PowerMultiplier;
                    tile.Use = 1;
                    //DEBUG: showing which power sources are being used
                    //tile.SetText("Power Used");
                    //tile.SetText(tile.Use.ToString());
                    //tile.ShowText();
                }
                else if (Power > 0)
                {
                    tile.Use = Power / (tile.PowerProduction*PowerMultiplier);
                    Power -= tile.PowerProduction*PowerMultiplier * tile.Use;
                    //DEBUG: showing which power sources are being used
                    //Debug.Log(Power + "/" + tile.PowerProduction);
                    //tile.SetText(tile.Use.ToString());
                    //tile.ShowText();
                }
            }

            //else tile.SetText(String.Empty);
            //tile.SetText((Mathf.RoundToInt(tile.Use*100)).ToString() + "%");
            //tile.ShowText();
        }
    }

    public void Intialize()
    {
        TilesParent = new GameObject("Tiles");
        tiles = new();
    }
    public void GenerateTown()
    {
        var TownCenter = tiles[UnityEngine.Random.Range(0, tiles.Count)];
        bool ValidNeighbors(Tile tile)
        {
            if (tile.Neighbors.Count == 6)
            {
                bool TypeCheck = true;
                foreach (Tile NeighborTile in tile.Neighbors)
                {
                    if (NeighborTile.TileType != 2)
                    {
                        TypeCheck = false; break;
                    }
                }
                if (TypeCheck == true)
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }
        while (ValidNeighbors(TownCenter) == false)
        {
            TownCenter = tiles[UnityEngine.Random.Range(0, tiles.Count)];
        }
        if (ValidNeighbors(TownCenter))
        {
            foreach (Tile NeighborTile in TownCenter.Neighbors)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    NeighborTile.AddStructure(1, false);
                }
            }
            TownCenter.AddStructure(7, false);
        }
        foreach (Tile tile in tiles)
        {
            tile.EvaluateNeighbors();
        }
    }
    public void AddTile(int TileID, int TileType, Vector3 TilePosition, Quaternion TileRotation, Vector3 TileScale)
    {
        var NewTile = new GameObject(TileID.ToString());
        NewTile.AddComponent<Tile>();
        var TileComponent = NewTile.GetComponent<Tile>();
        //Set GameObject to tile position purely for debugging purposes
        NewTile.transform.position = TilePosition;
        NewTile.transform.SetParent(TilesParent.transform, true);
        //Pass Tile IDs into Tile class
        TileComponent.TileID = TileID;
        TileComponent.TileType = TileType;
        //Pass transforms into Tile class
        TileComponent.TilePosition = TilePosition;
        TileComponent.TileRotation = TileRotation;
        TileComponent.TileScale = TileScale;

        tiles.Add(TileComponent);
    }
    public void AddTreesToTile(int TileID, List<Tree> Trees)
    {
        var TileIndex = tiles.FindIndex(Tile => Tile.TileID == TileID);
        for (int i = 0; i < Trees.Count; i++)
        {
            var Tree = Trees[i];
            tiles[TileIndex].Trees.Add(Tree);
        }
    }
    public void UpdateTiles()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            var tile = tiles[i];
            tile.UpdateTile();
        }
    }
    public void ClearTiles()
    {
        Destroy(TilesParent);
        Intialize();
    }
    public void FindTileNeighbors()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            var tile = tiles[i];
            tile.FindNeighbors();
        }
    }
}

public class Tile : MonoBehaviour
{
    public int TileID;
    public int TileType;
    public Vector3 TilePosition;
    public Quaternion TileRotation;
    public Vector3 TileScale;
    public Vector3 TopPosition;

    public List<Tree> Trees = new();
    public List<Tile> Neighbors = new();

    private GameObject TileObject;
    private GameObject TextParent;
    private GameObject TextObject;
    private GameObject IconObject;
    private GameObject TreesParent;
    private GameObject StructuresParent;
    public Structure TileStructure;
    private GameObject PlacementPreview;

    public float PowerUse = 0;
    public float PowerCap;
    public float PowerProduction;
    public bool Powered = false;
    public float Use = 0;
    public int PointProduction;
    public float PollutionProduction;

    private float timer = 0f;
    private void Update()
    {
        if (TitleScript.playing == true)
        {
            if (Powered == false && TileStructure != null && TileStructure.attributes.PowerConsumed != 0)
            {
                IconObject.SetActive(true);
            }
            else IconObject.SetActive(false);
            if (TileStructure != null && TileStructure.StructureID == 2)
            {
                if (timer < 5)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    timer = 0f;
                    PowerProduction =  TileStructure.attributes.PowerGenerated*(Mathf.Sin((0.05f) * Time.time) + 1);
                }
            }
        }
    }

    public void Awake()
    {
        TreesParent = new GameObject("Trees");
        TreesParent.transform.SetParent(gameObject.transform);
        StructuresParent = new GameObject("Structures");
        TextParent = new GameObject("Text");
        TextParent.transform.SetParent(gameObject.transform);
        StructuresParent.transform.SetParent(gameObject.transform);
        GameManager.ActionUpdate += EvaluateNeighbors;
        GameManager.PointUpdate += AddPoints;
        GameManager.PollutionUpdate += AddPollution;
    }
    void OnDisable()
    {
        GameManager.ActionUpdate -= EvaluateNeighbors;
        GameManager.PointUpdate -= AddPoints;
        GameManager.PollutionUpdate -= AddPollution;
    }
    public void AddPoints()
    {
        if (PointProduction != 0 && Powered == true)
        {
            TileManager.Instance.Points += PointProduction;
            var Rotation = Vector3.RotateTowards(new Vector3(TopPosition.x, TopPosition.y + 1f, TopPosition.z), Camera.main.transform.position, 20f, 0f) * -1;
            var Text = Instantiate(TileManager.Instance.PointsAddedPrefab, new Vector3(TopPosition.x, TopPosition.y + 1f, TopPosition.z), Quaternion.LookRotation(Rotation));
            var TextComponent = Text.GetComponentInChildren<TextMeshPro>();
            TextComponent.text = "+" + PointProduction.ToString();
            TextComponent.color = Color.green;
        }
    }
    public void AddPollution()
    {
        if (PollutionProduction != 0)
        {
            float PollutionGen = 0;
            if (TileStructure.attributes.PowerGenerated != 0)
            {
                PollutionGen = (Mathf.RoundToInt(PollutionProduction * Use*100))/100;
            }
            else PollutionGen = PollutionProduction;
            TileManager.Instance.Pollution += PollutionGen;
            var Rotation = Vector3.RotateTowards(new Vector3(TopPosition.x, TopPosition.y + 1f, TopPosition.z), Camera.main.transform.position, 20f, 0f) * -1;
            var Text = Instantiate(TileManager.Instance.PollutionAddedPrefab, new Vector3(TopPosition.x, TopPosition.y + 1f, TopPosition.z), Quaternion.LookRotation(Rotation));
            var TextComponent = Text.GetComponentInChildren<TextMeshPro>();
            TextComponent.text = "+" + (PollutionGen).ToString();
            TextComponent.color = new Color(0.85f, 0.85f, 0.85f);
        }
    }
    public void Hover()
    {
        if (GameManager.Building_ID != 0)
        {
            ShowText();
            SetText(PreviewNeighborEvaluation().ToString());
        }
        if (TileStructure != null && PointProduction != 0) SetText(PointProduction.ToString()); ShowText();
        ShowPreviewNeighborEffects();
    }
    public void UnHover()
    {
        HideText();
        SetText(String.Empty);
        UnShowPreviewNeighborEffects();

    }
    public void UpdateTile()
    {
        gameObject.DestroyTaggedChildren("Destroy");
        switch (TileType)
        {
            case 1:
                TileObject = Instantiate(TileManager.Instance.hexagonPrefab1_Mountain, TilePosition, TileRotation);
                break;
            case 2:
                TileObject = Instantiate(TileManager.Instance.hexagonPrefab2_Field, TilePosition, TileRotation);
                break;
        }
        TileObject.transform.localScale = TileScale;
        TileObject.transform.SetParent(gameObject.transform, true);
        TileObject.GetComponent<Hexagon>().ParentTile = this;

        TopPosition = new Vector3(TilePosition.x, (TileScale.y * 2), TilePosition.z);

        IntializeText();
        IntializeIcon();

        UpdateTrees();
    }
    public void IntializeIcon()
    {
        IconObject = Instantiate(TileManager.Instance.IconPrefab);
        IconObject.transform.position = new Vector3(gameObject.transform.position.x, TopPosition.y + 1.5f, gameObject.transform.position.z);
        IconObject.transform.SetParent(gameObject.transform, true);
        IconObject.SetActive(false);
    }
    public void UpdateTrees()
    {
        TreesParent.DestroyChildren();
        for (int i = 0; i < Trees.Count; i++)
        {
            var tree = Trees[i];
            var treeObject = Instantiate(TileManager.Instance.treeDatabase.treesData[tree.TreeType].Prefab, tree.TreePosition, tree.TreeRotation);
            treeObject.transform.SetParent(TreesParent.transform, true);
        }
    }
    public void TreeVisibility(bool Bool)
    {
        TreesParent.EnabledChildrenRenderers(Bool);
    }
    public void ClearTrees()
    {
        TreesParent.DestroyChildren();
        Trees.Clear();
    }
    public void FindNeighbors()
    {

        var number = TileID;

        int row = (int)((Math.Ceiling(number / IslandGenerator.x + 0.001f)) % 2);
        int OffsetIndex1 = (int)((number + IslandGenerator.x) - row);
        int OffsetIndex2 = (int)((number - IslandGenerator.x) - row);

        var Neighbor1 = (TileManager.tiles.GetTile(TileID + 1));
        if (Neighbor1 != null)
        {
            Neighbors.Add(Neighbor1);
        }

        var Neighbor2 = (TileManager.tiles.GetTile(TileID - 1));
        if (Neighbor2 != null)
        {
            Neighbors.Add(Neighbor2);
        }


        var Neighbor3 = (TileManager.tiles.GetTile(OffsetIndex1));
        if (Neighbor3 != null)
        {
            Neighbors.Add(Neighbor3);
        }

        var Neighbor4 = (TileManager.tiles.GetTile(OffsetIndex2));
        if (Neighbor4 != null)
        {
            Neighbors.Add(Neighbor4);
        }

        var Neighbor5 = (TileManager.tiles.GetTile(OffsetIndex1 + 1));
        if (Neighbor5 != null)
        {
            Neighbors.Add(Neighbor5);
        }

        var Neighbor6 = (TileManager.tiles.GetTile(OffsetIndex2 + 1));
        if (Neighbor6 != null)
        {
            Neighbors.Add(Neighbor6);
        }
        //Debug.Log(TileID.ToString() +" "+ Neighbor1 + " " +Neighbor2 + " " +Neighbor3 + " " +Neighbor4 + " " +Neighbor5 + " " +Neighbor6);
    }
    public void ShowPreviewNeighborEffects()
    {
        if (GameManager.Building_ID != 0)
        {
            foreach (Tile Neighbor in Neighbors)
            {
                if (Neighbor.TileStructure != null && Neighbor.TileStructure.attributes.Type == Structure.Attributes.StructureType.Residential && TileManager.Instance.StructuresData.Structures[GameManager.Building_ID].attributes.ResidentalNeighborPoints != 0)
                {
                    var Effect = TileManager.Instance.StructuresData.Structures.GetStructure(GameManager.Building_ID).attributes.ResidentalNeighborPoints;
                    Neighbor.SetText(Effect.ToString());
                    if (Effect > 0)
                    {
                        //Neighbor.TextObject.GetComponent<TextMeshPro>().color = Color.green;
                    }
                }
                Neighbor.ShowText();
            }
        }
    }
    public void UnShowPreviewNeighborEffects()
    {
        foreach (Tile Neighbor in Neighbors)
        {
            Neighbor.HideText();
            Neighbor.SetText(String.Empty);
        }
    }
    public void EvaluateNeighbors()
    {
        if (TileStructure != null)
        {
            PointProduction = TileStructure.attributes.PointsGenerated;
            foreach (Tile Neighbor in Neighbors)
            {
                if (Neighbor.TileStructure != null)
                {
                    if (TileStructure.attributes.Type == Structure.Attributes.StructureType.Residential)
                    {
                        PointProduction += Neighbor.TileStructure.attributes.ResidentalNeighborPoints;
                    }
                    if (TileStructure.attributes.Type == Structure.Attributes.StructureType.Office)
                    {

                    }
                }
                //Add Pollution Neighbor stuff
            }
        }
    }
    public int PreviewNeighborEvaluation()
    {
        var PreviewStructure = TileManager.Instance.StructuresData.Structures[GameManager.Building_ID];
        var PreviewPointProduction = PreviewStructure.attributes.PointsGenerated;
        foreach (Tile Neighbor in Neighbors)
        {
            if (Neighbor.TileStructure != null)
            {
                if (PreviewStructure.attributes.Type == Structure.Attributes.StructureType.Residential)
                {
                    PreviewPointProduction += Neighbor.TileStructure.attributes.ResidentalNeighborPoints;
                }
                if (PreviewStructure.attributes.Type == Structure.Attributes.StructureType.Office)
                {

                }
            }
        }
        return PreviewPointProduction;
    }
    public void AddStructure(int StructureID, bool Sound)
    {
        if (TileStructure == null)
        {
            StructuresParent.DestroyChildren();
            TileStructure = TileManager.Instance.StructuresData.Structures.GetStructure(StructureID);


            //Inc stats
            TileManager.Instance.PowerUse += TileStructure.attributes.PowerConsumed;

            PowerUse = TileStructure.attributes.PowerConsumed;
            PowerProduction = TileStructure.attributes.PowerGenerated;
            PointProduction = TileStructure.attributes.PointsGenerated;
            PollutionProduction = TileStructure.attributes.PollutionGenerated;

            var Instance = Instantiate(TileStructure.Prefabs[0], TopPosition, Quaternion.identity);
            Instance.transform.SetParent(StructuresParent.transform);
            if (TileStructure.DeleteTrees)
            {
                ClearTrees();
            }
            if (PowerProduction != 0 && PollutionProduction == 0)
            {
                TileManager.PowerList1.Add(this);
            }
            else if (PowerProduction != 0)
            {
                TileManager.PowerList2.Add(this);
            }
            Instantiate(TileManager.Instance.Particles, TopPosition, Quaternion.Euler(-90f, 0f, 0f));
            if (Sound)
            {

            TileManager.Instance.PlacementPlayer.Play();
            }
        }
        else Debug.Log("Structure already exists");
    }
    public void UpdateStructure(int StructureID)
    {
        StructuresParent.DestroyChildren();
        TileStructure = TileManager.Instance.StructuresData.Structures.GetStructure(StructureID);
        //Inc stats
        TileManager.Instance.PowerUse += TileStructure.attributes.PowerConsumed;


        var Instance = Instantiate(TileStructure.Prefabs[0], TopPosition, Quaternion.identity);
        Instance.transform.SetParent(StructuresParent.transform);
        ClearTrees();
    }

    public void IntializeText()
    {
        TextObject = Instantiate(TileManager.Instance.StatsPrefab);
        //TextObject.SetActive(true);
        TextObject.transform.position = new Vector3(gameObject.transform.position.x, TopPosition.y + 1.5f, gameObject.transform.position.z);
        TextObject.transform.SetParent(TextParent.transform);
        var Text = TextObject.GetComponent<TextMeshPro>();
        Text.text = String.Empty;
        Text.fontSize = 30f;
    }
    public void ShowText()
    {
        TextObject.SetActive(true);
    }
    public void SetText(string StatsText)
    {
        var Text = TextObject.GetComponent<TextMeshPro>();
        Text.text = StatsText;
    }
    public void HideText()
    {
        TextObject.SetActive(false);
        StopAllCoroutines();
    }
    public void ShowNeighborsText()
    {
        for (int i = 0; i < Neighbors.Count; i++)
        {
            Neighbors[i].ShowText();
        }
    }
    public void HideNeighborsText()
    {
        for (int i = 0; i < Neighbors.Count; i++)
        {
            Neighbors[i].HideText();
        }
    }
    public void SetNeighborsText(string NeighborText)
    {
        for (int i = 0; i < Neighbors.Count; i++)
        {
            Neighbors[i].SetText(NeighborText);
        }
    }

    public IEnumerator Construction(float ConstructionTime, int Building_ID)
    {
        yield return null;
    }
}



public class Tree
{
    public int TreeType;
    public Vector3 TreePosition;
    public Quaternion TreeRotation;
}