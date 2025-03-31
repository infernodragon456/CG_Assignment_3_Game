using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [Header("Unit References")]
    private UnitData unitData;
    private NavMeshAgent agent;
    private Animator animator;
    
    [Header("Status")]
    public bool isPlayerUnit;
    private float currentHealth;
    private Unit targetUnit;
    private bool isBattleStarted = false;
    private bool isAttacking = false;
    private float attackCooldown = 0f;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }
    
    public void Initialize(UnitData data, bool isPlayer)
    {
        this.unitData = data;
        this.isPlayerUnit = isPlayer;
        this.currentHealth = data.health;
        
        // Set NavMeshAgent properties
        agent.speed = data.moveSpeed;
        agent.stoppingDistance = data.attackRange * 0.8f;
        
        // Set team color/material
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                m.color = isPlayerUnit ? Color.blue : Color.red;
            }
        }
        
        // Disable NavMeshAgent until battle starts
        agent.isStopped = true;
    }
    
    public void StartBattle()
    {
        isBattleStarted = true;
        agent.isStopped = false;
        StartCoroutine(FindTargetRoutine());
    }
    
    IEnumerator FindTargetRoutine()
    {
        while (isBattleStarted && currentHealth > 0)
        {
            FindClosestTarget();
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    void FindClosestTarget()
    {
        // Find all enemy units
        Unit[] allUnits = FindObjectsOfType<Unit>();
        Unit closestUnit = null;
        float closestDistance = float.MaxValue;
        
        foreach (Unit unit in allUnits)
        {
            if (unit.isPlayerUnit != this.isPlayerUnit && unit.currentHealth > 0)
            {
                float distance = Vector3.Distance(transform.position, unit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestUnit = unit;
                }
            }
        }
        
        targetUnit = closestUnit;
    }
    
    void Update()
    {
        if (!isBattleStarted || currentHealth <= 0) return;
        
        // Update attack cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        
        // If we have a target, move towards it and attack when in range
        if (targetUnit != null && targetUnit.currentHealth > 0)
        {
            // Move towards target
            agent.SetDestination(targetUnit.transform.position);
            
            // Check if in attack range
            float distanceToTarget = Vector3.Distance(transform.position, targetUnit.transform.position);
            if (distanceToTarget <= unitData.attackRange)
            {
                // Stop moving
                agent.isStopped = true;
                
                // Face the target
                Vector3 lookDirection = (targetUnit.transform.position - transform.position).normalized;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDirection);
                }
                
                // Attack if cooldown is done
                if (attackCooldown <= 0 && !isAttacking)
                {
                    StartCoroutine(AttackRoutine());
                }
            }
            else
            {
                // Continue moving
                agent.isStopped = false;
            }
        }
        else
        {
            // No target or target is dead, find a new one
            FindClosestTarget();
            
            // If still no target, wander around
            if (targetUnit == null)
            {
                if (!agent.hasPath)
                {
                    Vector3 randomPoint = transform.position + Random.insideUnitSphere * 5f;
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }
                }
            }
        }
    }
    
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        
        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Wait a bit for the animation to play
        yield return new WaitForSeconds(0.5f);
        
        // Deal damage if the target is still alive
        if (targetUnit != null && targetUnit.currentHealth > 0)
        {
            targetUnit.TakeDamage(unitData.damage);
        }
        
        // Set attack cooldown
        attackCooldown = 1f / unitData.attackSpeed;
        
        isAttacking = false;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // Play hurt animation
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        // Disable components
        agent.isStopped = true;
        agent.enabled = false;
        
        // Remove colliders
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
        
        // Disable the unit after a short delay (to show death animation)
        StartCoroutine(DisableAfterDelay(2f));
    }
    
    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
} 