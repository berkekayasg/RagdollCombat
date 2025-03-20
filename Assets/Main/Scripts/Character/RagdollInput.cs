using UnityEngine;

public class RagdollInput : RagdollController
{
    void Update()
    {
        if (!ragdollManager.isActive)
            return;
            
        // Attack inputs
        if (Input.GetKeyDown(KeyCode.J) && Time.time > lastAttackTime + settings.attackCooldown)
        {
            LeftPunch();
        }
        else if (Input.GetKeyDown(KeyCode.K) && Time.time > lastAttackTime + settings.attackCooldown)
        {
            RightPunch();
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
            ApplyMovement(hips.forward, vertical * settings.moveForce);
        }
        
        // Apply turning torque
        if (horizontal != 0)
        {
            ApplyTurn(horizontal);
        }
    }
}