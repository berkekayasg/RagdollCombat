using System.Collections;
using UnityEngine;
using DitzelGames.FastIK;

public class LegControll : MonoBehaviour
{
    [Header("References")]
    public Transform leftRayOrigin;    // Left foot ray origin
    public Transform rightRayOrigin;   // Right foot ray origin
    public Transform hips;             // Character's hips
    public Transform leftFootTarget;   // IK target for left foot
    public Transform rightFootTarget;  // IK target for right foot
    public FastIKFabric leftIK;        // Left IK component
    public FastIKFabric rightIK;       // Right IK component
    
    [Header("Step Settings")]
    public float stepHeight = 0.3f;    // How high foot lifts when stepping
    public float stepThreshold = 0.3f; // When to trigger a step
    public float stepSpeed = 5f;       // How fast feet move
    public float predictiveFactor = 0.1f; // Look-ahead for steps
    
    [Header("Status")]
    public bool isLegControllActive = false;
    
    // Private variables with clearer names
    private Vector3 leftFootPos;       // Current left foot position
    private Vector3 rightFootPos;      // Current right foot position
    private Vector3 leftFootTarget2D;  // Ideal left foot position
    private Vector3 rightFootTarget2D; // Ideal right foot position
    private float leftDistance;        // Distance for left foot
    private float rightDistance;       // Distance for right foot
    private bool isLeftStepping = false;
    private bool isRightStepping = false;
    private string lastStep = "right"; // Last foot that stepped
    private bool isInitialized = false;
    private float stepTime = 0f;
    
    // Ground detection
    private readonly int groundLayer = 1; // Default ground layer

        void OnEnable()
    {
        GetComponent<RagdollManager>().OnFall += () => isLegControllActive = false;
    }

    void OnDisable()
    {
        GetComponent<RagdollManager>().OnFall -= () => isLegControllActive = false;
   }

    void Start()
    {
        leftFootPos = leftFootTarget.position;
        rightFootPos = rightFootTarget.position;
    }
    
    void Update()
    {
        if (!isLegControllActive)
        {
            if (isInitialized)
            {
                // Disable IK when not active
                leftIK.enabled = false;
                rightIK.enabled = false;
                isInitialized = false;
            }
            return;
        }
        
        // Initialize positions if needed
        if (!isInitialized)
        {
            InitializeFootPositions();
            leftIK.enabled = true;
            rightIK.enabled = true;
            isInitialized = true;
        }
        
        // Update foot positioning
        UpdateFootRaycasts();
        CheckForSteps();
        UpdateFootPositions();
    }
    
    // Initialize foot positions with raycasts
    private void InitializeFootPositions()
    {
        RaycastHit hit;
        
        // Set left foot initial position
        if (Physics.Raycast(leftRayOrigin.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            leftFootTarget.position = hit.point;
            leftFootPos = hit.point;
        }
        
        // Set right foot initial position
        if (Physics.Raycast(rightRayOrigin.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            rightFootTarget.position = hit.point;
            rightFootPos = hit.point;
        }
    }
    
    // Update foot target positions with raycasts
    private void UpdateFootRaycasts()
    {
        RaycastHit hit;
        Rigidbody hipsRb = hips.GetComponent<Rigidbody>();
        
        // Calculate predictive offset
        Vector3 velocityOffset = Vector3.zero;
        if (hipsRb != null)
        {
            velocityOffset = hipsRb.linearVelocity * predictiveFactor;
            velocityOffset.y = 0; // Keep offset horizontal
        }
        
        // Update left foot target
        if (Physics.Raycast(leftRayOrigin.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            // Calculate target position with prediction
            Vector3 baseTarget = hit.point;
            leftFootTarget2D = baseTarget + velocityOffset;
            
            // Update foot orientation to match surface
            if (!isLeftStepping)
            {
                Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                leftFootTarget.rotation = Quaternion.Slerp(leftFootTarget.rotation,
                                                          surfaceRotation * Quaternion.Euler(0, hips.eulerAngles.y, 0),
                                                          Time.deltaTime * 5f);
            }
        }
        
        // Update right foot target
        if (Physics.Raycast(rightRayOrigin.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            // Calculate target position with prediction
            Vector3 baseTarget = hit.point;
            rightFootTarget2D = baseTarget + velocityOffset;
            
            // Update foot orientation to match surface
            if (!isRightStepping)
            {
                Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                rightFootTarget.rotation = Quaternion.Slerp(rightFootTarget.rotation,
                                                           surfaceRotation * Quaternion.Euler(0, hips.eulerAngles.y, 0),
                                                           Time.deltaTime * 5f);
            }
        }
        
        // Calculate distances
        leftDistance = Vector3.Distance(leftFootPos, leftFootTarget2D);
        rightDistance = Vector3.Distance(rightFootPos, rightFootTarget2D);
    }
    
    // Check if we need to take steps
    private void CheckForSteps()
    {
        // Only start a step if we're not already stepping
        if (!isLeftStepping && !isRightStepping)
        {
            if (lastStep == "right" && leftDistance > stepThreshold)
            {
                StartCoroutine(StepLeft());
                lastStep = "left";
            }
            else if (lastStep == "left" && rightDistance > stepThreshold)
            {
                StartCoroutine(StepRight());
                lastStep = "right";
            }
        }
    }
    
    // Update foot positions
    private void UpdateFootPositions()
    {
        leftFootTarget.position = Vector3.Lerp(leftFootTarget.position, leftFootPos, Time.deltaTime * 15f);
        rightFootTarget.position = Vector3.Lerp(rightFootTarget.position, rightFootPos, Time.deltaTime * 15f);
    }
    
    // Left foot stepping coroutine
    private IEnumerator StepLeft()
    {
        isLeftStepping = true;
        
        // Calculate step speed based on distance
        float speedMultiplier = Mathf.Clamp(leftDistance / stepThreshold, 1f, 1.5f);
        
        // Store positions
        Vector3 startPos = leftFootPos;
        Vector3 targetPos = leftFootTarget2D;
        
        // Step animation
        stepTime = 0f;
        while (stepTime < 1f)
        {
            stepTime += Time.deltaTime * stepSpeed * speedMultiplier;
            
            // Calculate position with arc
            float x = Mathf.Lerp(startPos.x, targetPos.x, stepTime);
            float z = Mathf.Lerp(startPos.z, targetPos.z, stepTime);
            float y = Mathf.Sin(stepTime * Mathf.PI) * stepHeight + Mathf.Lerp(startPos.y, targetPos.y, stepTime);
            
            leftFootPos = new Vector3(x, y, z);
            
            yield return null;
        }
        
        // Ensure we reach exactly the target
        leftFootPos = targetPos;
        stepTime = 0f;
        isLeftStepping = false;
    }
    
    // Right foot stepping coroutine
    private IEnumerator StepRight()
    {
        isRightStepping = true;
        
        // Calculate step speed based on distance
        float speedMultiplier = Mathf.Clamp(rightDistance / stepThreshold, 1f, 1.5f);
        
        // Store positions
        Vector3 startPos = rightFootPos;
        Vector3 targetPos = rightFootTarget2D;
        
        // Step animation
        stepTime = 0f;
        while (stepTime < 1f)
        {
            stepTime += Time.deltaTime * stepSpeed * speedMultiplier;
            
            // Calculate position with arc
            float x = Mathf.Lerp(startPos.x, targetPos.x, stepTime);
            float z = Mathf.Lerp(startPos.z, targetPos.z, stepTime);
            float y = Mathf.Sin(stepTime * Mathf.PI) * stepHeight + Mathf.Lerp(startPos.y, targetPos.y, stepTime);
            
            rightFootPos = new Vector3(x, y, z);
            
            yield return null;
        }
        
        // Ensure we reach exactly the target
        rightFootPos = targetPos;
        stepTime = 0f;
        isRightStepping = false;
    }
}