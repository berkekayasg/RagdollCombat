using UnityEngine;

public class RagdollBalance : MonoBehaviour
{
    [Header("Rigidbody References")]
    public Rigidbody head;
    public Rigidbody rightFoot;
    public Rigidbody leftFoot;
    public Rigidbody centerMass;
    public Rigidbody leftArm; // For counter-balancing
    public Rigidbody rightArm; // For counter-balancing

    [Header("Force Settings")]
    public float headUpForce = 1f;
    public float upForce = 10f;
    public float downForce = 5f;
    public float armCounterBalanceForce = 5f; // Force applied to arms for balancing

    [Header("Status")]
    public bool isBalanceActive = false;

    private RagdollManager ragdollManager;


    private void Start()
    {
        ragdollManager = GetComponent<RagdollManager>();

        if (ragdollManager == null)
            Debug.LogWarning("RagdollBalance: RagdollManager component not found!");
    }

    void FixedUpdate()
    {
        if (isBalanceActive)
        {
            // Apply basic forces
            ApplyBasicForces();

            // Apply arm counter-balance if arms are assigned
            ApplyArmCounterBalance();
        }
    }

    // Apply basic forces to keep the character upright
    private void ApplyBasicForces()
    {
        // Apply downward force to feet to keep them grounded
        rightFoot.AddForce(Vector3.down * downForce);
        leftFoot.AddForce(Vector3.down * downForce);

        // Apply upward force to center of mass, scaled by how upright the character is
        float consciousnessMultiplier = ragdollManager != null ? ragdollManager.consciousness : 1f;
        float uprightFactor = Mathf.Clamp(Vector3.Dot(Vector3.up, centerMass.transform.up), 0f, 1f);

        centerMass.AddForce(centerMass.transform.up * upForce * uprightFactor * consciousnessMultiplier);

        // Apply gentle upward force to head to help keep it upright
        head.AddForce(Vector3.up * headUpForce * consciousnessMultiplier);

        // Reduce linear velocity of center of mass based on consciousness
        centerMass.linearVelocity *= Mathf.Clamp01((1.7f - consciousnessMultiplier));
    }


    // Use arms for counter-balance when leaning
    private void ApplyArmCounterBalance()
    {
        if (leftArm == null || rightArm == null)
            return;

        // Get side-to-side lean amount
        Vector3 rightVector = centerMass.transform.right;
        float sideLeaning = Vector3.Dot(rightVector, Vector3.up);

        // Apply counter forces to arms based on lean direction
        if (sideLeaning > 0.1f)
        {
            // Leaning right, counter with left arm
            leftArm.AddForce(Vector3.up * armCounterBalanceForce * sideLeaning);
            // Optionally add some inward force to keep arms from flailing
            leftArm.AddForce(-rightVector * armCounterBalanceForce * 0.5f * sideLeaning);
        }
        else if (sideLeaning < -0.1f)
        {
            // Leaning left, counter with right arm
            rightArm.AddForce(Vector3.up * armCounterBalanceForce * -sideLeaning);
            // Optionally add some inward force to keep arms from flailing
            rightArm.AddForce(rightVector * armCounterBalanceForce * 0.5f * -sideLeaning);
        }

        // Get forward/backward lean
        Vector3 forwardVector = centerMass.transform.forward;
        float forwardLeaning = Vector3.Dot(forwardVector, Vector3.up);

        // Counter forward/backward leaning with both arms
        if (Mathf.Abs(forwardLeaning) > 0.1f)
        {
            Vector3 counterDirection = -forwardVector * Mathf.Sign(forwardLeaning);
            leftArm.AddForce(counterDirection * armCounterBalanceForce * Mathf.Abs(forwardLeaning));
            rightArm.AddForce(counterDirection * armCounterBalanceForce * Mathf.Abs(forwardLeaning));
        }
    }
}