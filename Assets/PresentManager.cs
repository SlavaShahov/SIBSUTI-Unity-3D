using System.Collections.Generic;
using UnityEngine;

public class PresentManager : MonoBehaviour
{
    private static PresentManager instance;
    private HashSet<string> collectedPresents = new HashSet<string>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Сохраняем ID собранного подарка
    public void MarkPresentAsCollected(string presentId)
    {
        collectedPresents.Add(presentId);
        Debug.Log($"Present {presentId} marked as collected. Total collected: {collectedPresents.Count}");
    }

    // Проверяем, собран ли подарок
    public bool IsPresentCollected(string presentId)
    {
        return collectedPresents.Contains(presentId);
    }

    // Очищаем сохраненные данные (при перезапуске игры)
    public void ClearCollectedPresents()
    {
        collectedPresents.Clear();
    }
}