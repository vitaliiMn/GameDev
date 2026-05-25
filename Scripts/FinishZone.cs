using UnityEngine;

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в зону зашёл именно игрок
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.WinGame();
                Debug.Log(" Игрок достиг финиша!");
            }
        }
    }
}