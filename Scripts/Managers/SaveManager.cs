using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame() //check notes for why we do this
    {
        yield return new WaitForEndOfFrame();

        if (PlayerPrefs.HasKey("Save"))
            Load();
    }

    void Update()
    {
        if (Keyboard.current.nKey.wasPressedThisFrame)
            Save();
        
        if (Keyboard.current.mKey.wasPressedThisFrame)
            Load();
    }

    void Save() 
    {
        SaveData data = new SaveData();

        //player location
        data.playerPos = new SVec3(PlayerController.instance.transform.position);
        data.playerRot = new SVec3(PlayerController.instance.transform.eulerAngles);
        data.playerLook = new SVec3(PlayerController.instance.cameraContainer.localEulerAngles);

        //player needs
        data.health = PlayerNeeds.instance.health.curValue;
        data.sleep = PlayerNeeds.instance.sleep.curValue;
        data.thirst = PlayerNeeds.instance.thirst.curValue;
        data.hunger = PlayerNeeds.instance.hunger.curValue;

        //inventory
        data.inventory = new SInvetorySlot[Inventory.instance.slots.Length];

        for (int x = 0; x < Inventory.instance.slots.Length; x++) {
            data.inventory[x] = new SInvetorySlot();
            data.inventory[x].occupied = Inventory.instance.slots[x].item != null;

            if (!data.inventory[x].occupied) 
                continue;
            
            data.inventory[x].itemID = Inventory.instance.slots[x].item.id;
            data.inventory[x].quantity = Inventory.instance.slots[x].quantity;
            data.inventory[x].equipped = Inventory.instance.uiSlots[x].equipped;
        }

        //dropped items
        ItemObject[] droppedItems = FindObjectsOfType<ItemObject>();
        data.droppedItems = new SDroppedItem[droppedItems.Length];

        for (int x = 0; x < droppedItems.Length; x++) {
            data.droppedItems[x] = new SDroppedItem();
            data.droppedItems[x].itemID = droppedItems[x].item.id;
            data.droppedItems[x].position = new SVec3(droppedItems[x].transform.position);
            data.droppedItems[x].rotation = new SVec3(droppedItems[x].transform.eulerAngles);
        }

        //buildings
        Building[] buildingObjects = FindObjectsOfType<Building>();
        data.buildings = new SBuilding[buildingObjects.Length];

        for (int x = 0; x < buildingObjects.Length; x++) {
            data.buildings[x] = new SBuilding();
            data.buildings[x].buldingID = buildingObjects[x].data.id;
            data.buildings[x].position = new SVec3(buildingObjects[x].transform.position);
            data.buildings[x].rotation = new SVec3(buildingObjects[x].transform.eulerAngles);
            data.buildings[x].customProperties = buildingObjects[x].GetCustomProperties();
        }

        //resources
        data.resources = new SResource[ObjectManager.instance.resources.Length];

        for (int x=  0; x < ObjectManager.instance.resources.Length; x++) {
            data.resources[x] = new SResource();
            data.resources[x].index = x;
            data.resources[x].destroyed = ObjectManager.instance.resources[x] == null;

            if (!data.resources[x].destroyed)
                data.resources[x].capacity = ObjectManager.instance.resources[x].capacity;
        }

        //NPCs
        NPC[] nPCs = FindObjectsOfType<NPC>();
        data.npcs = new SNPC[nPCs.Length];

        for (int x = 0; x < nPCs.Length; x++) {
            data.npcs[x] = new SNPC();
            data.npcs[x].prefabId = nPCs[x].data.id;
            data.npcs[x].position = new SVec3(nPCs[x].transform.position);
            data.npcs[x].rotation = new SVec3(nPCs[x].transform.eulerAngles);
            data.npcs[x].aiState = (int)nPCs[x].aIState;                        //to convert an enumerator into an int for save data simply put the (int)
            data.npcs[x].hasAgentDestination = !nPCs[x].agent.isStopped;
            data.npcs[x].agentDestination = new SVec3(nPCs[x].agent.destination);
        }

        //time of day
        data.timeOfDay = DayNightCycle.instance.time;

        string rawData = JsonUtility.ToJson(data); //JsonUtility is a popular tool used to convert an object to a string

        //save it to our player prefs
        PlayerPrefs.SetString("Save", rawData);
    }

    void Load() 
    {
        SaveData data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("Save"));

        //player location
        PlayerController.instance.transform.position = data.playerPos.GetVector3();
        PlayerController.instance.transform.eulerAngles = data.playerRot.GetVector3();
        PlayerController.instance.cameraContainer.localEulerAngles = data.playerLook.GetVector3();

        //player needs
        PlayerNeeds.instance.health.curValue = data.health;
        PlayerNeeds.instance.sleep.curValue = data.sleep;
        PlayerNeeds.instance.thirst.curValue = data.thirst;
        PlayerNeeds.instance.hunger.curValue = data.hunger;

        //inventory
        int equippedItem = 999;

        for (int x = 0; x < data.inventory.Length; x++) {
            if (!data.inventory[x].occupied)
                continue;

            Inventory.instance.slots[x].item = ObjectManager.instance.GetItemByID(data.inventory[x].itemID);
            Inventory.instance.slots[x].quantity = data.inventory[x].quantity;

            if (data.inventory[x].equipped) {
                equippedItem = x;
            }
        }

        if (equippedItem != 999) {
            Inventory.instance.SelectItem(equippedItem);
            Inventory.instance.OnEquipButton();
        }

        //destroy all previous dropped items
        ItemObject[] droppedItems = FindObjectsOfType<ItemObject>();

        for (int  x = 0; x < droppedItems.Length; x++) 
            Destroy(droppedItems[x].gameObject);

        //spawn in saved dropped items
        for (int x = 0; x < data.droppedItems.Length; x++) {
            GameObject prefab = ObjectManager.instance.GetItemByID(data.droppedItems[x].itemID).dropPrefab;
            Instantiate(prefab, data.droppedItems[x].position.GetVector3(), Quaternion.Euler(data.droppedItems[x].rotation.GetVector3()));
        }
        
        //buildings
        for (int x = 0; x <data.buildings.Length; x++) {
            GameObject prefab = ObjectManager.instance.GetBuildingByID(data.buildings[x].buldingID).spawnPrefab;
            GameObject building  = Instantiate(prefab, data.buildings[x].position.GetVector3(), Quaternion.Euler(data.buildings[x].rotation.GetVector3()));
            building.GetComponent<Building>().RecieveCustomProperties(data.buildings[x].customProperties);
        }

        //resources
        for (int x = 0; x < ObjectManager.instance.resources.Length; x++) {
            if (data.resources[x].destroyed) {
                Destroy(ObjectManager.instance.resources[x].gameObject);
                continue;
            }

            ObjectManager.instance.resources[x].capacity = data.resources[x].capacity;
        }

        //NPCs
        NPC[]nPCs = FindObjectsOfType<NPC>();

        for (int x = 0; x < nPCs.Length; x++)
            Destroy(nPCs[x].gameObject);

        //spawn in saved NPCs
        for (int x = 0; x < data.npcs.Length; x++) {
            GameObject prefab = ObjectManager.instance.GetNPCByID(data.npcs[x].prefabId).spawnPrefab;
            GameObject npcObj = Instantiate(prefab, data.npcs[x].position.GetVector3(), Quaternion.Euler(data.npcs[x].rotation.GetVector3()));
            NPC npc = npcObj.GetComponent<NPC>();

            npc.aIState = (AIState)data.npcs[x].aiState;
            npc.agent.isStopped = !data.npcs[x].hasAgentDestination;

            if (!npc.agent.isStopped)
                npc.agent.SetDestination(data.npcs[x].agentDestination.GetVector3());
        }

        //time of day
        DayNightCycle.instance.time = data.timeOfDay;
    }
}
