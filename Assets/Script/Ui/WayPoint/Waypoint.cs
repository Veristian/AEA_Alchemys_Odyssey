using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public enum WaypointType
    {
        Normal,
        Destination
    }

    [Header("Waypoint Type")]
    [SerializeField] private WaypointType waypointType;

    [Header("Connections")]
    [SerializeField]
    private List<Waypoint> connectedWaypoints =
        new List<Waypoint>();

    public WaypointType Type => waypointType;

    public List<Waypoint> ConnectedWaypoints =>
        connectedWaypoints;

    private void OnDrawGizmos()
    {
        // Normal waypoint
        if (waypointType == WaypointType.Normal)
        {
            Gizmos.color = Color.cyan;
        }
        // Destination waypoint
        else
        {
            Gizmos.color = Color.yellow;
        }

        // Draw waypoint
        Gizmos.DrawSphere(
            transform.position,
            0.2f
        );

        // Draw connections
        foreach (
            Waypoint waypoint
            in connectedWaypoints
        )
        {
            if (waypoint != null)
            {
                Gizmos.DrawLine(
                    transform.position,
                    waypoint.transform.position
                );
            }
        }
    }
}