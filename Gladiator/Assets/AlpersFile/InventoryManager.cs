using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI References")]
    public GameObject inventoryPanel;
    public KeyCode inventoryKey = KeyCode.Tab;

    private bool inventoryOpen = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Envanter baþlangýçta kapalý
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }

        // ESC ile envanteri kapat
        if (Input.GetKeyDown(KeyCode.Escape) && inventoryOpen)
        {
            CloseInventory();
        }
    }

    public void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        inventoryPanel.SetActive(inventoryOpen);

        // Cursor kontrolü
        if (inventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Taþýnan item varsa býrak
            if (Inventory.carriedItem != null)
            {
                DropCarriedItem();
            }
        }
    }

    public void OpenInventory()
    {
        inventoryOpen = true;
        inventoryPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseInventory()
    {
        inventoryOpen = false;
        inventoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Taþýnan item varsa býrak
        if (Inventory.carriedItem != null)
        {
            DropCarriedItem();
        }
    }

    public bool IsInventoryOpen()
    {
        return inventoryOpen;
    }

    void DropCarriedItem()
    {
        if (Inventory.carriedItem != null)
        {
            // Item'i eski slotuna geri koy
            InventoryItem item = Inventory.carriedItem;
            if (item.activeSlot != null)
            {
                item.activeSlot.SetItem(item);
            }
        }
    }
}