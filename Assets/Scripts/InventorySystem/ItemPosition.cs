using UnityEngine;

public struct ItemPosition
{
    public int x;
    public int y;
    public int orientation;
    public int xsize;
    public int ysize;
    public int instanceId;
    public string containerId; 

    public ItemPosition(int x, int y, int orientation, int xsize, int ysize, int instanceId, string containerId)
    {
        this.x = x;
        this.y = y;
        this.orientation = orientation;
        this.xsize = xsize;
        this.ysize = ysize;
        this.instanceId = instanceId;
        this.containerId = containerId;
    }

    public ItemPosition(int x, int y, int orientation, int xsize, int ysize, int instanceId)
    {
        this.x = x;
        this.y = y;
        this.orientation = orientation;
        this.xsize = xsize;
        this.ysize = ysize;
        this.instanceId = instanceId;
        this.containerId = ""; 
    }

    public ItemPosition(int x, int y, int orientation, int xsize, int ysize)
    {
        this.x = x;
        this.y = y;
        this.orientation = orientation;
        this.xsize = xsize;
        this.ysize = ysize;
        this.instanceId = -1; // Default value for instanceId
        this.containerId = "";
    }

    public ItemPosition(int x, int y, int orientation)
    {
        this.x = x;
        this.y = y;
        this.orientation = orientation;
        this.xsize = 1;
        this.ysize = 1;
        this.instanceId = -1; // Default value for instanceId
        this.containerId = "";
    }

    public ItemPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.orientation = 0;
        this.xsize = 1;
        this.ysize = 1;
        this.instanceId = -1; // Default value for instanceId
        this.containerId = "";
    }

    public ItemPosition(int x)
    {
        this.x = -1;
        this.y = -1;
        this.orientation = -1;
        this.xsize = -1;
        this.ysize = -1;
        this.instanceId = -1; // Default value for instanceId
        this.containerId = "";
    }
}
