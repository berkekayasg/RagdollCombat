using UnityEngine;

public class HitDetector : MonoBehaviour
{
    [HideInInspector] public CharacterSettings settings;


    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Ignore weak impacts
        if (rb == null || collision.relativeVelocity.magnitude < settings.impactThreshold)
            return;
        
        // Check if we hit ourselves
        if (collision.transform.root.gameObject == transform.root.gameObject)
            return;
            
        // Check if we hit a valid target
        RagdollManager hitReceiver = collision.gameObject.GetComponentInParent<RagdollManager>();
        if (hitReceiver != null)
        {
            // Calculate damage based on impact
            float damage = collision.relativeVelocity.magnitude * rb.mass * settings.damageMultiplier;
            
            // Get hit direction
            Vector3 hitDirection = collision.contacts[0].point - transform.position;
            hitDirection.Normalize();

            hitReceiver.ApplyHit(hitDirection, damage);
        }
    }
}