using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings", menuName = "RagdollCombat/Character Settings")]
public class CharacterSettings : ScriptableObject
{
    [Header("Movement")]
    public float moveForce = 10f;
    public float turnTorque = 5f;
    public float jumpForce = 15f;
    
    [Header("Combat")]
    public float punchForce = 20f;
    // Kick functionality removed due to IK system limitations
    public float attackCooldown = 0.5f;
    
    [Header("AI Settings")]
    public float targetingRange = 10f;
    public float optimalDistance = 2f;
    public float attackProbability = 0.3f;
}
