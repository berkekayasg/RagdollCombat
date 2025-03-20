using UnityEngine;

public class RagdollInput : MonoBehaviour
{
    public RagdollManager ragdollManager;
    public Transform hips;
    public float moveForce = 10f;
    public float turnTorque = 5f;
    public float jumpForce = 15f;
    public float punchForce = 20f;
    public float kickForce = 30f;
    
    public Transform leftFist;
    public Transform rightFist;
    public Transform leftFoot;
    public Transform rightFoot;
    
    public float attackCooldown = 0.5f;
    
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
        if (Input.GetKeyDown(KeyCode.J) && Time.time > lastAttackTime + attackCooldown)
        {
            LeftPunch();
            lastAttackTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.K) && Time.time > lastAttackTime + attackCooldown)
        {
            RightPunch();
            lastAttackTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.N) && Time.time > lastAttackTime + attackCooldown)
        {
            LeftKick();
            lastAttackTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.M) && Time.time > lastAttackTime + attackCooldown)
        {
            RightKick();
            lastAttackTime = Time.time;
        }
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
            hipsRb.AddForce(hips.forward * vertical * moveForce);
        }
        
        // Apply turning torque
        if (horizontal != 0)
        {
            hipsRb.AddTorque(Vector3.up * horizontal * turnTorque);
        }
        
        // Jump
        if (Input.GetButton("Jump"))
        {
            hipsRb.AddForce(Vector3.up * jumpForce);
        }
    }
    
    void LeftPunch()
    {
        if (leftFist != null && leftFist.GetComponent<Rigidbody>() != null)
        {
            leftFist.GetComponent<Rigidbody>().AddForce(hips.forward * punchForce, ForceMode.Impulse);
        }
    }
    
    void RightPunch()
    {
        if (rightFist != null && rightFist.GetComponent<Rigidbody>() != null)
        {
            rightFist.GetComponent<Rigidbody>().AddForce(hips.forward * punchForce, ForceMode.Impulse);
        }
    }
    
    void LeftKick()
    {
        if (leftFoot != null && leftFoot.GetComponent<Rigidbody>() != null)
        {
            leftFoot.GetComponent<Rigidbody>().AddForce(hips.forward * kickForce, ForceMode.Impulse);
        }
    }
    
    void RightKick()
    {
        if (rightFoot != null && rightFoot.GetComponent<Rigidbody>() != null)
        {
            rightFoot.GetComponent<Rigidbody>().AddForce(hips.forward * kickForce, ForceMode.Impulse);
        }
    }
}