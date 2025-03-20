using Unity.VisualScripting;
using UnityEngine;

public class SimpleRagdollAI : RagdollController
{
    [Header("AI Settings")]
    public string targetTag = "Player";
    private Transform currentTarget;
    
    protected override void Start()
    {
        base.Start();
        
        // Find target
        if (!string.IsNullOrEmpty(targetTag))
        {
            GameObject target = GameObject.FindGameObjectWithTag(targetTag);
            if (target != null)
            {
                currentTarget = target.transform;
            }
        }
    }
    
    void Update()
    {
        if (!ragdollManager.isActive || currentTarget == null)
            return;
            
        // Simple attack decision
        if (Time.time > lastAttackTime + settings.attackCooldown && 
            Random.value < settings.attackProbability)
        {
            // Choose random punch
            if (Random.value < 0.5f)
                LeftPunch();
            else
                RightPunch();
        }
    }
    
    void FixedUpdate()
    {
        if (!ragdollManager.isActive || currentTarget == null)
            return;
            
        // Get direction to target
        Vector3 directionToTarget = currentTarget.position - hips.position;
        float distanceToTarget = directionToTarget.magnitude;
        directionToTarget.y = 0; // Keep on horizontal plane
        directionToTarget.Normalize();
        
        // Move toward target if too far, away if too close
        if (distanceToTarget > settings.optimalDistance * 1.2f)
        {
            // Move toward target
            ApplyMovement(directionToTarget, settings.moveForce);
        }
        else if (distanceToTarget < settings.optimalDistance * 0.8f)
        {
            // Move away from target
            ApplyMovement(-directionToTarget, settings.moveForce * 0.5f);
        }
        
        // Turn to face target
        Vector3 targetDirection = new Vector3(
            currentTarget.position.x - hips.position.x,
            0,
            currentTarget.position.z - hips.position.z
        ).normalized;
        
        float turnDirection = Vector3.Cross(hips.forward, targetDirection).y;
        ApplyTurn(turnDirection);
    }
    
    // Override to target the enemy
    protected override Vector3 GetPunchDirection(Transform fist)
    {
        if (currentTarget != null)
        {
            return (currentTarget.position - fist.position).normalized;
        }
        return base.GetPunchDirection(fist);
    }
    
    // Public method to set target
    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
}