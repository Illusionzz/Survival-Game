using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selcted Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatNames;
    public TextMeshProUGUI selectedItemStatValues;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;
    
    private int curEquipIndex;

    //components
    private PlayerController controller;
    private PlayerNeeds needs;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    //singleton
    public static Inventory instance;

    void Awake()
    {
        instance  = this;
        controller = GetComponent<PlayerController>();
        needs = GetComponent<PlayerNeeds>();
    }

    void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        //initialize the slots
        for (int x = 0; x < slots.Length; x++) {
            slots[x] = new ItemSlot();
            uiSlots[x].index = x;
            uiSlots[x].Clear();
        }

        ClearSelectedItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started) {
            Toggle();
        }
    }

    public void Toggle() 
    {
        if (inventoryWindow.activeInHierarchy) {
            inventoryWindow.SetActive(false);
            onCloseInventory.Invoke();
            controller.ToggleCursor(false);
        }
        else {
            inventoryWindow.SetActive(true);
            onOpenInventory.Invoke();
            ClearSelectedItemWindow();
            controller.ToggleCursor(true);
        }
    }

    public bool Isopen() 
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item) 
    {
        //if there is a stack of the current item thewn add it to the stack
        if (item.canStack) {
            ItemSlot slotToStackTo = GetItemStack(item);

            if (slotToStackTo != null) {
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();
        //if there is an empty slot then create a new stack
        if (emptySlot != null) {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }
        //if there is a full stack and no empty slots then throw the item away
        ThrowItem(item);
    }

    //spawns the item infront of the player
    void ThrowItem(ItemData item) 
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360.0f));
    }

    //updates the UI of the slots
    void UpdateUI() 
    {
        for (int x = 0; x < slots.Length; x++) {
            if (slots[x].item != null)
                uiSlots[x].Set(slots[x]);
            else
                uiSlots[x].Clear();
        }
    }

    //returns the item slot that the requested item can be stacked on
    //returns null if ther is no stack  available
    ItemSlot GetItemStack(ItemData item) 
    {
        for (int x = 0; x < slots.Length; x++) {
            if (slots[x].item == item && slots[x].quantity < item.maxStackAmount) {
                return slots[x];
            }
        }

        return null;
    }

    //returns an empty slot in the inventory
    //if ther are no empty slots, return null
    ItemSlot GetEmptySlot() 
    {
        for (int x = 0; x < slots.Length; x++) {
            if (slots[x].item == null) {
                return slots[x];
            }
        }
        return null;
    }

    public void SelectItem(int index) 
    {
        if (slots[index].item == null) {
            return;
        }
        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;

        //set stat values and names
        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        for (int x = 0; x < selectedItem.item.consumables.Length; x++) {
            selectedItemStatNames.text += selectedItem.item.consumables[x].type.ToString() + "\n";
            selectedItemStatValues.text += selectedItem.item.consumables[x].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped);
        unequipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);
        dropButton.SetActive(true);
    }

    void ClearSelectedItemWindow() 
    {
        //clear the text elements
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        //disable the buttons
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void OnUseButton() 
    {
        if (selectedItem.item.type == ItemType.Consumable) {
            for (int x = 0; x < selectedItem.item.consumables.Length; x++) {
                switch(selectedItem.item.consumables[x].type) {
                    case ConsumableType.Health: needs.Heal(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Hunger: needs.Eat(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Thirst: needs.Drink(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Sleep: needs.Sleep(selectedItem.item.consumables[x].value); break;
                }
            }
        }

        RemoveSelectedItem();
    }

    public void OnEquipButton() 
    {
        if (uiSlots[curEquipIndex].equipped)
            Unequip(curEquipIndex);

        uiSlots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        EquipManager.instance.EquipNew(selectedItem.item);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    void Unequip(int index) 
    {
        uiSlots[index].equipped = false;
        EquipManager.instance.Unequip();
        UpdateUI();

        if (selectedItemIndex == index) 
            SelectItem(index);
    }

    public void OnUnequipButton() 
    {
        Unequip(selectedItemIndex);
    }

    public void OnDropButton() 
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem() 
    {
        selectedItem.quantity--;

        if (selectedItem.quantity == 0) {
            if (uiSlots[selectedItemIndex].equipped == true) 
                Unequip(selectedItemIndex);
            selectedItem.item = null;
            ClearSelectedItemWindow();
        } 
        UpdateUI();
    }

    public void RemoveItem(ItemData item) 
    {
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].item == item) {
                slots[i].quantity--;

                if (slots[i].quantity == 0) {
                    if (uiSlots[i].equipped == true)
                        Unequip(i);

                    slots[i].item = null;
                    ClearSelectedItemWindow();
                }

                UpdateUI();
                return;
            }
        }
    }

    public bool HasItems(ItemData item, int quantity) 
    {
        int amount = 0;

        for (int i = 0; i < slots.Length; i ++) {
            if (slots[i].item == item)
                amount += slots[i].quantity;

            if (amount >= quantity)
                return true;
        }
        return false;
    }
}

public class ItemSlot 
{
    public ItemData item;
    public int quantity;
}
