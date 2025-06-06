using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] hotbarSlots;

    // 0=Head, 1=Chest, 2=Legs, 3=Feet
    [SerializeField] InventorySlot[] equipmentSlots;

    [SerializeField] Transform draggablesTransform;
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] Item[] items;

    [Header("Debug")]
    [SerializeField] Button giveItemBtn;

    void Awake()
    {
        Singleton = this;
        giveItemBtn.onClick.AddListener( delegate { SpawnInventoryItem(); } );
    }

    void Update()
    {
        if(carriedItem == null) return;

        carriedItem.transform.position = Input.mousePosition;
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if(carriedItem != null)
        {
            if(item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag) return;
            item.activeSlot.SetItem(carriedItem);
        }

        if(item.activeSlot.myTag != SlotTag.None)
        { EquipEquipment(item.activeSlot.myTag, null); }

        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }

    public void EquipEquipment(SlotTag tag, InventoryItem item = null)
    {
        switch (tag)
        {
            case SlotTag.Head:
                if(item == null)
                {
                    // Destroy item.equipmentPrefab on the Player Object;
                    Debug.Log("Unequipped helmet on " + tag);
                }
                else
                {
                    // Instantiate item.equipmentPrefab on the Player Object;
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case SlotTag.Chest:
                break;
            case SlotTag.Legs:
                break;
            case SlotTag.Feet:
                break;
        }
    }

    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item;
        if(_item == null)
        { _item = PickRandomItem(); }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Check if the slot is empty
            if(inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
    }
    // Inventory.cs'e eklenecek metodlar
    // Bu metodlarý mevcut Inventory sýnýfýnýza ekleyin

    public bool HasEmptySlot()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                return true;
            }
        }
        return false;
    }

    public int GetEmptySlotCount()
    {
        int emptySlots = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                emptySlots++;
            }
        }
        return emptySlots;
    }

    public bool RemoveItem(Item item, int quantity = 1)
    {
        int removed = 0;
        for (int i = 0; i < inventorySlots.Length && removed < quantity; i++)
        {
            if (inventorySlots[i].myItem != null && inventorySlots[i].myItem.myItem == item)
            {
                Destroy(inventorySlots[i].myItem.gameObject);
                inventorySlots[i].myItem = null;
                removed++;
            }
        }
        return removed == quantity;
    }

    public int CountItem(Item item)
    {
        int count = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem != null && inventorySlots[i].myItem.myItem == item)
            {
                count++;
            }
        }
        return count;
    }

    // InventorySlot.cs'e eklenecek metodlar
    // Bu metodlarý mevcut InventorySlot sýnýfýnýza ekleyin

    public void OnDrop(PointerEventData eventData)
    {
        // Trader slot'undan gelen drag&drop iþlemi için
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject != null)
        {
            TraderSlot traderSlot = draggedObject.GetComponent<TraderSlot>();
            if (traderSlot != null && Inventory.carriedItem == null)
            {
                // Trader'dan item satýn alma
                return;
            }
        }
    }

    // IDropHandler interface'ini InventorySlot'a eklemeyi unutmayýn:
    // public class InventorySlot : MonoBehaviour, IPointerClickHandler, IDropHandler
    Item PickRandomItem()
    {
        int random = Random.Range(0, items.Length);
        return items[random];
    }
}
