using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Setup")]
    public int playerPoints = 1000;
    public Transform playerSpawnArea;
    public Transform enemySpawnArea;
    
    [Header("UI References")]
    public TextMeshProUGUI pointsDisplay;
    public GameObject unitSelectionPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    [Header("Unit Prefabs")]
    public List<UnitData> availableUnits = new List<UnitData>();
    
    private List<GameObject> playerUnits = new List<GameObject>();
    private List<GameObject> enemyUnits = new List<GameObject>();
    private bool isPlacementPhase = true;
    private bool isGameOver = false;

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

    private void Start()
    {
        UpdatePointsDisplay();
        SetupEnemyUnits();
    }

    private void Update()
    {
        if (isPlacementPhase && Input.GetKeyDown(KeyCode.Space))
        {
            StartBattle();
        }

        CheckGameState();
    }

    public void SpawnUnit(UnitData unitData, Vector3 position, bool isPlayerUnit)
    {
        // Check if unitData or unitPrefab is null
        if (unitData == null || unitData.unitPrefab == null)
        {
            Debug.LogError("Cannot spawn unit: Unit data or prefab is null!");
            return;
        }

        if (isPlayerUnit)
        {
            if (playerPoints >= unitData.cost)
            {
                GameObject unit = Instantiate(unitData.unitPrefab, position, Quaternion.identity);
                Unit unitComponent = unit.GetComponent<Unit>();
                unitComponent.Initialize(unitData, isPlayerUnit);
                playerUnits.Add(unit);
                
                playerPoints -= unitData.cost;
                UpdatePointsDisplay();
            }
            else
            {
                Debug.Log("Not enough points to spawn this unit!");
            }
        }
        else
        {
            GameObject unit = Instantiate(unitData.unitPrefab, position, Quaternion.identity);
            Unit unitComponent = unit.GetComponent<Unit>();
            unitComponent.Initialize(unitData, isPlayerUnit);
            enemyUnits.Add(unit);
        }
    }

    private void UpdatePointsDisplay()
    {
        if (pointsDisplay != null)
        {
            pointsDisplay.text = "Points: " + playerPoints.ToString();
        }
    }

    private void SetupEnemyUnits()
    {
        // This will be customized based on the level design
        // For now, we'll add some basic enemy units

        // Example of spawning enemy units
        if (enemySpawnArea == null)
        {
            Debug.LogError("Enemy spawn area is not set!");
            return;
        }

        if (availableUnits == null || availableUnits.Count == 0)
        {
            Debug.LogError("No available units to spawn! Make sure to assign units in the inspector.");
            return;
        }

        // Randomly choose units and positions within the enemy spawn area
        int enemyBudget = playerPoints + 200; // Give the AI a slight advantage
        
        while (enemyBudget > 0 && availableUnits.Count > 0)
        {
            UnitData randomUnit = availableUnits[Random.Range(0, availableUnits.Count)];
            
            // Skip null units or units with null prefabs
            if (randomUnit == null || randomUnit.unitPrefab == null)
            {
                Debug.LogWarning("Found a null unit or unit prefab in availableUnits list. Skipping.");
                continue;
            }
            
            if (randomUnit.cost <= enemyBudget)
            {
                Vector3 spawnPos = enemySpawnArea.position + new Vector3(
                    Random.Range(-enemySpawnArea.localScale.x/2, enemySpawnArea.localScale.x/2),
                    0,
                    Random.Range(-enemySpawnArea.localScale.z/2, enemySpawnArea.localScale.z/2)
                );
                
                SpawnUnit(randomUnit, spawnPos, false);
                enemyBudget -= randomUnit.cost;
            }
            else
            {
                // If we can't afford any units, break out of the loop
                bool canAffordAny = false;
                foreach (UnitData unit in availableUnits)
                {
                    if (unit.cost <= enemyBudget)
                    {
                        canAffordAny = true;
                        break;
                    }
                }
                
                if (!canAffordAny) break;
            }
        }
    }

    public void StartBattle()
    {
        isPlacementPhase = false;
        unitSelectionPanel.SetActive(false);
        
        // Disable spawn area mesh renderers
        DisableSpawnAreaRenderers();
        
        // Activate all units' AI
        foreach (GameObject unit in playerUnits)
        {
            unit.GetComponent<Unit>().StartBattle();
        }
        
        foreach (GameObject unit in enemyUnits)
        {
            unit.GetComponent<Unit>().StartBattle();
        }
    }
    
    private void DisableSpawnAreaRenderers()
    {
        // Disable player spawn area renderer
        if (playerSpawnArea != null)
        {
            MeshRenderer renderer = playerSpawnArea.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }
        
        // Disable enemy spawn area renderer
        if (enemySpawnArea != null)
        {
            MeshRenderer renderer = enemySpawnArea.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }
    }

    private void CheckGameState()
    {
        if (isGameOver || isPlacementPhase) return;
        
        // Check if all player units are defeated
        bool allPlayerUnitsDead = true;
        foreach (GameObject unit in playerUnits)
        {
            if (unit != null && unit.activeSelf)
            {
                allPlayerUnitsDead = false;
                break;
            }
        }
        
        // Check if all enemy units are defeated
        bool allEnemyUnitsDead = true;
        foreach (GameObject unit in enemyUnits)
        {
            if (unit != null && unit.activeSelf)
            {
                allEnemyUnitsDead = false;
                break;
            }
        }
        
        if (allPlayerUnitsDead || allEnemyUnitsDead)
        {
            EndGame(allEnemyUnitsDead);
        }
    }

    private void EndGame(bool playerWon)
    {
        isGameOver = true;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverText != null)
            {
                gameOverText.text = playerWon ? "Victory!" : "Defeat!";
            }
        }
        
        Debug.Log(playerWon ? "Player won!" : "Player lost!");
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
} 