using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int itemId;
    public string itemName;
    public Sprite itemIcon;
    public bool isHoldable = false;
    public float swingCost = 20; // This is the stamina cost for swinging the item
    public bool shoots = false; // This is for items that shoot projectiles
    public GameObject prefab; // This is the item's prefab
    public GameObject holdPrefab; // This is the item's prefab when it's being held
    public int ammountPerSlot = 50; // This is the ammount of items that can be stored in a single slot
    public int xsize = 1;   // Horizontal slots that the object occupies
    public int ysize = 1;   // Vertical slots that the object occupies
    public bool unique = false; // This is for items that require a instanceId even if they're on the ground

    public string getItemName() {  return itemName; }
    public Sprite getItemIcon() { return itemIcon; }
    public int getItemId() { return itemId; }
}
