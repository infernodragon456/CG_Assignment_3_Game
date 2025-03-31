using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAreaVisualizer : MonoBehaviour
{
    public enum AreaType { Player, Enemy }
    
    [Header("Settings")]
    public AreaType areaType = AreaType.Player;
    public float alpha = 0.3f;
    
    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    private Material colorMaterial;
    
    private void Awake()
    {
        // Get the existing mesh renderer
        meshRenderer = GetComponent<MeshRenderer>();
        
        // If this object doesn't have a mesh renderer, try to find one in children
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            
            // If still not found, add one
            if (meshRenderer == null)
            {
                Debug.LogWarning($"No MeshRenderer found on {gameObject.name}. Make sure the spawn area has a visible mesh.");
                return;
            }
        }
        
        // Store the original material if needed later
        originalMaterial = meshRenderer.material;
        
        // Create a new transparent material
        colorMaterial = new Material(Shader.Find("Standard"));
        colorMaterial.SetFloat("_Mode", 3); // Set transparent mode
        colorMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        colorMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        colorMaterial.SetInt("_ZWrite", 0);
        colorMaterial.DisableKeyword("_ALPHATEST_ON");
        colorMaterial.EnableKeyword("_ALPHABLEND_ON");
        colorMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        colorMaterial.renderQueue = 3000;
        
        // Set color based on area type
        Color areaColor = (areaType == AreaType.Player) ? 
            new Color(0, 1, 0, alpha) :  // Green for player
            new Color(1, 0, 0, alpha);   // Red for enemy
            
        colorMaterial.color = areaColor;
        
        // Apply the material
        meshRenderer.material = colorMaterial;
    }
    
    private void OnDestroy()
    {
        // If we have a mesh renderer and the original material, restore it
        if (meshRenderer != null && originalMaterial != null)
        {
            meshRenderer.material = originalMaterial;
        }
    }
} 