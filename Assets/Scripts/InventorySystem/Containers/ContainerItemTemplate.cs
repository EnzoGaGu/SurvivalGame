using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ContainerItemTemplate
{
    public int itemId;
    public float durability = 0; // Durability of the item. If it's 0, the item doesn't have durability (item.unique == false)
    public int ammount;
    public int orientation = 0; 
}
