using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    Rigidbody[] bodyParts;
    public float consciousness = 1f;
    public float generalForce = 10f;
    public float fallAngle = 0.68f;
    public float limitRbSpeed = 10f;

    // Balance parameters
    public bool isActive = false;
    private Transform hips;

    private void Start()
    {
        hips = transform.Find("Hips");
        bodyParts = GetComponentsInChildren<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            // Check if character has fallen over
            if (Mathf.Clamp(Vector3.Dot(Vector3.up, hips.up), 0f, 1f) < fallAngle)
            {
                GetComponent<LegControll>().isLegControllActive = false;
                GetComponent<RagdollBalance>().isBalanceActive = false;
                isActive = false;
                // Character has fallen
            }
        }
    }

    // Apply a hit force to the character
    public void ApplyHit(Vector3 hitDirection, float power)
    {
        Vector3 hitPosition = hips.position - (hitDirection * 0.25f) + new Vector3(0, 0.25f, 0);
        
        // Apply explosion force to upper body parts
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].AddExplosionForce(power, hitPosition, 0.33f);
        }

        consciousness -= power / 1000f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            consciousness -= 0.1f;
        }
    }
}