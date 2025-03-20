using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    [Header("Character State")]
    [Range(0f, 1f)]
    public float consciousness = 1f;
    public bool isActive = false;
    
    [Range(0f, 1f)]
    public float fallAngle = 0.68f;
    
    // Events
    public System.Action OnFall;
    public System.Action OnHit;
    
    // Private references
    private Rigidbody[] bodyParts;
    private Transform hips;

    private void Start()
    {
        // Find references
        hips = transform.Find("Hips");
        bodyParts = GetComponentsInChildren<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        if (isActive && hips != null)
        {
            // Check if fallen over
            float upAlignment = Vector3.Dot(Vector3.up, hips.up);
            if (upAlignment < fallAngle)
            {
                Fall();
            }
        }
    }
    
    // Apply a hit force to the character
    public void ApplyHit(Vector3 hitPosition, float power)
    {
        if (hips == null || bodyParts == null) return;
        
        
        // Apply explosion force to body parts
        foreach (var rb in bodyParts)
        {
            rb.AddExplosionForce(power, hitPosition.normalized, 0.33f, 0f, ForceMode.Impulse);
        }
        
        // Reduce consciousness
        consciousness = Mathf.Clamp01(consciousness - (power / 1000f));
        
        // Notify listeners
        OnHit?.Invoke();
    }
    
    // Called when character falls
    private void Fall()
    {            
        OnFall?.Invoke();
        isActive = false;
    }
}