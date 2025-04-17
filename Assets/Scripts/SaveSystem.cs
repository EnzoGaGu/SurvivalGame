using UnityEngine;

public static class SaveSystem
{
    static string initialPath = Application.persistentDataPath + "/Saves/";
    static string containerPath = initialPath + "Containers/";

    public static void SaveContainerData(ContainerData containerData)
    {
        string path = containerPath + containerData.containerId + ".json";
        string json = JsonUtility.ToJson(containerData, true);
        System.IO.File.WriteAllText(path, json);
    }

    public static ContainerData? LoadContainerData(string containerId)
    {
        string path = containerPath + containerId + ".json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            ContainerData containerData = JsonUtility.FromJson<ContainerData>(json);
            return containerData;
        }
        else
        {
            Debug.LogError("Container data file not found at: " + path);
            return null;
        }
    }
}
