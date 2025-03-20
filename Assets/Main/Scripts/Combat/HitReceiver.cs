using UnityEngine;

public class HitReceiver : MonoBehaviour
{
    public RagdollManager ragdollManager;
    public float health = 100f;
    public float damageMultiplier = 1f;
    
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Find the ragdoll manager if not assigned
        if (ragdollManager == null)
        {
            ragdollManager = GetComponentInParent<RagdollManager>();
        }
    }
    
    public void ReceiveHit(Vector3 direction, float damage, float force)
    {
        // Apply damage
        health -= damage * damageMultiplier;
        
        // Apply force to this body part
        rb.AddForce(direction * force, ForceMode.Impulse);
        
        // Notify the ragdoll manager about the hit
        if (ragdollManager != null)
        {
            ragdollManager.ApplyHit(direction, force);
        }
        
        // Check if health is depleted
        if (health <= 0)
        {
            // Handle character defeat/death
            Debug.Log($"{gameObject.name} has been defeated!");
        }
    }
}