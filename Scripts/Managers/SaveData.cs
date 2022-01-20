using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //player location
    public SVec3 playerPos;
    public SVec3 playerRot;
    public SVec3 playerLook;

    //player needs
    public float health;
    public float hunger;
    public float thirst;
    public float sleep;

    //inventory
    public SInvetorySlot[] inventory;

    //dropped items
    public SDroppedItem[] droppedItems;

    //buildings
    public SBuilding[] buildings;

    //resources
    public SResource[] resources;

    //NPCs
    public SNPC[] npcs;

    //time
    public float timeOfDay;
}

[System.Serializable]
public struct SVec3 
{
    public float x;
    public float y;
    public float z;

    public SVec3 (Vector3 vector) 
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 GetVector3() 
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public struct SInvetorySlot 
{
    public bool occupied;
    public string itemID;
    public int quantity;
    public bool equipped;
}

[System.Serializable]
public struct SDroppedItem 
{
    public string itemID;
    public SVec3 position;
    public SVec3 rotation;
}

[System.Serializable]
public struct SBuilding 
{
    public string buldingID;
    public SVec3 position;
    public SVec3 rotation;
    public string customProperties;
}

[System.Serializable]
public struct SResource
{
    public int index;
    public bool destroyed;
    public int capacity;
}

[System.Serializable]
public struct SNPC 
{
    public string prefabId;
    public SVec3 position;
    public SVec3 rotation;
    public int aiState;            //storing it as an int because enums cannot be serailized
    public bool hasAgentDestination;
    public SVec3 agentDestination;

}