using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    [Header("Portal Settings")]
    public string targetSceneName;
    public Transform spawnPoint;
    public string portalID;

    [Header("Destination Settings")]
    public string targetPortalID; // ID портала в целевой сцене

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Телепортация: {portalID} -> {targetPortalID} в {targetSceneName}");

            // Сохраняем ID портала-приемника
            PlayerPrefs.SetString("TargetPortalID", targetPortalID);
            PlayerPrefs.Save();

            SceneManager.LoadScene(targetSceneName);
        }
    }
}