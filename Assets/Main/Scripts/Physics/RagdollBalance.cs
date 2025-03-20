using UnityEngine;

public class RagdollBalance : MonoBehaviour
{
    [Header("Rigidbody References")]
    public Rigidbody head;
    public Rigidbody centerMass;
    public Rigidbody leftFoot;
    public Rigidbody rightFoot;
    public Rigidbody leftArm;
    public Rigidbody rightArm;
    
    [Header("Force Settings")]
    public float headUpForce = 1f;
    public float upForce = 10f;
    public float downForce = 5f;
    public float armCounterForce = 5f;
    
    [Header("Status")]
    public bool isBalanceActive = false;
    
    private RagdollManager ragdollManager;


    void OnEnable()
    {
        GetComponent<RagdollManager>().OnFall += () => isBalanceActive = false;
    }

    void OnDisable()
    {
        GetComponent<RagdollManager>().OnFall -= () => isBalanceActive = false;
    }

    void Start()
    {
        ragdollManager = GetComponent<RagdollManager>();
    }
    
    void FixedUpdate()
    {
        if (!isBalanceActive) return;
        
        // Only apply forces if we have the necessary components
        if (centerMass == null) return;
        
        // Get consciousness level
        float conscious = ragdollManager != null ? ragdollManager.consciousness : 1f;
        
        // Apply ground forces to feet
        ApplyFootForces();
        
        // Apply upright forces to body
        ApplyUprightForces(conscious);
        
        // Apply counter-balance with arms
        ApplyArmCounterBalance();
    }
    
    void ApplyFootForces()
    {
        if (leftFoot != null)
            leftFoot.AddForce(Vector3.down * downForce);
            
        if (rightFoot != null)
            rightFoot.AddForce(Vector3.down * downForce);
    }
    
    void ApplyUprightForces(float conscious)
    {
        // Calculate how upright the character is (0-1)
        float uprightFactor = Mathf.Clamp01(Vector3.Dot(Vector3.up, centerMass.transform.up));
        
        // Apply scaled force to stay upright
        centerMass.AddForce(centerMass.transform.up * upForce * uprightFactor * conscious);
        
        // Apply head force
        if (head != null)
            head.AddForce(Vector3.up * headUpForce * conscious);
        
        // Dampen velocity when unconscious
        if (conscious < 1f)
            centerMass.linearVelocity *= Mathf.Clamp01(1.7f - conscious);
    }
    
    void ApplyArmCounterBalance()
    {
        if (leftArm == null || rightArm == null) return;
        
        // Get side-to-side lean
        float sideLean = Vector3.Dot(centerMass.transform.right, Vector3.up);
        
        // Counter side lean
        if (Mathf.Abs(sideLean) > 0.1f)
        {
            if (sideLean > 0)
            {
                // Leaning right, counter with left arm
                leftArm.AddForce(Vector3.up * armCounterForce * sideLean);
                leftArm.AddForce(-centerMass.transform.right * armCounterForce * 0.5f * sideLean);
            }
            else
            {
                // Leaning left, counter with right arm
                rightArm.AddForce(Vector3.up * armCounterForce * -sideLean);
                rightArm.AddForce(centerMass.transform.right * armCounterForce * 0.5f * -sideLean);
            }
        }
        
        // Get forward/backward lean
        float forwardLean = Vector3.Dot(centerMass.transform.forward, Vector3.up);
        
        // Counter forward/backward lean
        if (Mathf.Abs(forwardLean) > 0.1f)
        {
            Vector3 counterDir = -centerMass.transform.forward * Mathf.Sign(forwardLean);
            float leanAmount = Mathf.Abs(forwardLean);
            
            leftArm.AddForce(counterDir * armCounterForce * leanAmount);
            rightArm.AddForce(counterDir * armCounterForce * leanAmount);
        }
    }
}