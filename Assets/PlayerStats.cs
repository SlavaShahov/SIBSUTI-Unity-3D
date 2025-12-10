using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Present System")]
    public int totalPresentsInLevel = 15;

    private TMP_Text presentsText;
    private int collectedPresents = 0;
    private static PlayerStats instance;
    private static bool isUICreated = false;
    private bool allPresentsCollected = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CreateUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateUI()
    {
        if (isUICreated)
        {
            FindExistingUI();
            return;
        }

        // Создаем Canvas
        GameObject canvasGO = new GameObject("UICanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        DontDestroyOnLoad(canvasGO);

        // Создаем текстовый элемент
        GameObject textGO = new GameObject("PresentsText");
        presentsText = textGO.AddComponent<TextMeshProUGUI>();
        textGO.transform.SetParent(canvas.transform, false);

        // Настраиваем RectTransform для полного видимости
        RectTransform rect = presentsText.rectTransform;
        rect.anchorMin = new Vector2(0, 1);    // Top-Left
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(20, -20);
        rect.sizeDelta = new Vector2(600, 50); // Увеличил ширину и высоту

        // Настраиваем текст
        presentsText.text = $"Подарки: {collectedPresents}/{totalPresentsInLevel}";
        presentsText.fontSize = 28; // Увеличил размер шрифта
        presentsText.color = Color.white;
        presentsText.alignment = TextAlignmentOptions.TopLeft; // Выравнивание по левому краю
        presentsText.enableWordWrapping = false; // Отключаем перенос слов
        presentsText.overflowMode = TextOverflowModes.Overflow; // Разрешаем выход за границы
        presentsText.enableAutoSizing = true; // Автоподбор размера
        presentsText.fontSizeMin = 20; // Минимальный размер
        presentsText.fontSizeMax = 32; // Максимальный размер

        // Добавляем Outline для читаемости
        var outline = textGO.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2); // Увеличил контур

        isUICreated = true;
    }

    void FindExistingUI()
    {
        Canvas canvas = GameObject.Find("UICanvas")?.GetComponent<Canvas>();
        if (canvas != null)
        {
            Transform textTransform = canvas.transform.Find("PresentsText");
            if (textTransform != null)
            {
                presentsText = textTransform.GetComponent<TMP_Text>();

                // Восстанавливаем состояние если все подарки собраны
                if (allPresentsCollected && presentsText != null)
                {
                    presentsText.color = Color.green;
                    presentsText.text = "ВСЕ ПОДАРКИ СОБРАНЫ! СЧАСТЛИВОГО НОВОГО ГОДА!";
                }
            }
        }
        UpdatePresentsUI();
    }

    void Start()
    {
        UpdatePresentsUI();
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        FindExistingUI();
    }

    public void CollectPresent()
    {
        if (allPresentsCollected) return;

        collectedPresents++;

        if (collectedPresents >= totalPresentsInLevel)
        {
            AllPresentsCollected();
        }
        else
        {
            UpdatePresentsUI();
        }
    }

    void UpdatePresentsUI()
    {
        if (presentsText != null && !allPresentsCollected)
        {
            presentsText.text = $"Подарки: {collectedPresents}/{totalPresentsInLevel}";
        }
    }

    void AllPresentsCollected()
    {
        allPresentsCollected = true;
        Debug.Log("Все подарки собраны! Уровень пройден!");

        if (presentsText != null)
        {
            presentsText.color = Color.green;
            presentsText.text = "ВСЕ ПОДАРКИ СОБРАНЫ! СЧАСТЛИВОГО НОВОГО ГОДА!";
            presentsText.fontSize = 24; // Фиксированный размер для победы
            presentsText.enableAutoSizing = false; // Отключаем авторазмер
        }
    }

    public void ResetPresents()
    {
        collectedPresents = 0;
        allPresentsCollected = false;
        if (presentsText != null)
        {
            presentsText.color = Color.white;
            presentsText.enableAutoSizing = true; // Включаем авторазмер обратно
        }
        UpdatePresentsUI();
    }

    public int GetCollectedPresents()
    {
        return collectedPresents;
    }
}