using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("UI References")]
    public GameObject unitSelectionPanel;
    public GameObject gameOverPanel;
    public Button startBattleButton;
    public Transform unitButtonsContainer;
    public GameObject unitButtonPrefab;
    
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
        // Set initial UI state
        if (unitSelectionPanel != null)
        {
            unitSelectionPanel.SetActive(true);
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Setup unit selection buttons
        SetupUnitButtons();
        
        // Assign start battle button event
        if (startBattleButton != null)
        {
            startBattleButton.onClick.AddListener(() => {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.StartBattle();
                }
            });
        }
    }
    
    void SetupUnitButtons()
    {
        if (GameManager.Instance == null || unitButtonsContainer == null || unitButtonPrefab == null)
            return;
        
        // Clear any existing buttons
        foreach (Transform child in unitButtonsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create a button for each available unit
        foreach (UnitData unitData in GameManager.Instance.availableUnits)
        {
            GameObject buttonObj = Instantiate(unitButtonPrefab, unitButtonsContainer);
            Button button = buttonObj.GetComponent<Button>();
            
            // Set button text
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = unitData.unitName + " (" + unitData.cost + ")";
            }
            
            // Set button icon
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null && unitData.icon != null)
            {
                buttonImage.sprite = unitData.icon;
            }
            
            // Set button tooltip
            TooltipTrigger tooltipTrigger = buttonObj.GetComponent<TooltipTrigger>();
            if (tooltipTrigger != null)
            {
                tooltipTrigger.tooltipText = unitData.description;
            }
            
            // Add click event to select this unit
            UnitData capturedUnitData = unitData; // Capture variable for closure
            button.onClick.AddListener(() => {
                SelectUnit(capturedUnitData);
            });
        }
    }
    
    void SelectUnit(UnitData unitData)
    {
        if (PlacementManager.Instance != null)
        {
            PlacementManager.Instance.StartPlacement(unitData);
        }
    }
    
    public void ShowGameOver(bool playerWon)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Set appropriate message
            Text gameOverText = gameOverPanel.GetComponentInChildren<Text>();
            if (gameOverText != null)
            {
                gameOverText.text = playerWon ? "Victory!" : "Defeat!";
            }
        }
    }
    
    public void OnRestartButtonClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
} 