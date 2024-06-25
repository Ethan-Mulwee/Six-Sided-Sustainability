using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using UnityEngine.UIElements;

public class IslandGenerator : MonoBehaviour
{
    [Header("Island Generation")]
    public float islandSize = 10f;
    public static float x = 0;
    public static float z = 0;
    [Header("Shape Perlin Noise Properties")]
    [SerializeField] private float NoiseScale = 0.1f;
    [SerializeField] private float NoiseStrength = 1f;
    private int seed = 0;
    [Header("Height Perlin Noise Properties")]
    [SerializeField] private float HeightNoiseScale = 0.25f;
    [SerializeField] private float HeightNoiseStrength = 0.28f;
    [Header("Island Mapping")]
    [SerializeField] private float Height = 0.75f;
    [SerializeField] private float HeightOffset = 0.12f;
    [Header("Tiles Assets")]
    [SerializeField] private GameObject hexagonPrefab1_Mountain;
    [SerializeField] private GameObject hexagonPrefab2_Field;
    [Header("Tree Assets")]
    [SerializeField] private TreeDatabase TreeDatabase;
    [Header("Misc Assets")]
    [SerializeField] private Material WaterShader;
    [SerializeField] private GameObject StatsText;

    [Header("Internal Variables")]
    internal List<Vector3> positions;
    internal List<Vector3> DistortedPositions;
    internal List<Vector4> computePositions;

    public ComputeShader computeShader;
    public RenderTexture renderTexture;

    public GameObject IslandParent;


    public static IslandGenerator instance;



    Vector3 Smoothstep(Vector3 Vector)
    {
        return new Vector3((math.pow(Vector.x, 2) * 3) - ((math.pow(Vector.x, 3) * 2)), (math.pow(Vector.y, 2) * 3) - ((math.pow(Vector.y, 3) * 2)), (math.pow(Vector.z, 2) * 3) - ((math.pow(Vector.z, 3) * 2)));
    }

    float PseudoRandom(Vector3 position)
    {
        return math.frac((math.sin(math.dot(position, new Vector3(12.9898f, 0, 78.233f)))) * 43758.5f);
    }

    float PerlinNoise(Vector3 position, float Scale, int Seed)
    {
        position = new Vector3((position.x * Scale) + Seed * 2, (position.y * Scale) + Seed * 2, (position.z * Scale) + Seed * 2);
        var Influence = new Vector3(math.floor(position.x), 0, math.floor(position.z));
        var Offset = new Vector3(position.x - Influence.x, 0, position.z - Influence.z);
        var Factor = (Smoothstep(Offset));
        float PerlinEval(Vector3 Offset, Vector3 Influence, Vector3 Eval)
        {
            return math.dot(new Vector3(Mathf.Cos((PseudoRandom(Influence + Eval) * (Mathf.PI * 2))), 0, Mathf.Sin((PseudoRandom(Influence + Eval) * (Mathf.PI * 2)))), Offset - Eval);
        }
        return math.lerp(math.lerp(PerlinEval(Offset, Influence, new Vector3(0, 0, 0)), PerlinEval(Offset, Influence, new Vector3(1, 0, 0)), Factor.x), math.lerp(PerlinEval(Offset, Influence, new Vector3(0, 0, 1)), PerlinEval(Offset, Influence, new Vector3(1, 0, 1)), Factor.x), Factor.z);
    }
    Vector3 Hexsnap(Vector3 position)
    {
        var sqrt3 = math.sqrt(3);
        var sqrt32 = sqrt3 / 2;
        var invsqrt3 = 1 / sqrt3;
        var Offset = (((((math.floor((position.x + sqrt32) * (1 / sqrt32))) % 2) * 2) - 1));
        var z = (math.floor(((position.z + 0.75) + ((((((0.5 / sqrt32) * position.x) + ((math.floor((position.x - sqrt32) * -invsqrt3)) * 0.5 - 0.25)) * Offset) * (((math.floor((1 / 1.5) * position.z)) % 2) * -2 + 1)))) * (2 / 3))) * 1.5;
        var x = ((math.floor((position.x + sqrt32) * (invsqrt3))) * sqrt3) + (Offset * ((z % 3) * (sqrt32 / 1.5)));
        return new Vector3((float)x, 0, (float)z);
    }

    float mapRange(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return ((value - fromMin) * (toMax - toMin) / (fromMax - fromMin)) + toMin;
    }


    //public void CalculateSDF()
    //{
    //    for (int i = 0; i < positions.Count; i++)
    //    {
    //        if (DistortedPositions[i].magnitude < islandSize)
    //        {
    //            computePositions.Add(new Vector4(positions[i].x, positions[i].y, positions[i].z, 0));
    //        }
    //    }
    //    Debug.Log("Real Positions: " + computePositions.Count);
    //    Debug.Log("Calculated Positions: " + positions.Count);
    //    var size = sizeof(float) * 4;
    //    ComputeBuffer PositionsBuffer = new ComputeBuffer(computePositions.Count, size);
    //    PositionsBuffer.SetData(computePositions);


    //    computeShader.SetInt("Count", computePositions.Count);
    //    computeShader.SetBuffer(0, "positions", PositionsBuffer);
    //    computeShader.SetFloat("Resolution", renderTexture.width);
    //    computeShader.SetTexture(0, "Result", renderTexture);
    //    computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

    //    PositionsBuffer.Dispose();
    //}

    private void Start()
    {
        instance = this;
    }

    public void DestoryIsland()
    {
        Destroy(IslandParent);
    }

    public void Generate()
    {
        IslandParent = new();
        IslandParent.name = "Island";

        positions = new();
        DistortedPositions = new();
        computePositions = new();


        //Intialize SDF renderTexture
        //renderTexture = new RenderTexture(1024, 1024, 24);
        //renderTexture.enableRandomWrite = true;
        //renderTexture.Create();

        seed = UnityEngine.Random.Range(0, 5000);

        x = (int)(islandSize * 2);
        z = (int)(islandSize * math.sqrt(3) * 1.5f);

        for (int i = 0; i < x * z; i++) { positions.Add(new Vector3(0, 0, 0)); DistortedPositions.Add(new Vector3(0, 0, 0)); }
        for (int i = 0; i < positions.Count; i++)
        {
            var position = positions[i];

            //Handle the Z
            position.z = (float)Math.Floor(i / x);

            //Handle the X
            position.x = (float)(Math.Floor(i % x) + (Math.Floor(position.z % 2) * .5f));

            //Center the grid
            //This code doesn't actually find the center of each hexagon it's to ensure the grid stays centered around 0,0 of the world
            Vector3 center = (new Vector3(x, 0, z) * .5f) + new Vector3(-.5f, 0, -.5f);
            Vector3 hexagonSpace = new((float)Math.Sqrt(3), 1, 1.5f);

            position = Vector3.Scale((position - center), hexagonSpace);

            //Distort Positions with 2 ocative Perlin Noise
            Vector3 distortedPositionVector = position * (((PerlinNoise(position, NoiseScale, seed) + (PerlinNoise(position, NoiseScale * 2, seed)) * 0.5f) * NoiseStrength) + 1f);
            //Scale gradient from halfway to the cneter to the outside
            var zscale = mapRange(distortedPositionVector.magnitude, (islandSize * 0.5f), islandSize, Height, 0);
            //Adding height offset and perlin noise, the noise falls off at the edges via the zscale gradient
            var distortedzscale = zscale + (Mathf.Lerp(HeightOffset, mapRange(PerlinNoise(position, HeightNoiseScale, seed), -0.01f, 0.36f, HeightOffset, HeightOffset + HeightNoiseStrength), zscale));
            Vector3 scale = new(0.95f, distortedzscale, 0.95f);

            positions[i] = position;
            DistortedPositions[i] = distortedPositionVector;

            bool NotOverlap(Vector3 position, List<Tree> positionsList)
            {
                for (int j = 0; j < positionsList.Count; j++)
                {
                    if (Vector3.Distance(positionsList[j].TreePosition, position) < 0.2)
                    {
                        //Debug.Log("Distance Check Failed");
                        return false;
                    }
                }
                return true;
            }

            Vector2 Vector2Rotate(Vector2 Vector, float Theta)
            {
                float Cosine = Mathf.Cos(Theta);
                float Sine = Mathf.Sin(Theta);
                return new Vector2((Vector.x * Cosine) - (Vector.y * Sine), (Vector.x * Sine) + (Vector.y * Cosine));
            }

            Vector3 RandomHexagonPostion(float Scale)
            {
                float Theta = UnityEngine.Random.Range(0f, 360f);
                float ModTheta = ((Theta) % 60) - 30;
                float h = (Mathf.Sqrt(3f) / 2f) / Mathf.Cos(ModTheta * Mathf.Deg2Rad);
                var Vector = (Vector2Rotate(new Vector2(h, 0), Theta * Mathf.Deg2Rad)) * Scale;
                return new Vector3(Vector.x, 0f, Vector.y);
            }

            void InstanceTrees(int amount, int RangeStart, int RangeEnd, int TileID)
            {
                RangeEnd = RangeEnd + 1;
                List<Tree> Trees = new();
                for (int i = 0; i < amount; i++)
                {

                    //Vector3 RandomPosition = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0f, UnityEngine.Random.Range(-0.5f, 0.5f));
                    Vector3 RandomPosition = RandomHexagonPostion(UnityEngine.Random.Range(0f, 0.85f));
                    int iterations = 0;
                    while (NotOverlap(RandomPosition, Trees) == false)
                    {
                        iterations++;
                        //RandomPosition = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0f, UnityEngine.Random.Range(-0.5f, 0.5f));
                        RandomPosition = RandomHexagonPostion(UnityEngine.Random.Range(0f, 0.85f));
                        if (iterations > 100) break;
                    }
                    iterations = 0;

                    //TreePostions.Add(RandomPosition);
                    var TreeIndex = UnityEngine.Random.Range(RangeStart, RangeEnd);
                    var NewTree = new Tree();
                    NewTree.TreeType = TreeIndex;
                    NewTree.TreePosition = new Vector3(position.x + RandomPosition.x, position.y + (2 * distortedzscale) - 0.05f, position.z + RandomPosition.z);
                    NewTree.TreeRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                    Trees.Add(NewTree);

                    //Instantiate(TreeDatabase.treesData[TreeIndex].Prefab, new Vector3(position.x + RandomPosition.x, position.y + (2 * distortedzscale) - 0.05f, position.z + RandomPosition.z), Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0)).transform.SetParent(hexagon.transform);
                }
                TileManager.Instance.AddTreesToTile(TileID, Trees);
            }


            if ((distortedPositionVector).magnitude < islandSize)
            {
                if (scale.y > 1.35)
                {
                    //TerrainID = 1;
                    TileManager.Instance.AddTile(i, 1, position, Quaternion.Euler(0, (60 * Mathf.Floor(UnityEngine.Random.Range(0, 3))) + (UnityEngine.Random.Range(-8, 8)), 0), scale);
                    //var hexagon = Instantiate(hexagonPrefab1_Mountain, position, Quaternion.Euler(0, (60 * Mathf.Floor(UnityEngine.Random.Range(0, 3))) + (UnityEngine.Random.Range(-8, 8)), 0));
                    //hexagon.transform.localScale = Vector3.Scale(hexagon.transform.localScale, scale);
                    //hexagon.transform.SetParent(IslandParent.transform, true);
                    //hexagon.name = i.ToString();
                    //hexagon.GetComponent<Hexagon>().TerrainID = 1;
                }
                else
                {
                    //TerrainID = 2;
                    //var hexagon = Instantiate(hexagonPrefab2_Field, position, Quaternion.Euler(0, (60*Mathf.Floor(UnityEngine.Random.Range(0, 3)))+(UnityEngine.Random.Range(-8,8)), 0));
                    TileManager.Instance.AddTile(i, 2, position, Quaternion.Euler(0, (60 * Mathf.Floor(UnityEngine.Random.Range(0, 3))) + (UnityEngine.Random.Range(-8, 8)), 0), scale);
                    //hexagon.transform.localScale = Vector3.Scale(hexagon.transform.localScale, scale);
                    //hexagon.transform.SetParent(IslandParent.transform, true);
                    //hexagon.name = i.ToString();
                    //var HexagonScript = hexagon.GetComponent<Hexagon>();
                    //HexagonScript.TerrainID = 2;
                    var StatsTextInstance = Instantiate(StatsText, new Vector3(position.x, position.y, position.z), Quaternion.identity);
                    StatsTextInstance.transform.SetParent(IslandParent.transform, true);
                    //StatsTextInstance.transform.SetParent(hexagon.transform, false);
                    //HexagonScript.StatsText = StatsTextInstance;

                    if (scale.y > 0.2)
                    {
                        //TerrainID = 3;
                        //hexagon.GetComponent<Hexagon>().TerrainID = 3;

                        var TreeNoise = PerlinNoise(position, 0.2f, seed);
                        if (TreeNoise > -0.3 && scale.y < 1)
                        {
                            InstanceTrees(3, 0, 0, i);
                        }
                        if (TreeNoise > 0)
                        {
                            if (scale.y > 1 && scale.y < 1.35)
                            {
                                InstanceTrees(20, 2, 3, i);
                            }
                            else
                            {
                                InstanceTrees(8, 0, 2, i);
                            }
                        }
                        else
                        {
                            if (scale.y > 1 && scale.y < 1.35)
                            {
                                InstanceTrees(0, 2, 3, i);
                            }
                            else
                            {
                                InstanceTrees(0, 0, 2, i);
                            }
                        }
                    }
                }
            }
        }
        //CalculateSDF();
        WaterShader.SetTexture("_Texture2D", renderTexture);
        //IslandGenerated?.Invoke();
        TileManager.Instance.UpdateTiles();
        TileManager.Instance.FindTileNeighbors();
        TileManager.Instance.GenerateTown();
    }


}