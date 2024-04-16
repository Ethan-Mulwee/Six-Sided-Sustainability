using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static GameManager;

public class Hexagon : MonoBehaviour
{
    public Tile ParentTile;

    public static Action CallUIUpdate;


    public void Construct(int Building_ID)
    {
        if (GameManager.Singleton.structuresSO.Structures[Building_ID].Inventory > 0 && GameManager.Hover.GetComponent<Hexagon>().ParentTile.TileType == 2 && GameManager.Hover.GetComponent<Hexagon>().ParentTile.TileStructure == null)
        {
            ParentTile.AddStructure(Building_ID, true);
            GameManager.Singleton.structuresSO.Structures[Building_ID].Inventory -= 1;

        }
        CallUIUpdate?.Invoke();
    }

    public void UnHover()
    {
        ParentTile.UnHover();
        Preview.instance.gameObject.SetActive(false);
        ParentTile.TreeVisibility(true);
    }

    public void Hover()
    {
        ParentTile.Hover();
        Preview.instance.UpdatePosition();
        Preview.instance.gameObject.SetActive(true);
        GameManager.Singleton.HexagonMaterial.SetVector("_Highlight", gameObject.transform.position);
    }


}
