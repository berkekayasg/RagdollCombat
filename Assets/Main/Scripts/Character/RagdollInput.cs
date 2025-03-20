using UnityEngine;

public class RagdollInput : MonoBehaviour
{
    public RagdollManager ragdollManager;
    public Transform hips;
    public CharacterSettings settings;
    
    public Transform leftFist;
    public Transform rightFist;
    
    private float lastAttackTime;
    private Rigidbody hipsRb;
    
    void Start()
    {
        if (ragdollManager == null)
            ragdollManager = GetComponent<RagdollManager>();
            
        hipsRb = hips.GetComponent<Rigidbody>();
        
        // Activate ragdoll physics initially
        ragdollManager.isActive = true;
    }
    
    void Update()
    {
        if (!ragdollManager.isActive)
            return;
            
        // Attack inputs
        if (Input.GetKeyDown(KeyCode.J) && Time.time > lastAttackTime + settings.attackCooldown)
        {
            LeftPunch();
            lastAttackTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.K) && Time.time > lastAttackTime + settings.attackCooldown)
        {
            RightPunch();
            lastAttackTime = Time.time;
        }
        // Kicks disabled due to IK system issues
        // Kick inputs (N, M) no longer respond
    }
    
    void FixedUpdate()
    {
        if (!ragdollManager.isActive)
            return;
            
        // Movement inputs
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Apply movement forces
        if (vertical != 0)
        {
            hipsRb.AddForce(hips.forward * vertical * settings.moveForce);
        }
        
        // Apply turning torque
        if (horizontal != 0)
        {
            hipsRb.AddTorque(Vector3.up * horizontal * settings.turnTorque);
        }
        
        // Jump
        if (Input.GetButton("Jump"))
        {
            hipsRb.AddForce(Vector3.up * settings.jumpForce);
        }
    }
    
    void LeftPunch()
    {
        if (leftFist != null && leftFist.GetComponent<Rigidbody>() != null)
        {
            leftFist.GetComponent<Rigidbody>().AddForce(hips.forward * settings.punchForce, ForceMode.Impulse);
        }
    }
    
    void RightPunch()
    {
        if (rightFist != null && rightFist.GetComponent<Rigidbody>() != null)
        {
            rightFist.GetComponent<Rigidbody>().AddForce(hips.forward * settings.punchForce, ForceMode.Impulse);
        }
    }
    
    // Kick attacks removed as they don't work well with IK system
}