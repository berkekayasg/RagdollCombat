using UnityEngine;

public class SimpleRagdollAI : MonoBehaviour
{
    public RagdollManager ragdollManager;
    public Transform hips;
    public CharacterSettings settings;
    public string targetTag = "Player";
    
    public Transform leftFist;
    public Transform rightFist;
    
    private Transform currentTarget;
    private float lastAttackTime;
    private Rigidbody hipsRb;
    
    void Start()
    {
        if (ragdollManager == null)
            ragdollManager = GetComponent<RagdollManager>();
            
        hipsRb = hips.GetComponent<Rigidbody>();
        
        // Activate ragdoll physics initially
        ragdollManager.isActive = true;
        
        // Find target if tag is set
        if (!string.IsNullOrEmpty(targetTag))
        {
            GameObject target = GameObject.FindGameObjectWithTag(targetTag);
            if (target != null)
            {
                currentTarget = target.transform.Find("Hips");
            }
        }
    }
    
    void Update()
    {
        if (!ragdollManager.isActive || currentTarget == null)
            return;
            
        // Simple attack decision
        if (Time.time > lastAttackTime + settings.attackCooldown && Random.value < settings.attackProbability)
        {
            // Choose random punch attack
            if (Random.value < 0.5f)
            {
                LeftPunch();
            }
            else
            {
                RightPunch();
            }
            
            lastAttackTime = Time.time;
        }
    }
    
    void FixedUpdate()
    {
        if (!ragdollManager.isActive || currentTarget == null)
            return;
            
        // Get direction to target
        Vector3 directionToTarget = currentTarget.position - hips.position;
        float distanceToTarget = directionToTarget.magnitude;
        directionToTarget.Normalize();
        directionToTarget.y = 0; // Keep on horizontal plane
        
        // Move toward target if too far, away if too close
        if (distanceToTarget > settings.optimalDistance * 1.2f)
        {
            // Move toward target
            hipsRb.AddForce(directionToTarget * settings.moveForce);
        }
        else if (distanceToTarget < settings.optimalDistance * 0.8f)
        {
            // Move away from target
            hipsRb.AddForce(-directionToTarget * settings.moveForce * 0.5f);
        }
        
        // Turn to face target
        Vector3 targetXZ = new Vector3(currentTarget.position.x, hips.position.y, currentTarget.position.z);
        Vector3 directionToFace = (targetXZ - hips.position).normalized;
        
        // Calculate current forward direction in XZ plane
        Vector3 currentForward = new Vector3(hips.forward.x, 0, hips.forward.z).normalized;
        
        // Cross product to determine turn direction
        float turnDirection = Vector3.Cross(currentForward, directionToFace).y;
        
        // Apply turning torque
        hipsRb.AddTorque(Vector3.up * turnDirection * settings.turnTorque);
        
        // Random jumps
        if (Random.value < 0.005f) // 0.5% chance per physics frame
        {
            hipsRb.AddForce(Vector3.up * settings.jumpForce);
        }
    }
    
    void LeftPunch()
    {
        if (leftFist != null && leftFist.GetComponent<Rigidbody>() != null)
        {
            Vector3 direction = GetAttackDirection(leftFist);
            leftFist.GetComponent<Rigidbody>().AddForce(direction * settings.punchForce, ForceMode.Impulse);
        }
    }
    
    void RightPunch()
    {
        if (rightFist != null && rightFist.GetComponent<Rigidbody>() != null)
        {
            Vector3 direction = GetAttackDirection(rightFist);
            rightFist.GetComponent<Rigidbody>().AddForce(direction * settings.punchForce, ForceMode.Impulse);
        }
    }
    
    // Kick attacks removed as they don't work well with IK system
    
    // Helper to get direction to target or forward if no target
    Vector3 GetAttackDirection(Transform attackLimb)
    {
        if (currentTarget != null)
        {
            return (currentTarget.position - attackLimb.position).normalized;
        }
        else
        {
            return hips.forward;
        }
    }
    
    // Optional function to manually set target
    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
}
