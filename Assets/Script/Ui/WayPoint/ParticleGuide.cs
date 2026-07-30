using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGuide : Singleton<ParticleGuide>
{
    [System.Serializable]
    public class Destination
    {
        public string name;
        public GameObject destinationObject;
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField] private Destination[] destinations;
    private Coroutine particlePathCoroutine;

    [Header("Particle Settings")]
    [SerializeField] private float particleSpacing = 1f;
    [SerializeField] private float particleHeight = 0.3f;
    [SerializeField] private float particleSpawnDelay = 0.08f;
    [SerializeField] private Transform ParticleContainer;

    [Header("Guide Settings")]
    [SerializeField] private float destinationReachDistance = 1.5f;
    [SerializeField] private float updateInterval = 0.5f;

    private Transform currentDestination;

    private List<Waypoint> currentPath =
        new List<Waypoint>();

    private List<ParticleSystem> spawnedParticles =
        new List<ParticleSystem>();

    private float updateTimer;

    private bool guideActive;

    private void Update()
    {
        if (!guideActive)
            return;

        if (player == null || currentDestination == null)
            return;

        // Check if player reached destination
        float distance = Vector3.Distance(
            player.position,
            currentDestination.position
        );

        if (distance <= destinationReachDistance)
        {
            HideGuide();
            return;
        }

        // Recalculate path periodically
        updateTimer -= Time.deltaTime;

        if (updateTimer <= 0f)
        {
            updateTimer = updateInterval;

            UpdateGuidePath();
        }
    }

    // Start guiding player to destination
    public void GuideTo(Transform destination)
    {
        if (destination == null)
        {
            Debug.LogWarning(
                "ParticleGuide: Destination is null."
            );

            return;
        }

        if (player == null)
        {
            Debug.LogWarning(
                "ParticleGuide: Player is not assigned."
            );

            return;
        }

        if (particlePrefab == null)
        {
            Debug.LogWarning(
                "ParticleGuide: Particle prefab is not assigned."
            );

            return;
        }

        currentDestination = destination;

        guideActive = true;

        updateTimer = 0f;

        CleanParticleRoutine();
        UpdateGuidePath();
    }

    private void UpdateGuidePath()
    {
        if (WaypointManager.Instance == null)
        {
            Debug.LogWarning(
                "ParticleGuide: WaypointManager not found."
            );

            return;
        }

        currentPath =
            WaypointManager.Instance
            .FindPathToDestination(
                player.position,
                currentDestination
            );

        if (currentPath == null ||
            currentPath.Count == 0)
        {
            ClearParticles();

            return;
        }

        CreateParticlePath();
    }

    //private void CreateParticlePath()
    //{
    //    ClearParticles();

    //    // Create path points
    //    List<Vector3> pathPoints =
    //        new List<Vector3>();

    //    // Start from player
    //    pathPoints.Add(player.position);

    //    // Add waypoint positions
    //    foreach (Waypoint waypoint in currentPath)
    //    {
    //        if (waypoint != null)
    //        {
    //            pathPoints.Add(
    //                waypoint.transform.position
    //            );
    //        }
    //    }

    //    // Add final destination
    //    pathPoints.Add(
    //        currentDestination.position
    //    );

    //    // Create particles between points
    //    for (int i = 0;
    //         i < pathPoints.Count - 1;
    //         i++)
    //    {
    //        CreateParticlesBetweenPoints(
    //            pathPoints[i],
    //            pathPoints[i + 1]
    //        );
    //    }
    //}

    private void CreateParticlePath()
    {
        // Stop previous particle spawning
        if (particlePathCoroutine != null)
        {
            StopCoroutine(particlePathCoroutine);
        }

        //ClearParticles();

        particlePathCoroutine =
            StartCoroutine(
                SpawnParticlePath()
            );
    }

    private void CleanParticleRoutine()
    {
        if (particlePathCoroutine != null)
        {
            StopCoroutine(particlePathCoroutine);
        }

        ClearParticles();
    }

    private IEnumerator SpawnParticlePath()
    {
        // Create path points
        List<Vector3> pathPoints =
            new List<Vector3>();

        // Start from player
        pathPoints.Add(
            player.position
        );

        // Add waypoint positions
        foreach (
            Waypoint waypoint
            in currentPath)
        {
            if (waypoint != null)
            {
                pathPoints.Add(
                    waypoint.transform.position
                );
            }
        }

        // Add destination
        pathPoints.Add(
            currentDestination.position
        );

        // Spawn particles one by one
        for (int i = 0;
             i < pathPoints.Count - 1;
             i++)
        {
            Vector3 startPoint =
                pathPoints[i];

            Vector3 endPoint =
                pathPoints[i + 1];

            float distance =
                Vector3.Distance(
                    startPoint,
                    endPoint
                );

            int particleCount =
                Mathf.CeilToInt(
                    distance /
                    particleSpacing
                );

            for (int j = 0;
                 j < particleCount;
                 j++)
            {
                float t =
                    (float)j /
                    particleCount;

                Vector3 position =
                    Vector3.Lerp(
                        startPoint,
                        endPoint,
                        t
                    );

                position.y +=
                    particleHeight;

                ParticleSystem particle =
                    Instantiate(
                        particlePrefab,
                        position,
                        Quaternion.identity,
                        ParticleContainer
                    );

                particle.Play();

                spawnedParticles.Add(
                    particle
                );

                // Wait before spawning next particle
                yield return new WaitForSeconds(particleSpawnDelay);
            }
        }

        particlePathCoroutine = null;
    }

    private void CreateParticlesBetweenPoints(
        Vector3 startPoint,
        Vector3 endPoint)
    {
        float distance =
            Vector3.Distance(
                startPoint,
                endPoint
            );

        int particleCount =
            Mathf.CeilToInt(
                distance / particleSpacing
            );

        for (int i = 0;
             i < particleCount;
             i++)
        {
            float t =
                (float)i /
                particleCount;

            Vector3 position =
                Vector3.Lerp(
                    startPoint,
                    endPoint,
                    t
                );

            // Raise particle above ground
            position.y += particleHeight;

            ParticleSystem particle =
                Instantiate(
                    particlePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

            particle.Play();

            spawnedParticles.Add(
                particle
            );
        }
    }

    public void HideGuide()
    {
        guideActive = false;

        currentDestination = null;

        currentPath.Clear();

        ClearParticles();

        ShowAllDestinations();
    }

    private void ClearParticles()
    {
        foreach (
            ParticleSystem particle
            in spawnedParticles
        )
        {
            if (particle != null)
            {
                Destroy(
                    particle.gameObject
                );
            }
        }

        spawnedParticles.Clear();
    }

    public bool IsGuideActive()
    {
        return guideActive;
    }

    private void OnDestroy()
    {
        ClearParticles();
    }

    public void FindDestinationWithName(string name)
    {
        if (name == "None")
        {
            HideGuide();
            return;
        }
        foreach (Destination destination in destinations)
        {
            if (destination.name == name)
            {
                HideAllDestinations();

                // Show only the selected destination
                destination.destinationObject.SetActive(true);
                

                GuideTo(destination.destinationObject.transform);
                Marker marker = destination.destinationObject.GetComponent<Marker>();

                marker.SetTracking(true);   

                return;
            }
        }

        Debug.LogWarning(
            "Destination not found: " + name
        );
    }

    private void HideAllDestinations()
    {
        foreach (Destination destination in destinations)
        {
            if (destination.destinationObject != null)
            {
                destination.destinationObject.SetActive(false);
            }
        }
    }

    private void ShowAllDestinations()
    {
        foreach (Destination destination in destinations)
        {
            if (destination.destinationObject != null)
            {
                destination.destinationObject.SetActive(true);
                Marker marker = destination.destinationObject.GetComponent<Marker>();

                marker.SetTracking(false);
            }
        }
    }
}