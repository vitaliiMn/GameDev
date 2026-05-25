using UnityEngine;
using TMPro;

public class NotificationSystem : MonoBehaviour
{
    public static NotificationSystem Instance;
    
    [Header("📝 UI Элементы")]
    public TextMeshProUGUI notificationText;
    public RectTransform rectTransform;
    
    [Header("⏱️ Настройки")]
    public float displayDuration = 2f;
    public float fadeDuration = 0.5f;
    
    [Header("🎨 Цвета")]
    public Color successColor = Color.green;
    public Color errorColor = Color.red;
    public Color infoColor = Color.white;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    public void Show(string message, NotificationType type = NotificationType.Info)
    {
        if (notificationText == null)
        {
            Debug.LogWarning("NotificationSystem: notificationText не назначен!");
            return;
        }

        // Останавливаем предыдущую анимацию
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        // Устанавливаем текст и цвет
        notificationText.text = message;
        
        switch (type)
        {
            case NotificationType.Success:
                notificationText.color = successColor;
                break;
            case NotificationType.Error:
                notificationText.color = errorColor;
                break;
            default:
                notificationText.color = infoColor;
                break;
        }

        // Показываем уведомление
        notificationText.gameObject.SetActive(true);
        
        // Запускаем исчезновение
        fadeCoroutine = StartCoroutine(FadeOutAfterDelay());
    }

    public void ShowSuccess(string message)
    {
        Show(message, NotificationType.Success);
    }

    public void ShowError(string message)
    {
        Show(message, NotificationType.Error);
    }

    System.Collections.IEnumerator FadeOutAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;
        Color startColor = notificationText.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            notificationText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        notificationText.gameObject.SetActive(false);
        notificationText.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
    }

    public enum NotificationType
    {
        Info,
        Success,
        Error
    }
}