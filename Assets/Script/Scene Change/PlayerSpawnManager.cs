using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSpawnManager : Singleton<PlayerSpawnManager>
{

    private string targetSpawnPointName;
    protected override void Awake()
    {
        transform.parent = null;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        base.Awake();
        DontDestroyOnLoad(gameObject);
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
        Debug.Log("sapwning");
        yield return new WaitUntil(() =>
            GameObject.FindAnyObjectByType<PlayerController>() != null &&
            GameObject.Find(targetSpawnPointName) != null);
        GameObject spawnPoint = GameObject.Find(targetSpawnPointName);

        if (spawnPoint != null)
        {
            GameObject player = GameObject.FindAnyObjectByType<PlayerController>(FindObjectsInactive.Exclude).gameObject;
            if (player != null)
            {
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.position = spawnPoint.transform.position;
                rb.rotation = spawnPoint.transform.rotation;
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

        // targetSpawnPointName = null; // Reset
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