using UnityEngine;

public class MinionMovement : MonoBehaviour
{
    // Configuration Parameters
    [Header("Pathfinding")]
    [SerializeField] private Transform[] waypoints; // Array to hold the path waypoints
    [SerializeField] private float moveSpeed = 2f;  // Speed of the minion

    // State
    private int currentWaypointIndex = 0; // Tracks which waypoint we are heading towards

    void Start()
    {
        // Make sure waypoints are assigned and the array is not empty
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Waypoints not assigned or empty for " + gameObject.name);
            enabled = false; // Disable this script if no path is set
            return;
        }

        // Optional: Snap minion exactly to the first waypoint's position at the start
        transform.position = waypoints[currentWaypointIndex].position;
    }

    void Update()
    {
        // If there are no more waypoints, stop moving (or handle reaching the end)
        if (currentWaypointIndex >= waypoints.Length)
        {
            ReachEndOfPath();
            return; // Exit Update() early
        }

        MoveTowardsWaypoint();
    }

    private void MoveTowardsWaypoint()
    {
        // Get the target waypoint's position
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;

        // Calculate the distance to move this frame
        float step = moveSpeed * Time.deltaTime;

        // Move towards the target waypoint
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);

        // Check if we have reached the target waypoint
        // Using a small threshold for distance check is generally better than exact equality for floats
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            // We've reached the waypoint, move to the next one
            currentWaypointIndex++;
        }
    }

    private void ReachEndOfPath()
    {
        // What happens when the minion reaches the end?
        // For now, let's just destroy it. You can change this later!
        Debug.Log(gameObject.name + " reached the end of the path.");
        // Destroy(gameObject);

        // --- Other possible actions ---
        // - Reduce player health/lives
        // - Trigger a 'goal reached' event
        // - Play a sound/effect
        // - Just disable the minion instead of destroying: gameObject.SetActive(false);
    }

    // Public function to set the waypoints externally if needed (e.g., from a spawner)
    public void SetWaypoints(Transform[] newWaypoints)
    {
         waypoints = newWaypoints;
         currentWaypointIndex = 0; // Reset index
         if (waypoints != null && waypoints.Length > 0)
         {
              transform.position = waypoints[0].position; // Teleport to start
              enabled = true; // Make sure script is enabled
         }
         else
         {
              enabled = false; // Disable if path is invalid
         }
    }
}