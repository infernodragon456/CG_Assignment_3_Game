using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }
    
    [Header("Placement Settings")]
    public LayerMask placementAreaMask;
    public GameObject placementGhost;
    public float placementHeight = 0.1f;
    public Color validPlacementColor = Color.green;
    public Color invalidPlacementColor = Color.red;
    
    private UnitData selectedUnit;
    private bool isPlacing = false;
    private Renderer ghostRenderer;
    private Material ghostMaterial;
    private bool isValidPlacement = false;
    
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
        
        if (placementGhost != null)
        {
            placementGhost.SetActive(false);
            ghostRenderer = placementGhost.GetComponentInChildren<Renderer>();
            if (ghostRenderer != null)
            {
                ghostMaterial = ghostRenderer.material;
            }
        }
    }
    
    void Update()
    {
        if (!isPlacing || GameManager.Instance == null) return;
        
        // Handle placement visualization
        UpdatePlacementGhost();
        
        // Handle click to place unit
        if (Input.GetMouseButtonDown(0) && isValidPlacement)
        {
            PlaceUnit();
        }
        
        // Handle cancellation
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }
    
    public void StartPlacement(UnitData unitToPlace)
    {
        if (GameManager.Instance == null || unitToPlace == null) return;
        
        selectedUnit = unitToPlace;
        isPlacing = true;
        
        if (placementGhost != null)
        {
            // Update ghost prefab to match selected unit
            placementGhost.SetActive(true);
            
            // You might want to update the ghost model to match the unit prefab
            // For simplicity, we'll just use a basic ghost for all units
        }
    }
    
    void UpdatePlacementGhost()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100f, placementAreaMask))
        {
            // Update ghost position
            Vector3 placementPosition = hit.point;
            placementPosition.y += placementHeight;
            placementGhost.transform.position = placementPosition;
            
            // Check if this is a valid placement spot (within player area)
            isValidPlacement = IsValidPlacementPosition(hit.point);
            
            // Update ghost color based on valid/invalid
            if (ghostMaterial != null)
            {
                ghostMaterial.color = isValidPlacement ? validPlacementColor : invalidPlacementColor;
            }
        }
    }
    
    bool IsValidPlacementPosition(Vector3 position)
    {
        // Check if the position is within the player spawn area
        if (GameManager.Instance.playerSpawnArea != null)
        {
            Bounds spawnBounds = new Bounds(
                GameManager.Instance.playerSpawnArea.position,
                GameManager.Instance.playerSpawnArea.localScale
            );
            
            // Adjust for Y height
            spawnBounds.extents = new Vector3(
                spawnBounds.extents.x,
                10f, // High value to allow placement anywhere in the Y axis
                spawnBounds.extents.z
            );
            
            return spawnBounds.Contains(position);
        }
        
        return false;
    }
    
    void PlaceUnit()
    {
        if (GameManager.Instance == null || selectedUnit == null) return;
        
        // Place the actual unit
        GameManager.Instance.SpawnUnit(selectedUnit, placementGhost.transform.position, true);
        
        // Reset placement state
        CancelPlacement();
    }
    
    public void CancelPlacement()
    {
        isPlacing = false;
        selectedUnit = null;
        
        if (placementGhost != null)
        {
            placementGhost.SetActive(false);
        }
    }
} 