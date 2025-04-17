using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class ContainerData
{
    public string containerId;
    public string containerName;
    public int containerSize = 20;
    public int rowSize = 5;
    public List<ItemStack> itemStack; // List of item stacks in the container
    public List<List<int>> inventoryGrid = new List<List<int>>(); //Grid of inventory slots. If ItemStack contains an empty item, there's nothing in that space 
    public bool initiallyFilled = false; // Indicates if the container has initial items (for naturally spawning containers)
    public List<ContainerItemTemplate> initialItems; // List of initial items in the container

    public ContainerData()
    {
        containerId = "";
        containerName = "";
        containerSize = 20;
        rowSize = 5;
        itemStack = new List<ItemStack>();
        inventoryGrid = new List<List<int>>();
        initiallyFilled = false;
        initialItems = new List<ContainerItemTemplate>();
    }
}
