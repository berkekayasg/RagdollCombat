using UnityEngine;

public class RaycastShooter : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;          // Camera used for shooting
    public Transform rayOrigin;        // Where the ray starts (usually gun barrel or player position)
    public float maxDistance = 100f;   // Maximum distance the ray will travel
    public LayerMask hitLayers = -1;   // Which layers to hit
    
    [Header("Force Settings")]
    public float impactForce = 20f;    // Force applied to hit objects
    public ForceMode forceMode = ForceMode.Impulse;  // Type of force application
    
    [Header("Visual")]
    public bool showDebugRay = true;   // Show the ray in Scene view
    public float debugRayDuration = 1f; // How long the debug ray shows
    public Color rayColor = Color.red;  // Color of the debug ray
    
    [Header("Input")]
    public KeyCode shootKey = KeyCode.Mouse0;  // Key to press for shooting
    
    void Start()
    {
        // If no camera is assigned, use the main camera
        if (mainCamera == null)
            mainCamera = Camera.main;
            
        // If no ray origin is set, use this transform
        if (rayOrigin == null)
            rayOrigin = transform;
    }
    
    void Update()
    {
        // Check for input
        if (Input.GetKeyDown(shootKey))
        {
            ShootAtMousePosition();
        }
    }
    
    public void ShootAtMousePosition()
    {
        // Create a ray from the mouse position on screen into the 3D world
        Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        RaycastHit hit;
        // Shoot the ray
        if (Physics.Raycast(mouseRay, out hit, maxDistance, hitLayers))
        {

            // Debug visualization
            if (showDebugRay)
            {
                Debug.DrawLine(rayOrigin.position, hit.point, rayColor, debugRayDuration);
            }

            // Check if hit object has a rigidbody
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply force to the rigidbody
                rb.AddForce(mouseRay.direction * impactForce, forceMode);

                // Optional: Check for RagdollManager to apply the hit to the entire character
                RagdollManager ragdoll = hit.collider.GetComponentInParent<RagdollManager>();
                if (ragdoll != null)
                {
                    ragdoll.ApplyHit(mouseRay.direction, impactForce);
                }

                // Optional: Check for HitReceiver to apply damage
                HitReceiver hitReceiver = hit.collider.GetComponent<HitReceiver>();
                if (hitReceiver != null)
                {
                    hitReceiver.ReceiveHit(mouseRay.direction, impactForce * 0.5f, impactForce);
                }

                Debug.Log($"Hit {hit.collider.name} with force {impactForce}");
            }
        }

        else
        {
            // If mouse ray didn't hit anything, shoot forward from rayOrigin
            ShootForward();
        }
    }
    
    // Original shoot method, now shooting straight forward
    public void ShootForward()
    {
        // If no origin is set, use this transform
        if (rayOrigin == null)
            rayOrigin = transform;
            
        RaycastHit hit;
        // Shoot the ray
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, maxDistance, hitLayers))
        {
            // Debug visualization
            if (showDebugRay)
            {
                Debug.DrawLine(rayOrigin.position, hit.point, rayColor, debugRayDuration);
            }
            
            // Check if hit object has a rigidbody
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply force to the rigidbody
                rb.AddForce(rayOrigin.forward * impactForce, forceMode);
                
                // Optional: Check for RagdollManager to apply the hit to the entire character
                RagdollManager ragdoll = hit.collider.GetComponentInParent<RagdollManager>();
                if (ragdoll != null)
                {
                    ragdoll.ApplyHit(rayOrigin.forward, impactForce);
                }
                
                // Optional: Check for HitReceiver to apply damage
                HitReceiver hitReceiver = hit.collider.GetComponent<HitReceiver>();
                if (hitReceiver != null)
                {
                    hitReceiver.ReceiveHit(rayOrigin.forward, impactForce * 0.5f, impactForce);
                }
                
                Debug.Log($"Hit {hit.collider.name} with force {impactForce}");
            }
        }
        else
        {
            // Nothing was hit, show the ray going to maximum distance
            if (showDebugRay)
            {
                Debug.DrawLine(rayOrigin.position, rayOrigin.position + rayOrigin.forward * maxDistance, rayColor, debugRayDuration);
            }
        }
    }
    
    // Public method to shoot from code (for AI or other triggers)
    public void ShootAt(Vector3 targetPosition)
    {
        // If no origin is set, use this transform
        if (rayOrigin == null)
            rayOrigin = transform;
            
        Vector3 direction = (targetPosition - rayOrigin.position).normalized;
        RaycastHit hit;
        
        // Shoot the ray
        if (Physics.Raycast(rayOrigin.position, direction, out hit, maxDistance, hitLayers))
        {
            // Debug visualization
            if (showDebugRay)
            {
                Debug.DrawLine(rayOrigin.position, hit.point, rayColor, debugRayDuration);
            }
            
            // Check if hit object has a rigidbody
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply force to the rigidbody
                rb.AddForce(direction * impactForce, forceMode);
                
                // Same optional checks as in regular Shoot()
                RagdollManager ragdoll = hit.collider.GetComponentInParent<RagdollManager>();
                if (ragdoll != null)
                {
                    ragdoll.ApplyHit(direction, impactForce);
                }
                
                HitReceiver hitReceiver = hit.collider.GetComponent<HitReceiver>();
                if (hitReceiver != null)
                {
                    hitReceiver.ReceiveHit(direction, impactForce * 0.5f, impactForce);
                }
            }
        }
    }
}