using UnityEngine;


[System.Serializable]
public class ItemStack
{
    public int instanceId; // This is the instance id of the item on the inventory
    public Item item;
    public int orientation; 
    public int ammount;
    
    public ItemStack(int instanceId, Item item, int orientation, int ammount)
    {
        this.instanceId = instanceId;
        this.item = item;
        this.orientation = orientation;
        this.ammount = ammount;
    }

    public int getItemId()
    {
        return item.itemId;
    }
}
