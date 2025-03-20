using UnityEngine;

public abstract class RagdollController : MonoBehaviour
{
    [Header("References")]
    public RagdollManager ragdollManager;
    public Transform hips;
    public CharacterSettings settings;
    public Transform leftFist;
    public Transform rightFist;
    
    protected Rigidbody hipsRb;
    protected float lastAttackTime;
    
    protected virtual void Start()
    {
        // Get references if not assigned
        if (ragdollManager == null)
            ragdollManager = GetComponent<RagdollManager>();
            
        if (hips == null)
            hips = transform.Find("Hips");
            
        hipsRb = hips?.GetComponent<Rigidbody>();
        
        // Activate ragdoll physics initially
        if (ragdollManager != null)
            ragdollManager.isActive = true;

        leftFist.gameObject.AddComponent<HitDetector>().settings = settings;
        rightFist.gameObject.AddComponent<HitDetector>().settings = settings;
    }
    
    // Movement methods
    protected void ApplyMovement(Vector3 direction, float force)
    {
        if (hipsRb != null)
            hipsRb.AddForce(direction * force);
    }
    
    protected void ApplyTurn(float amount)
    {
        if (hipsRb != null)
            hipsRb.AddTorque(Vector3.up * amount * settings.turnTorque);
    }
    
    // Attack methods
    protected void LeftPunch()
    {
        if (leftFist != null && leftFist.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(GetPunchDirection(leftFist) * settings.punchForce, ForceMode.Impulse);
            lastAttackTime = Time.time;
        }
    }
    
    protected void RightPunch()
    {
        if (rightFist != null && rightFist.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(GetPunchDirection(rightFist) * settings.punchForce, ForceMode.Impulse);
            lastAttackTime = Time.time;
        }
    }
    
    // Can be overridden for different targeting
    protected virtual Vector3 GetPunchDirection(Transform fist)
    {
        return hips != null ? hips.forward : transform.forward;
    }
}