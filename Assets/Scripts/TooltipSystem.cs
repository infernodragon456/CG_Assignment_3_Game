using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    
    public Tooltip tooltip;
    
    private void Awake()
    {
        current = this;
    }
    
    public static void Show(string content, Vector2 position)
    {
        if (current != null && current.tooltip != null)
        {
            current.tooltip.SetText(content);
            current.tooltip.SetPosition(position);
            current.tooltip.gameObject.SetActive(true);
        }
    }
    
    public static void Hide()
    {
        if (current != null && current.tooltip != null)
        {
            current.tooltip.gameObject.SetActive(false);
        }
    }
} 