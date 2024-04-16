using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    private bool Highlighted;


    public void DestoryObject()
    {
        Destroy(gameObject);
    }
    public void DeconstructHighlight(bool Bool)
    {
        //REMOVE THIS CODE LATER TESTING ONLY
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            if (Highlighted == false)
            {

            }
            Material[] materialsStored = renderer.materials;
            if (Bool)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = GameManager.Singleton.DeconstructionMaterial;
                }
                renderer.materials = materials;
                Highlighted = true;
            }
            else
            {
                renderer.materials = materialsStored;
                Highlighted = false;
            }

        }
    }
}
