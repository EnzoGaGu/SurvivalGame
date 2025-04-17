using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ContainerDatabase", menuName = "Inventory/Container Database")]
public class ContainerDatabase : ScriptableObject
{
    public List<ContainerData> containerData = new List<ContainerData>(); // List of container data

    public ContainerData getContainerById(string id)
    {
        return containerData.Find(container => container.containerId == id);
    }
}
