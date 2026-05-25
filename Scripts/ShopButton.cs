using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [Header("📝 Текстовые элементы")]
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI descriptionText;
    
    public enum UpgradeType { Time, Speed, Value }
    public UpgradeType type;
    
    [Header("🎨 Визуал")]
    public Color affordableColor = Color.white;
    public Color notAffordableColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    
    private GameManager gameManager;
    private Button button;
    private Image buttonImage;

    void Start()
    {
        gameManager = GameManager.Instance;
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        
        // Если картинки нет, создаём автоматически
        if (buttonImage == null)
            buttonImage = gameObject.AddComponent<Image>();
            
        // Если тексты не перетащили, ищем их внутри кнопки сами
        if (priceText == null) priceText = GetComponentInChildren<TextMeshProUGUI>();
        if (descriptionText == null) descriptionText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateButtonText();
        UpdateButtonState();
    }

void Update()
{
    // ✅ Проверяем, что игра активна и GameManager существует
    if (GameManager.Instance == null || !GameManager.Instance.isGameActive)
        return;
        
    UpdateButtonState();
}
        void UpdateButtonText()
    {
        if (gameManager == null) return;

        switch (type)
        {
            case UpgradeType.Time:
                if (priceText != null)

                    priceText.text = gameManager.timeCost + " $"; 
                if (descriptionText != null)
                    descriptionText.text = "+15 сек";
                break;
                
            case UpgradeType.Speed:
                if (priceText != null)
                    priceText.text = gameManager.speedCost + " $"; 
                if (descriptionText != null)
                    descriptionText.text = $"Скорость x{gameManager.speedMultiplier:F1}";
                break;
                
            case UpgradeType.Value:
                if (priceText != null)
                    priceText.text = gameManager.valueCost + " $"; 
                if (descriptionText != null)
                    descriptionText.text = $"x{gameManager.resourceValueMultiplier} к очкам";
                break;
        }
    }

   void UpdateButtonState()
{
    // ✅ ДОБАВЬ ПРОВЕРКИ:
    if (gameManager == null || buttonImage == null)
        return;
    
    // Если игра закончена - блокируем все кнопки
    if (!gameManager.isGameActive)
    {
        buttonImage.color = notAffordableColor;
        if (GetComponent<Button>() != null)
            GetComponent<Button>().interactable = false;
        return;
    }

    bool canAfford = false;
    
    switch (type)
    {
        case UpgradeType.Time:
            canAfford = gameManager.score >= gameManager.timeCost;
            break;
        case UpgradeType.Speed:
            canAfford = gameManager.score >= gameManager.speedCost;
            break;
        case UpgradeType.Value:
            canAfford = gameManager.score >= gameManager.valueCost;
            break;
    }

    buttonImage.color = canAfford ? affordableColor : notAffordableColor;
    
    if (GetComponent<Button>() != null)
        GetComponent<Button>().interactable = canAfford;
}

    public void OnButtonClick()
    {
        if (gameManager == null) return;

        switch (type)
        {
            case UpgradeType.Time:
                if (gameManager.score >= gameManager.timeCost) gameManager.BuyTime();
                break;
            case UpgradeType.Speed:
                if (gameManager.score >= gameManager.speedCost) gameManager.BuySpeed();
                break;
            case UpgradeType.Value:
                if (gameManager.score >= gameManager.valueCost) gameManager.BuyValue();
                break;
        }
        UpdateButtonText();
    }
}