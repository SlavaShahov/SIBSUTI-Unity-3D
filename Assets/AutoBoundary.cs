using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AutoBoundary : MonoBehaviour
{
    [Header("Автоматические границы")]
    public bool useAutoBoundaries = true;
    public float borderOffset = 5f;

    private Terrain terrain;
    private float minX, maxX, minZ, maxZ;

    void Start()
    {
        InitializeBoundaries();

        // Подписываемся на событие смены сцены
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ВЫЗЫВАЕТСЯ ПРИ КАЖДОЙ ЗАГРУЗКЕ НОВОЙ СЦЕНЫ
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Загружена новая сцена: {scene.name}");
        InitializeBoundaries(); // ПЕРЕСЧИТЫВАЕМ ГРАНИЦЫ ДЛЯ НОВОЙ СЦЕНЫ
    }

    void InitializeBoundaries()
    {
        if (!useAutoBoundaries) return;

        // Ищем террейн в НОВОЙ сцене
        terrain = FindObjectOfType<Terrain>();

        if (terrain != null)
        {
            CalculateBoundaries();
        }
        else
        {
            Debug.LogWarning("Террейн не найден в новой сцене! Использую границы по умолчанию.");
            SetDefaultBoundaries();
        }
    }

    void CalculateBoundaries()
    {
        // Получаем размеры террейна из НОВОЙ сцены
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPosition = terrain.transform.position;

        // Вычисляем границы с отступом
        minX = terrainPosition.x + borderOffset;
        maxX = terrainPosition.x + terrainSize.x - borderOffset;
        minZ = terrainPosition.z + borderOffset;
        maxZ = terrainPosition.z + terrainSize.z - borderOffset;

        Debug.Log($"Новые границы для сцены: X({minX} до {maxX}), Z({minZ} до {maxZ})");
    }

    void SetDefaultBoundaries()
    {
        // Границы по умолчанию для сцен без террейна
        minX = -250f; maxX = 250f;
        minZ = -250f; maxZ = 250f;
    }

    void Update()
    {
        HandleBoundaries();
    }

    void HandleBoundaries()
    {
        if (!useAutoBoundaries) return;

        Vector3 position = transform.position;
        float originalY = position.y;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.z = Mathf.Clamp(position.z, minZ, maxZ);
        position.y = originalY;

        transform.position = position;
    }

    void OnDestroy()
    {
        // ОТПИСЫВАЕМСЯ ОТ СОБЫТИЯ ПРИ УНИЧТОЖЕНИИ
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}