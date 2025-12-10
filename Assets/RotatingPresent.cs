using UnityEngine;

public class RotatingPresent : MonoBehaviour
{
    [Header("Animation Settings")]
    public float rotationSpeed = 100f;
    public float floatHeight = 0.5f;
    public float floatSpeed = 2f;

    [Header("Identification")]
    public string presentId; // Уникальный ID для каждого подарка

    private Vector3 startPosition;
    private float randomOffset;
    private PresentManager presentManager;

    void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);

        // Находим менеджер подарков
        presentManager = FindObjectOfType<PresentManager>();
        if (presentManager == null)
        {
            // Создаем если нет
            GameObject pm = new GameObject("PresentManager");
            presentManager = pm.AddComponent<PresentManager>();
        }

        // Если ID не установлен, генерируем его из позиции
        if (string.IsNullOrEmpty(presentId))
        {
            presentId = $"{transform.position.x}_{transform.position.y}_{transform.position.z}";
        }

        // Проверяем, не собран ли уже этот подарок
        if (presentManager.IsPresentCollected(presentId))
        {
            // Если собран - уничтожаем
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed + randomOffset) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Сообщаем менеджеру, что подарок собран
            if (presentManager != null)
            {
                presentManager.MarkPresentAsCollected(presentId);
            }

            // Сообщаем игроку
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.CollectPresent();
            }

            AudioManager.PlaySantaLaughSound();

            // Уничтожаем подарок
            Destroy(gameObject);
        }
    }
}