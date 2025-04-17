using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items = new List<Item>();
    public Item emptyItem; // Placeholder for empty slots and to manage non-existing items

    public Item getItemById(int id)
    {
        return items.Find(item => item.itemId == id);
    }

    public Item getEmptyItem()
    {
        return emptyItem;
    }
}
