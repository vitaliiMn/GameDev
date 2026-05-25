using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ✅ СОБЫТИЯ
    public System.Action<int> OnScoreChanged;
    public System.Action<string> OnPurchaseSuccess;
    public System.Action<string> OnPurchaseFailed;

    [Header("⏱️ Время и Очки")]
    public float timeLeft = 60f;
    public int score = 100;
    public bool isGameActive = true;

    [Header("⚡ Множители")]
    public float speedMultiplier = 1f;
    public int resourceValueMultiplier = 1;

    [Header("🛒 Цены магазина (фиксированные)")]
    public int timeCost = 50;
    public int speedCost = 100;
    public int valueCost = 150;

    [Header("📺 UI Ссылки")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public GameObject shopPanel;
    public TextMeshProUGUI endMessageText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
        if (endMessageText != null)
            endMessageText.gameObject.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        // ✅ Рестарт только ПОСЛЕ окончания игры
        if (!isGameActive && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
            return;
        }

        if (isGameActive)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
                GameOver(false);
            
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (timerText != null)
            timerText.text = Mathf.Ceil(timeLeft) + "s";
        if (scoreText != null)
            scoreText.text = score + " $";
    }

    // ✅ МЕТОД ДЛЯ СБОРА РЕСУРСОВ
    public void AddScore(int baseValue)
    {
        score += baseValue * resourceValueMultiplier;
        UpdateUI();
        OnScoreChanged?.Invoke(score);
    }

    // ✅ МЕТОД ДЛЯ ОТКРЫТИЯ МАГАЗИНА
    public void ToggleShop()
    {
        if (shopPanel == null)
        {
            Debug.LogError("ShopPanel не назначен!");
            return;
        }
        
        bool newState = !shopPanel.activeSelf;
        shopPanel.SetActive(newState);

        if (newState)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // ✅ ПОКУПКА ВРЕМЕНИ
    public void BuyTime()
    {
        if (score >= timeCost && isGameActive)
        {
            score -= timeCost;
            timeLeft += 15f;
            UpdateUI();
            
            OnPurchaseSuccess?.Invoke("Время +15 сек");
            if (NotificationSystem.Instance != null)
                NotificationSystem.Instance.ShowSuccess("Куплено время! +15 сек");
        }
        else
        {
            OnPurchaseFailed?.Invoke("Недостаточно очков!");
            if (NotificationSystem.Instance != null)
                NotificationSystem.Instance.ShowError("Недостаточно очков!");
        }
    }

    // ✅ ПОКУПКА СКОРОСТИ
    public void BuySpeed()
    {
        if (score >= speedCost && isGameActive)
        {
            score -= speedCost;
            speedMultiplier += 0.3f;
            UpdateUI();
            
            OnPurchaseSuccess?.Invoke("Скорость увеличена!");
            if (NotificationSystem.Instance != null)
                NotificationSystem.Instance.ShowSuccess("Скорость x" + speedMultiplier.ToString("F1"));
        }
        else
        {
            OnPurchaseFailed?.Invoke("Недостаточно очков!");
            if (NotificationSystem.Instance != null)
                NotificationSystem.Instance.ShowError("Недостаточно очков!");
        }
    }

    // ✅ ПОКУПКА МНОЖИТЕЛЯ ОЧКОВ
    public void BuyValue()
    {
        if (score >= valueCost && isGameActive)
        {
            score -= valueCost;
            resourceValueMultiplier *= 2;
            UpdateUI();
            
            OnPurchaseSuccess?.Invoke("Ценность ресурсов x" + resourceValueMultiplier);
            if (NotificationSystem.Instance != null)
                NotificationSystem.Instance.ShowSuccess("Ресурсы x" + resourceValueMultiplier);
        }
        else
        {
            OnPurchaseFailed?.Invoke("Недостаточно очков!");
            if (NotificationSystem.Instance != null)
                NotificationSystem.Instance.ShowError("Недостаточно очков!");
        }
    }

    // ✅ МЕТОД ДЛЯ ФИНИША
    public void WinGame()
    {
        if (isGameActive)
            GameOver(true);
    }

    // ✅ МЕТОД ОКОНЧАНИЯ ИГРЫ
    public void GameOver(bool won)
    {
        if (!isGameActive) return;
        
        isGameActive = false;
        if (endMessageText != null)
        {
            endMessageText.gameObject.SetActive(true);
            endMessageText.text = won 
                ? $"🏆 ПОБЕДА!\nОчки: {score}\nНажмите R для рестарта" 
                : "💀 ВЫ ПРОИГРАЛИ!\nНажмите R для рестарта";
            endMessageText.color = won ? Color.green : Color.red;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ✅ МЕТОД РЕСТАРТА
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}