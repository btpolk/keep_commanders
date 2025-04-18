using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Optional: If you want to display wave count on screen

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable] // Makes this visible and configurable in the Inspector
    public class Wave
    {
        public GameObject enemyPrefab; // The enemy type to spawn for this wave
        public int count;              // How many enemies in this wave
        [Range(0.1f, 5f)] // Clamp spawn interval to reasonable values
        public float spawnInterval = 0.5f; // Time between each enemy spawn within this wave
    }

    [Header("Wave Configuration")]
    [SerializeField] private Wave[] waves; // Array defining all the waves
    [SerializeField] private float timeBetweenWaves = 5f; // Time delay before the next wave starts

    [Header("References")]
    [SerializeField] private Transform startPoint; // The very first waypoint where enemies appear
    [SerializeField] private Transform[] waypoints; // The path the enemies follow (passed to them)

    [Header("UI (Optional)")]
    [SerializeField] private Text waveCountdownText; // Assign a UI Text element if you want countdown display
    [SerializeField] private Text wavesRemainingText; // Assign a UI Text element for wave count display

    // Internal State
    private int currentWaveIndex = 0;
    private float waveCountdown;
    private bool isSpawningWave = false;

    void Start()
    {
        Debug.Log(waves.Length + " waves configured for the Spawner!");
        Debug.Log("Time between waves: " + timeBetweenWaves + " seconds");
        // Print the waves details to the console
        foreach (var wave in waves)
        {
            Debug.Log($"Wave: {wave.enemyPrefab.name}, Count: {wave.count}, Interval: {wave.spawnInterval} seconds");
        }
        // Validate references
        if (startPoint == null || waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Spawner references (Start Point or Waypoints) are not set!");
            enabled = false; // Disable spawner if references are missing
            return;
        }
        if (waves == null || waves.Length == 0)
        {
             Debug.LogError("No waves configured for the Spawner!");
             enabled = false;
             return;
        }

        waveCountdown = timeBetweenWaves; // Initial countdown before the first wave
        UpdateWavesRemainingText(); // Initial UI update
    }

    void Update()
    {
        // Don't countdown if currently spawning or if all waves are done
        if (isSpawningWave || currentWaveIndex >= waves.Length)
        {
            // Optional: You could add logic here for what happens after all waves (e.g., "You Win!")
            if (waveCountdownText != null) waveCountdownText.text = "DONE"; // Update UI if all waves complete
            return;
        }

        // Countdown timer logic
        if (waveCountdown <= 0f)
        {
            // Time to start the next wave
            StartCoroutine(SpawnWave());
            waveCountdown = timeBetweenWaves; // Reset countdown for *after* this wave finishes
        }
        else
        {
            // Tick down the timer
            waveCountdown -= Time.deltaTime;
            waveCountdown = Mathf.Clamp(waveCountdown, 0f, float.MaxValue); // Prevent negative countdown
            UpdateWaveCountdownText(); // Update UI
        }

        // Print out how many minions are currently active in the scene
        // int activeMinions = FindObjectsByType<MinionMovement>(FindObjectsSortMode.None).Length;
        // Debug.Log("Active Minions: " + activeMinions);
    }

    IEnumerator SpawnWave()
    {
        isSpawningWave = true; // Flag that we are busy spawning

        Wave wave = waves[currentWaveIndex]; // Get the data for the current wave

        Debug.Log("Starting Wave " + (currentWaveIndex + 1));
        UpdateWavesRemainingText(); // Update UI

        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            // Wait for the specified interval before spawning the next enemy in the wave
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        currentWaveIndex++; // Move to the next wave index
        isSpawningWave = false; // Finished spawning this wave
         // Note: The countdown for the *next* wave was already reset in Update()
    }

    void SpawnEnemy(GameObject enemyPrefabToSpawn)
    {
        if (enemyPrefabToSpawn == null)
        {
            Debug.LogError("Enemy prefab is null for the current wave!");
            return;
        }

        // Enable the enemy prefab if it was disabled in the editor
        enemyPrefabToSpawn.SetActive(true);
        // Instantiate the enemy at the designated start point
        GameObject spawnedEnemy = Instantiate(enemyPrefabToSpawn, startPoint.position, startPoint.rotation);

        // Get the movement script from the spawned enemy
        MinionMovement minionMovement = spawnedEnemy.GetComponent<MinionMovement>();

        if (minionMovement != null)
        {
            // Assign the path (waypoints) to the newly spawned minion
            minionMovement.SetWaypoints(waypoints);
        }
        else
        {
            Debug.LogWarning("Spawned enemy prefab (" + enemyPrefabToSpawn.name + ") is missing MinionMovement script!");
        }
    }

    // --- Optional UI Update Functions ---
    void UpdateWaveCountdownText()
    {
        if (waveCountdownText != null)
        {
            waveCountdownText.text = $"Next Wave: {waveCountdown:00.0}"; // Format to one decimal place
        }
    }

     void UpdateWavesRemainingText()
    {
        if (wavesRemainingText != null)
        {
            int wavesLeft = waves.Length - currentWaveIndex;
            wavesRemainingText.text = $"Waves Left: {wavesLeft}";
        }
    }
}