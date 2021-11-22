using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Author: Darcy Matheson
// Purpose: Responsible for spawning and tracking enemies and kills, as well as the spawn budget

public class WaveManager : MonoBehaviour
{
    #region Variables

    #region Internal

    public static float waveProgress;
    public static int waveNumber;
    public static bool gameStarted { get; private set; }
    [HideInInspector]
    public bool waveActive;
    public static int eliminatedWaveEnemies;
    public static int waveEnemyCount { get; private set; }
    private int currentWaveCost;
    private float intermissionStartTime;
    private float waveStartTime;

    private List<Transform> validSpawnPoints;
    [HideInInspector]
    public static List<GameObject> waveEnemies;
    private List<GameObject> upgradeStationInstances;
    private List<int> upgradeStationSpawnOrder;
    private int lastSpawnLocation;

    #endregion

    #region Parameters

    #region Setup
    [Header("Setup")]

    public GameObject[] enemyPrefabs;
    public LayerMask spawnerLayer;
    public LayerMask environmentLayers;
    public LayerMask enemyLayer;

    #endregion

    #region Configuration
    [Header("Configuration")]

    public int minSpawnDistance;
    public int maxSpawnDistance;
    public int timeBetweenWaves;
    public float budgetInstancePercentAllowed;
    public int waveSpawnBudget;
    public int spawnBudgetIncreasePerWave;

    #endregion

    #endregion

    #region Components
    [Header("Components")]

    public Transform enemyParent;
    private Transform playerTransform;
    private Transform mainCameraTransform;
    private PlayerStats playerStats;

    #endregion

    #endregion

    private void Awake()
    {
        // Static assignment
        waveProgress = 0f;
        waveNumber = 0;
        gameStarted = false;
        eliminatedWaveEnemies = 0;
        waveEnemyCount = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        #region Initialisation

        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        mainCameraTransform = Camera.main.GetComponent<Transform>();

        upgradeStationInstances = new List<GameObject>();
        upgradeStationSpawnOrder = new List<int>();

        validSpawnPoints = new List<Transform>();
        waveEnemies = new List<GameObject>();

        // Find all upgrade stations in the map
        lastSpawnLocation = -1;
        GameObject[] upgradeStations = GameObject.FindGameObjectsWithTag("UpgradeStation");
        foreach (GameObject station in upgradeStations)
        {
            upgradeStationInstances.Add(station);
        }

        waveActive = false;

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            if (waveActive)
            {
                UpdateSpawnPoints();

                #region Constrict List

                // Shrink list to fit all enemies that are currently alive
                for (int i = 0; i < waveEnemies.Count; i++)
                {
                    if (waveEnemies[i] == null)
                    {
                        waveEnemies.RemoveAt(i);
                        i -= 1;
                    }
                }

                #endregion

                #region Spawn Enemies

                // If the wave is currently occurring and there is a valid place to spawn an enemy
                if (waveEnemies.Count > 0 && validSpawnPoints.Count > 0)
                {
                    // Determine cost of current enemies spawned in
                    int currentActiveEnemyCost = 0;
                    int remainingEnemies = 0;
                    for (int i = 0; i < waveEnemies.Count; i++)
                    {
                        // If the enemy is a single banana
                        if (waveEnemies[i].GetComponent<BananaSingleEnemy>() != null && waveEnemies[i].activeSelf == false)
                        {
                            // Spawn the enemy
                            if (NavMesh.SamplePosition(validSpawnPoints[Random.Range(0, validSpawnPoints.Count)].position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                            {
                                waveEnemies[i].transform.position = hit.position;
                                waveEnemies[i].GetComponent<Enemy>().isBurrowing = false;
                                waveEnemies[i].SetActive(true);
                            }

                            break;
                        }

                        if (waveEnemies[i].activeInHierarchy)
                        {
                            currentActiveEnemyCost += waveEnemies[i].GetComponent<Enemy>().spawnCost;
                        }
                        else
                        {
                            remainingEnemies += 1;
                        }
                    }

                    // Spawn more enemies if there isn't enough
                    if ((float)currentActiveEnemyCost / (float)currentWaveCost < budgetInstancePercentAllowed && remainingEnemies > 0)
                    {
                        SpawnEnemy();
                    }
                }

                #endregion
            }

            // If all enemies in the wave have been eliminated
            if (eliminatedWaveEnemies >= waveEnemyCount && waveActive)
            {
                waveActive = false;
                WaveEnded();
            }

            // If the intermission period has ended
            if (waveActive == false && playerStats.currentRunTime >= waveStartTime)
            {
                NewWave();
            }
        }

        #region Update UI

        waveProgress = (float)eliminatedWaveEnemies / (float)waveEnemyCount;
        waveProgress = Mathf.Clamp01(waveProgress);
        UserInterfaceHUD.waveProgress = Mathf.Floor(Mathf.Clamp01(waveProgress) * 1000f) / 1000f;

        waveNumber = Mathf.Max(1, waveNumber);
        UserInterfaceHUD.waveNumber = waveNumber;

        if (playerStats.currentRunTime < waveStartTime)
        {
            UserInterfaceHUD.intermissionProgress = Mathf.Clamp01((playerStats.currentRunTime - intermissionStartTime) / (waveStartTime - intermissionStartTime));
        }
        else
        {
            UserInterfaceHUD.intermissionProgress = -1f;
        }

        UserInterfaceHUD.intermissionDuration = timeBetweenWaves;

        #endregion
    }

    // Updates the list of currently available spawn points at this position and rotation relative to the player
    void UpdateSpawnPoints()
    {
        validSpawnPoints.Clear();

        // Cast maximum sphere
        Collider[] spawnPointColliders = Physics.OverlapSphere(playerTransform.position, maxSpawnDistance, spawnerLayer);

        // Add valid spawners to list
        for (int i = 0; i < spawnPointColliders.Length; i++)
        {
            #region Check Spawner Validity

            Vector3 spawnerPosition = spawnPointColliders[i].transform.position;

            // If the spawn point is outside the minimum range
            if (Vector3.Distance(spawnerPosition, playerTransform.position) > minSpawnDistance)
            {
                // If the spawn point is outside the player's vision
                Vector3 cameraToSpawner = spawnerPosition - mainCameraTransform.position;
                if (Vector3.Dot(cameraToSpawner.normalized, mainCameraTransform.forward) < 0f || Physics.Linecast(mainCameraTransform.position, spawnerPosition, environmentLayers))
                {
                    // If there is no enemy at this point currently
                    if (Physics.OverlapSphere(spawnerPosition, 3f, enemyLayer).Length <= 0)
                    {
                        // Add the spawn point to the list
                        validSpawnPoints.Add(spawnPointColliders[i].transform);
                    }
                }
            }

            #endregion
        }
    }

    // Selects the enemies for the upcoming wave given a spawn budget
    void ChooseWaveEnemies()
    {
        int bananaCount = enemyPrefabs[0].GetComponent<BananaBunchEnemy>().bananaCount;

        currentWaveCost = 0;
        waveEnemyCount = 0;
        eliminatedWaveEnemies = 0;
        waveEnemies.Clear();

        // Until the limit of this wave has been reached
        while (currentWaveCost < waveSpawnBudget)
        {
            // Add random enemy to distributed spawn list and update budget
            int enemyIndex = Random.Range(0, 5);

            #region Enemy Choice Distribution

            // Convert weighted randomness to flat keyed indices (0 - banana, 1 - strawberry, 2 - onion)
            switch (enemyIndex)
            {
                case 0:
                    enemyIndex = 0;
                    break;
                case 1:
                    enemyIndex = 0;
                    break;
                case 2:
                    enemyIndex = 1;
                    break;
                case 3:
                    enemyIndex = 1;
                    break;
                case 4:
                    enemyIndex = 2;
                    break;
            }

            #endregion

            // Sum all enemies in this wave
            waveEnemyCount += (enemyIndex == 0) ? bananaCount + 1 : 1;
            currentWaveCost += enemyPrefabs[enemyIndex].GetComponent<Enemy>().spawnCost;

            // Spawn enemy, disable it and add to the list
            GameObject enemyInstance = Instantiate<GameObject>(enemyPrefabs[enemyIndex], new Vector3(90f, 6f, 88f), Quaternion.identity, enemyParent);
            waveEnemies.Add(enemyInstance);
            enemyInstance.SetActive(false);
        }
    }

    // Enables and positions an enemy if there is one available
    void SpawnEnemy()
    {
        // If there are enemies available to spawn in this wave
        if (waveEnemies.Count > 0)
        {
            int spawnPoint = Random.Range(0, validSpawnPoints.Count);

            for (int i = 0; i < waveEnemies.Count; i++)
            {
                // If the enemy hasn't been spawned in yet
                if (waveEnemies[i].activeInHierarchy == false)
                {
                    // Spawn the enemy
                    if (NavMesh.SamplePosition(validSpawnPoints[spawnPoint].position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                    {
                        waveEnemies[i].transform.position = hit.position;
                        waveEnemies[i].GetComponent<Enemy>().isBurrowing = false;
                        waveEnemies[i].SetActive(true);
                    }

                    break;
                }
            }
        }
    }

    // Called when all enemies in a given wave have died
    void WaveEnded()
    {
        // Set intermission period
        intermissionStartTime = playerStats.currentRunTime;
        waveStartTime = playerStats.currentRunTime + timeBetweenWaves;

        // Set next wave budget
        waveSpawnBudget += spawnBudgetIncreasePerWave;
        playerStats.CalculateDifficultyLevel();

        #region Upgrade Station Spawns

        if (upgradeStationSpawnOrder.Count <= 0)
        {
            upgradeStationSpawnOrder.Clear();

            // Create spawn order
            List<int> tempOrderList = new List<int>();
            for (int i = 0; i < upgradeStationInstances.Count; i++)
            {
                tempOrderList.Add(i);
            }

            // Bag randomiser for spawn order, does not account for closed off areas due to barriers
            while (tempOrderList.Count > 0)
            {
                int index = Random.Range(0, tempOrderList.Count);
                upgradeStationSpawnOrder.Add(tempOrderList[index]);
                tempOrderList.RemoveAt(index);
            }

            // Prevent double ups between sequences
            if (upgradeStationSpawnOrder[0] == lastSpawnLocation)
            {
                upgradeStationSpawnOrder.Reverse();
            }
        }

        if (upgradeStationSpawnOrder.Count > 0)
        {
            // Turn upgrade station on
            upgradeStationInstances[upgradeStationSpawnOrder[0]].GetComponent<UpgradeStation>().isInteractable = true;
            lastSpawnLocation = upgradeStationSpawnOrder[0];
            upgradeStationSpawnOrder.RemoveAt(0);
        }

        #endregion
    }

    // Starts a new wave
    void NewWave()
    {
        // Turn all upgrade stations off
        for (int i = 0; i < upgradeStationInstances.Count; i++)
        {
            upgradeStationInstances[i].GetComponent<UpgradeStation>().isInteractable = false;
        }

        waveNumber += 1;
        waveActive = true;
        ChooseWaveEnemies();
    }

    // Starts the cycle of recurring waves
    public void StartWaveCycle()
    {
        waveNumber = 0;
        NewWave();
        gameStarted = true;
    }
}
