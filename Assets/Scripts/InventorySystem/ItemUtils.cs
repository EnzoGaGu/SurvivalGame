using System.Collections.Generic;
using UnityEngine;

public static class ItemUtils
{
    private static HashSet<int> instanceIds = new HashSet<int>();


    public static int GetNextFreeInstanceId()
    {
        int id = 1;

        while (instanceIds.Contains(id))
        {
            id++;
        }

        instanceIds.Add(id);
        return id;
    }
}
