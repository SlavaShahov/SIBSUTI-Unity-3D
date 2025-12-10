using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public static PlayerManager instance;

    private GameObject currentPlayer;
    private string pendingPortalID;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        CreateOrFindPlayer();

        if (currentPlayer != null && !PlayerPrefs.HasKey("TargetPortalID"))
        {
            UseDefaultSpawn();
            EnsureCameraFollow();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerPrefs.HasKey("TargetPortalID"))
        {
            pendingPortalID = PlayerPrefs.GetString("TargetPortalID");
            PlayerPrefs.DeleteKey("TargetPortalID");
        }

        StartCoroutine(DelayedPosition());
    }

    IEnumerator DelayedPosition()
    {
        yield return new WaitForSeconds(0.1f);
        PositionPlayerAfterLoad();
    }

    void PositionPlayerAfterLoad()
    {
        CreateOrFindPlayer();

        if (!string.IsNullOrEmpty(pendingPortalID))
        {
            PositionPlayerAtPortal(pendingPortalID);
            pendingPortalID = null;
        }
        else UseDefaultSpawn();

        EnsureCameraFollow();
    }

    void CreateOrFindPlayer()
    {
        if (currentPlayer != null) return;

        currentPlayer = GameObject.FindGameObjectWithTag("Player");

        if (currentPlayer == null && playerPrefab != null)
        {
            currentPlayer = Instantiate(playerPrefab);
            currentPlayer.tag = "Player";
            Debug.Log("Player created");
        }
    }

    void PositionPlayerAtPortal(string portalID)
    {
        if (currentPlayer == null) return;

        ScenePortal targetPortal = FindPortalByID(portalID);
        if (targetPortal != null && targetPortal.spawnPoint != null)
        {
            PlayerController pc = currentPlayer.GetComponent<PlayerController>();
            if (pc != null)
                pc.TeleportPlayer(targetPortal.spawnPoint.position, targetPortal.spawnPoint.rotation);
        }
        else UseDefaultSpawn();
    }

    void UseDefaultSpawn()
    {
        if (currentPlayer == null) return;

        GameObject spawnPoint = GameObject.Find("PlayerSpawnPoint");
        if (spawnPoint != null)
        {
            PlayerController pc = currentPlayer.GetComponent<PlayerController>();
            if (pc != null)
                pc.TeleportPlayer(spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
        else
            currentPlayer.transform.position = Vector3.zero;
    }

    void EnsureCameraFollow()
    {
        if (currentPlayer == null) return;

        CameraFollow cam = FindObjectOfType<CameraFollow>();
        if (cam != null)
            cam.SetPlayer(currentPlayer.transform);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    ScenePortal FindPortalByID(string portalID)
    {
        ScenePortal[] portals = FindObjectsOfType<ScenePortal>();
        foreach (var p in portals)
            if (p.portalID == portalID)
                return p;
        return null;
    }
}
