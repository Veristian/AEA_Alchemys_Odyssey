using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;

    private string targetSpawnPointName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   // Now this will work
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSpawnInfo(string spawnName)
    {
        targetSpawnPointName = spawnName;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(targetSpawnPointName)) return;

        StartCoroutine(SpawnAfterLoad());
    }

    private IEnumerator SpawnAfterLoad()
    {
        yield return new WaitForSeconds(0.2f); // Increased delay for safety

        GameObject spawnPoint = GameObject.Find(targetSpawnPointName);

        if (spawnPoint != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
                Debug.Log($" Player teleported to: {targetSpawnPointName}");
            }
            else
            {
                Debug.LogError(" Could not find Player with tag 'Player'");
            }
        }
        else
        {
            Debug.LogError($" Spawn point '{targetSpawnPointName}' not found!");
        }

        targetSpawnPointName = null; // Reset
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}