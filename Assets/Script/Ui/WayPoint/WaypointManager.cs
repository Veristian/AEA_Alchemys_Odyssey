using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance;

    [Header("Waypoint Settings")]
    [SerializeField]
    private List<Waypoint> allWaypoints =
        new List<Waypoint>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --------------------------------------------------
    // FIND CLOSEST NORMAL WAYPOINT
    // --------------------------------------------------

    public Waypoint GetClosestNormalWaypoint(
        Vector3 position)
    {
        Waypoint closestWaypoint = null;

        float closestDistance =
            Mathf.Infinity;

        foreach (
            Waypoint waypoint
            in allWaypoints)
        {
            if (waypoint == null)
                continue;

            // Ignore destination waypoints
            if (waypoint.Type !=
                Waypoint.WaypointType.Normal)
            {
                continue;
            }

            float distance =
                Vector3.Distance(
                    position,
                    waypoint.transform.position
                );

            if (distance <
                closestDistance)
            {
                closestDistance =
                    distance;

                closestWaypoint =
                    waypoint;
            }
        }

        return closestWaypoint;
    }

    // --------------------------------------------------
    // FIND DESTINATION WAYPOINT
    // --------------------------------------------------

    public Waypoint GetDestinationWaypoint(
        Transform destination)
    {
        if (destination == null)
            return null;

        Waypoint closestWaypoint = null;

        float closestDistance =
            Mathf.Infinity;

        foreach (
            Waypoint waypoint
            in allWaypoints)
        {
            if (waypoint == null)
                continue;

            // Only search destination waypoints
            if (waypoint.Type !=
                Waypoint.WaypointType.Destination)
            {
                continue;
            }

            float distance =
                Vector3.Distance(
                    destination.position,
                    waypoint.transform.position
                );

            if (distance <
                closestDistance)
            {
                closestDistance =
                    distance;

                closestWaypoint =
                    waypoint;
            }
        }

        return closestWaypoint;
    }

    // --------------------------------------------------
    // FIND PATH
    // --------------------------------------------------

    public List<Waypoint> FindPath(
        Waypoint startWaypoint,
        Waypoint targetWaypoint)
    {
        List<Waypoint> path =
            new List<Waypoint>();

        if (startWaypoint == null ||
            targetWaypoint == null)
        {
            return path;
        }

        if (startWaypoint ==
            targetWaypoint)
        {
            path.Add(
                startWaypoint
            );

            return path;
        }

        Queue<Waypoint> queue =
            new Queue<Waypoint>();

        Dictionary<Waypoint, Waypoint>
            previousWaypoint =
            new Dictionary<Waypoint, Waypoint>();

        HashSet<Waypoint> visited =
            new HashSet<Waypoint>();

        queue.Enqueue(
            startWaypoint
        );

        visited.Add(
            startWaypoint
        );

        while (queue.Count > 0)
        {
            Waypoint currentWaypoint =
                queue.Dequeue();

            foreach (
                Waypoint nextWaypoint
                in currentWaypoint
                    .ConnectedWaypoints)
            {
                if (nextWaypoint == null)
                    continue;

                if (visited.Contains(
                    nextWaypoint))
                {
                    continue;
                }

                visited.Add(
                    nextWaypoint
                );

                previousWaypoint[
                    nextWaypoint
                ] = currentWaypoint;

                queue.Enqueue(
                    nextWaypoint
                );

                if (nextWaypoint ==
                    targetWaypoint)
                {
                    return BuildPath(
                        startWaypoint,
                        targetWaypoint,
                        previousWaypoint
                    );
                }
            }
        }

        Debug.LogWarning(
            "No waypoint path found."
        );

        return path;
    }

    // --------------------------------------------------
    // BUILD PATH
    // --------------------------------------------------

    private List<Waypoint> BuildPath(
        Waypoint startWaypoint,
        Waypoint targetWaypoint,
        Dictionary<Waypoint, Waypoint>
            previousWaypoint)
    {
        List<Waypoint> path =
            new List<Waypoint>();

        Waypoint currentWaypoint =
            targetWaypoint;

        while (
            currentWaypoint !=
            startWaypoint)
        {
            path.Add(
                currentWaypoint
            );

            if (!previousWaypoint.ContainsKey(
                currentWaypoint))
            {
                return new List<Waypoint>();
            }

            currentWaypoint =
                previousWaypoint[
                    currentWaypoint
                ];
        }

        path.Add(
            startWaypoint
        );

        path.Reverse();

        return path;
    }

    // --------------------------------------------------
    // FIND ROUTE FROM PLAYER TO DESTINATION
    // --------------------------------------------------

    public List<Waypoint> FindPathToDestination(
        Vector3 playerPosition,
        Transform destination)
    {
        // Find closest NORMAL waypoint to player
        Waypoint startWaypoint =
            GetClosestNormalWaypoint(
                playerPosition
            );

        // Find destination waypoint
        Waypoint destinationWaypoint =
            GetDestinationWaypoint(
                destination
            );

        if (startWaypoint == null)
        {
            Debug.LogWarning(
                "No normal waypoint found near player."
            );

            return new List<Waypoint>();
        }

        if (destinationWaypoint == null)
        {
            Debug.LogWarning(
                "No destination waypoint found."
            );

            return new List<Waypoint>();
        }

        // Find route
        return FindPath(
            startWaypoint,
            destinationWaypoint
        );
    }
}