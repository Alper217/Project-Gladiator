using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TraderSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    [Header("UI Components")]
    public Image itemIcon;
    public Text priceText;
    public Text stockText;
    public Button buyButton;

    private Trader.TraderItem traderItem;
    private Trader trader;

    void Awake()
    {
        // UI componentlerini otomatik bul
        itemIcon = transform.Find("ItemIcon")?.GetComponent<Image>();
        priceText = transform.Find("PriceText")?.GetComponent<Text>();
        stockText = transform.Find("StockText")?.GetComponent<Text>();
        buyButton = transform.Find("BuyButton")?.GetComponent<Button>();

        if (buyButton != null)
            buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    public void Initialize(Trader.TraderItem item, Trader traderRef)
    {
        traderItem = item;
        trader = traderRef;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (traderItem == null) return;

        if (itemIcon != null)
            itemIcon.sprite = traderItem.item.sprite;

        if (priceText != null)
            priceText.text = traderItem.buyPrice + " altın";

        if (stockText != null)
        {
            if (traderItem.stock == -1)
                stockText.text = "∞";
            else
                stockText.text = traderItem.stock.ToString();
        }

        // Satın alma butonu aktifliği
        if (buyButton != null)
        {
            bool canBuy = trader.playerMoney >= traderItem.buyPrice &&
                         traderItem.stock != 0 &&
                         Inventory.Singleton.HasEmptySlot();
            buyButton.interactable = canBuy;
        }
    }

    void OnBuyButtonClick()
    {
        trader.BuyItem(traderItem);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Sol tık ile satın al
            trader.BuyItem(traderItem);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Drag & Drop ile satış
        if (Inventory.carriedItem != null)
        {
            bool sold = trader.SellItem(Inventory.carriedItem);
            if (sold)
            {
                Inventory.carriedItem = null;
            }
        }
    }
}