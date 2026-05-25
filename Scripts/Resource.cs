using UnityEngine;

public class Resource : MonoBehaviour
{
    [Header("️ Настройки сферы")]
    [Tooltip("Базовое количество очков (Железо=1, Золото=3, Алмаз=10)")]
    public int baseValue = 1;

    [Header("🎨 Визуал (опционально)")]
    public Color sphereColor = Color.gray;

    void Start()
    {
        // Применяем цвет, если назначен материал
        var rend = GetComponent<Renderer>();
        if (rend != null && rend.material != null)
        {
            rend.material.color = sphereColor;
        }
    }

    // Срабатывает, когда игрок касается сферы
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        if (GameManager.Instance == null) return;

        // GameManager.AddScore автоматически умножит на resourceValueMultiplier
        GameManager.Instance.AddScore(baseValue);
        
        // Эффект сбора (можно добавить звук или частицы позже)
        Debug.Log($"Собрано: {baseValue} очков! (Множитель: x{GameManager.Instance.resourceValueMultiplier})");
        
        // Удаляем сферу
        Destroy(gameObject);
    }
}