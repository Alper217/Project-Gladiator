using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trader : MonoBehaviour
{
    [Header("Trader Settings")]
    public string traderName = "Tüccar";
    public TraderItem[] traderItems;
    public float interactionDistance = 3f;

    [Header("UI References")]
    public GameObject traderPanel;
    public Transform traderSlotsParent;
    public InventorySlot traderSlotPrefab;
    public Text traderNameText;
    public Text playerMoneyText;
    public Button closeButton;

    [Header("Player Money")]
    public int playerMoney = 100;

    private Camera playerCamera;
    private bool isTraderOpen = false;
    private List<TraderSlot> traderSlots = new List<TraderSlot>();

    [System.Serializable]
    public class TraderItem
    {
        public Item item;
        public int buyPrice;
        public int sellPrice;
        public int stock = -1; // -1 = sonsuz stok
    }

    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
            playerCamera = FindObjectOfType<Camera>();

        traderPanel.SetActive(false);
        closeButton.onClick.AddListener(CloseTrader);

        SetupTraderSlots();
        UpdateUI();
    }

    void Update()
    {
        if (!isTraderOpen)
        {
            CheckForPlayerInteraction();
        }

        if (Input.GetKeyDown(KeyCode.E) && CanInteract())
        {
            OpenTrader();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isTraderOpen)
        {
            CloseTrader();
        }
    }

    bool CanInteract()
    {
        if (playerCamera == null) return false;

        Vector3 directionToTrader = transform.position - playerCamera.transform.position;
        float distance = directionToTrader.magnitude;

        if (distance > interactionDistance) return false;

        Vector3 forward = playerCamera.transform.forward;
        float angle = Vector3.Angle(forward, directionToTrader);

        return angle < 60f; // 120 derece görüþ açýsý
    }

    void CheckForPlayerInteraction()
    {
        if (CanInteract())
        {
            // Burada UI'da "E tuþuna basarak ticaret yap" yazýsý gösterebilirsin
        }
    }

    void SetupTraderSlots()
    {
        // Önceki slotlarý temizle
        foreach (Transform child in traderSlotsParent)
        {
            Destroy(child.gameObject);
        }
        traderSlots.Clear();

        // Yeni slotlarý oluþtur
        for (int i = 0; i < traderItems.Length; i++)
        {
            GameObject slotObj = Instantiate(traderSlotPrefab.gameObject, traderSlotsParent);
            TraderSlot traderSlot = slotObj.AddComponent<TraderSlot>();
            traderSlot.Initialize(traderItems[i], this);
            traderSlots.Add(traderSlot);
        }
    }

    public void OpenTrader()
    {
        isTraderOpen = true;
        traderPanel.SetActive(true);

        // Envanteri de aç
        InventoryManager.Instance.OpenInventory();

        UpdateUI();
    }

    public void CloseTrader()
    {
        isTraderOpen = false;
        traderPanel.SetActive(false);

        // Envanteri kapat
        InventoryManager.Instance.CloseInventory();
    }

    public bool BuyItem(TraderItem traderItem)
    {
        if (playerMoney < traderItem.buyPrice) return false;
        if (traderItem.stock == 0) return false;

        // Enventerde yer var mý kontrol et
        if (!Inventory.Singleton.HasEmptySlot()) return false;

        playerMoney -= traderItem.buyPrice;

        if (traderItem.stock > 0)
            traderItem.stock--;

        Inventory.Singleton.SpawnInventoryItem(traderItem.item);

        UpdateUI();
        return true;
    }

    public bool SellItem(InventoryItem inventoryItem)
    {
        // Bu item satýlabilir mi kontrol et
        TraderItem traderItem = GetTraderItemForSelling(inventoryItem.myItem);
        if (traderItem == null) return false;

        playerMoney += traderItem.sellPrice;

        // Item'i envanterden kaldýr
        inventoryItem.activeSlot.myItem = null;
        Destroy(inventoryItem.gameObject);

        UpdateUI();
        return true;
    }

    TraderItem GetTraderItemForSelling(Item item)
    {
        foreach (TraderItem traderItem in traderItems)
        {
            if (traderItem.item == item && traderItem.sellPrice > 0)
                return traderItem;
        }
        return null;
    }

    void UpdateUI()
    {
        traderNameText.text = traderName;
        playerMoneyText.text = "Para: " + playerMoney + " altýn";

        // Trader slotlarýný güncelle
        foreach (TraderSlot slot in traderSlots)
        {
            slot.UpdateSlot();
        }
    }
}