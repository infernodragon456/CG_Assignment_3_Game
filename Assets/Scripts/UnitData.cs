using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "TABS Game/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Unit Info")]
    public string unitName = "New Unit";
    public string description;
    public Sprite icon;
    public GameObject unitPrefab;
    
    [Header("Stats")]
    public int cost = 100;
    public float health = 100f;
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackSpeed = 1f; // Attacks per second
    public float moveSpeed = 3f;
} 