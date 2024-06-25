using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Preview : MonoBehaviour
{
    public static Preview instance;
    [SerializeField] private Material PreviewMaterial;
    private GameObject StoredPreviewInstance;

    private void Start()
    {
        instance = this;
        GameManager.StructureSelect += GameManager_OnStructureSelect;
        GameManager.StructureDeselect += DestroyPreview;
    }

    private void GameManager_OnStructureSelect()
    {
        SetPreview(GameManager.Building_ID);
    }

    public void Update()
    {
        //UpdatePosition();
    }
    public void UpdatePosition()
    {
        if (GameManager.Building_ID != 0 && GameManager.Hover != null)
        {
            gameObject.transform.position = GameManager.Hover.GetComponent<Hexagon>().ParentTile.TopPosition;
        }
    }

    public void SetPreview(int Building_ID)
    {
        gameObject.DestroyChildren();
        if (Building_ID != 0)
        {

        var PreviewInstance = Instantiate(TileManager.Instance.StructuresData.Structures.GetStructure(Building_ID).Prefabs[GameManager.Singleton.PrefabSelection], gameObject.transform.position, Quaternion.Euler(0,GameManager.BuildingRotation,0));

            //REMOVE THIS CODE LATER TESTING ONLY
        Renderer[] renderers = PreviewInstance.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = PreviewMaterial;
            }
            renderer.materials = materials;
        }
            //REMOVE THIS CODE LATER TESTING ONLY

            if (Building_ID == 6) { GameManager.Singleton.HexagonMaterial.SetInt("_Neighbors", 1); }
        else { GameManager.Singleton.HexagonMaterial.SetInt("_Neighbors", 0); }
        PreviewInstance.transform.SetParent(gameObject.transform, true);
            StoredPreviewInstance = PreviewInstance;
        }
    }
    public void UpdatePreview()
    {
        StoredPreviewInstance.transform.rotation = Quaternion.Euler(0, GameManager.BuildingRotation, 0);
    }

    public void DestroyPreview()
    {
        gameObject.DestroyChildren();
        GameManager.Singleton.HexagonMaterial.SetInt("_Neighbors", 0);
    }

}
