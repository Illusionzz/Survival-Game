using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [HideInInspector]
    public ItemData[] items;
    [HideInInspector]
    public Resource[] resources;
    [HideInInspector]
    public BuildingData[] buildings;
    [HideInInspector]
    public NPCData[] nPCs;

    public static ObjectManager instance;

    void Awake()
    {
        instance = this;

        //load in all the assets we need
        items = Resources.LoadAll<ItemData>("Item");
        buildings = Resources.LoadAll<BuildingData>("Buildings");
        nPCs = Resources.LoadAll<NPCData>("NPCs");
    }

    void Start()
    {
        //get all of the resources
        resources = FindObjectsOfType<Resource>();
    }

    public ItemData GetItemByID(string id) 
    {
        for (int x = 0; x < items.Length; x++) {
            if (items[x].id == id)
                return items[x];
        }

        Debug.LogError("No items have been found.");
        return null;
    }

    public BuildingData GetBuildingByID(string id) 
    {
        for (int x = 0; x < buildings.Length; x++) {
            if (buildings[x].id == id)
                return buildings[x];
        }

        Debug.LogError("No buildings have been found.");
        return null;
    }

    public NPCData GetNPCByID(string id) 
    {
        for (int x = 0; x < nPCs.Length; x++) {
            if (nPCs[x].id == id)
                return nPCs[x];
        }

        Debug.LogError("No NPCs have been found.");
        return null;
    } 
}
