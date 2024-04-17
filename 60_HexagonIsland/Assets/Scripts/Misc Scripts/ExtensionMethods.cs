using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static void DestroyTaggedChildren(this GameObject Parent, string Tag)
    {
        for (int i = 0; i < Parent.transform.childCount; i++) 
        {
            var Child = Parent.transform.GetChild(i);
            if (Child.gameObject.tag == Tag) Object.Destroy(Child.gameObject);
        }
    }
    public static void DestroyChildren(this GameObject Parent)
    {
        for (int i = 0; i < Parent.transform.childCount; i++)
        {
            var Child = Parent.transform.GetChild(i);
            Object.Destroy(Child.gameObject);
        }
    }
    public static void EnabledChildrenRenderers(this GameObject Parent, bool Boolean)
    {
        for (int i = 0; i < Parent.transform.childCount; i++)
        {
            var Child = Parent.transform.GetChild(i);
            if (Child.TryGetComponent(out Renderer renderer))
            {
                renderer.enabled = Boolean;
            }
        }
    }
    public static Tile GetTile(this List<Tile> tiles, int TileID)
    {
        var TileIndex = tiles.FindIndex(Tile => Tile.TileID == TileID);
        if (TileIndex != -1)
        {
            return tiles[TileIndex];
        }
        else return null;
    }
    public static Structure GetStructure(this List<Structure> structures, int StructureID)
    {
        var StructureIndex = structures.FindIndex(Structure => Structure.StructureID == StructureID);
        if (StructureIndex != -1)
        {
            return structures[StructureIndex];
        }
        else return null;
    }
}
