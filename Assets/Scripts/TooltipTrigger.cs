using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipText;
    public float showDelay = 0.5f;
    
    private Coroutine showTooltipCoroutine;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
        }
        
        showTooltipCoroutine = StartCoroutine(ShowTooltipDelayed());
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
            showTooltipCoroutine = null;
        }
        
        TooltipSystem.Hide();
    }
    
    private IEnumerator ShowTooltipDelayed()
    {
        yield return new WaitForSeconds(showDelay);
        TooltipSystem.Show(tooltipText, Input.mousePosition);
    }
} 