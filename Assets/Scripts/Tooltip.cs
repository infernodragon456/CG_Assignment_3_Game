using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [Header("UI References")]
    public Text tooltipText;
    public LayoutElement layoutElement;
    public int characterWrapLimit = 40;
    public RectTransform rectTransform;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void SetText(string text)
    {
        if (tooltipText != null)
        {
            tooltipText.text = text;
            
            int headerLength = text.Length;
            
            if (layoutElement != null)
            {
                layoutElement.enabled = headerLength > characterWrapLimit;
            }
        }
    }
    
    public void SetPosition(Vector2 position)
    {
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        
        // Adjust pivots to keep tooltip on screen
        pivotX = Mathf.Clamp(pivotX, 0.1f, 0.9f);
        pivotY = Mathf.Clamp(pivotY, 0.1f, 0.9f);
        
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
} 