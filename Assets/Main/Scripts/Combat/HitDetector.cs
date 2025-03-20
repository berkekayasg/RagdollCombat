using UnityEngine;

public class HitDetector : MonoBehaviour
{
    public float hitForce = 10f;
    public float damageMultiplier = 1f;
    public bool isWeapon = false;
    
    private Rigidbody rb;
    private float impactThreshold = 3f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if impact is strong enough
        if (collision.relativeVelocity.magnitude < impactThreshold)
            return;
            
        // Check if we hit another ragdoll
        HitReceiver hitReceiver = collision.gameObject.GetComponent<HitReceiver>();
        if (hitReceiver != null)
        {
            // Calculate damage based on velocity and mass
            float damage = collision.relativeVelocity.magnitude * rb.mass * damageMultiplier;
            
            // Apply force to the hit body part
            Vector3 hitDirection = collision.contacts[0].point - transform.position;
            hitReceiver.ReceiveHit(hitDirection.normalized, damage, hitForce);
            
            // Visual feedback could be added here
            Debug.Log($"Hit {collision.gameObject.name} for {damage} damage");
        }
    }
}