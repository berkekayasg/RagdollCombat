using System.Collections;
using UnityEngine;
using DitzelGames.FastIK;
using Unity.VisualScripting;
public class LegControll : MonoBehaviour
{
    public Transform Lray; // Raycast origin for left foot
    public Transform Rray; // Raycast origin for right foot
    public Transform Hips; // Character's hips
    public Transform leftTarget; // IK target for the left leg
    public Transform rightTarget; // IK target for the right leg
    public FastIKFabric leftIK; // Left leg IK component
    public FastIKFabric rightIK; // Right leg IK component

    public float LegStepUp = 0.3f; // How high the leg lifts when stepping
    public float wantStepAt = 0.3f; // Distance threshold to trigger a step
    public float legSpeed = 5f; // Lerp speed for foot movement
    public float predictiveStepFactor = 0.1f; // How much the feet should predict the body's movement

    public bool isLegControllActive = false;

    private Vector3 ShouldBeL; // Target position for left foot
    private Vector3 ShouldBeR; // Target position for right foot
    private Vector3 ShouldReallyBeL; // Ideal position for left foot based on raycast
    private Vector3 ShouldReallyBeR; // Ideal position for right foot based on raycast
    private float DistL; // Distance for left foot
    private float DistR; // Distance for right foot
    private bool LStepping = false; // Left leg is stepping
    private bool RStepping = false; // Right leg is stepping
    private string LastStep = "R"; // Last foot that stepped
    private bool firstTime = true;
    float timer = 0f;

    void Start()
    {
        ShouldBeL = leftTarget.position;
        ShouldBeR = rightTarget.position;
    }

    void Update()
    {
        if (!isLegControllActive)
        {
            firstTime = true;

            leftIK.enabled = false;
            rightIK.enabled = false;

            enabled = false;
            return;
        }
        else
        {
            if (firstTime)
            {
                // Initialize foot positions
                RaycastHit hitLFirst;
                if (Physics.Raycast(Lray.position, Vector3.down, out hitLFirst, Mathf.Infinity, 1))
                {
                    leftTarget.position = hitLFirst.point;
                }

                RaycastHit hitRFirst;
                if (Physics.Raycast(Rray.position, Vector3.down, out hitRFirst, Mathf.Infinity, 1))
                {
                    rightTarget.position = hitRFirst.point;
                }

                ShouldBeL = leftTarget.position;
                ShouldBeR = rightTarget.position;
                firstTime = false;
            }

            leftIK.enabled = true;
            rightIK.enabled = true;
        }

        // Cast rays to find where feet should be
        RaycastHit hitL;
        if (Physics.Raycast(Lray.position, Vector3.down, out hitL, Mathf.Infinity, 1))
        {
            // Add predictive offset based on movement direction
            Vector3 directionOffset = Vector3.ClampMagnitude(Hips.GetComponent<Rigidbody>().linearVelocity, 1f) * predictiveStepFactor;
            // Only apply significant offsets for horizontal movement
            directionOffset.y = 0;

            // Calculate the base target position
            Vector3 baseTarget = 0.4f * (hitL.point - leftTarget.position) + hitL.point;
            baseTarget.y = hitL.point.y;
            // Apply direction offset for predictive stepping
            ShouldReallyBeL = baseTarget + directionOffset;

            // Apply terrain adaptation - align foot with surface normal
            Vector3 surfaceNormal = hitL.normal;
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
            leftTarget.rotation = Quaternion.Slerp(leftTarget.rotation,
                                                  surfaceRotation * Quaternion.Euler(0, Hips.eulerAngles.y, 0),
                                                  Time.deltaTime * 5f);
        }

        RaycastHit hitR;
        if (Physics.Raycast(Rray.position, Vector3.down, out hitR, Mathf.Infinity, 1))
        {
            // Add predictive offset based on movement direction
            Vector3 directionOffset = Vector3.ClampMagnitude(Hips.GetComponent<Rigidbody>().linearVelocity, 1f) * predictiveStepFactor;

            // Only apply significant offsets for horizontal movement
            directionOffset.y = 0;

            // Calculate the base target position
            Vector3 baseTarget = 0.4f * (hitR.point - rightTarget.position) + hitR.point;
            baseTarget.y = hitR.point.y;
            // Apply direction offset for predictive stepping
            ShouldReallyBeR = baseTarget + directionOffset;

            // Apply terrain adaptation - align foot with surface normal
            Vector3 surfaceNormal = hitR.normal;
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
            rightTarget.rotation = Quaternion.Slerp(rightTarget.rotation,
                                                  surfaceRotation * Quaternion.Euler(0, Hips.eulerAngles.y, 0),
                                                  Time.deltaTime * 5f);
        }

        // Calculate distances for each foot
        DistL = Vector3.Distance(ShouldBeL, ShouldReallyBeL);
        DistR = Vector3.Distance(ShouldBeR, ShouldReallyBeR);

        // Decide which foot should step next
        if (!LStepping && !RStepping)
        {
            if (LastStep == "R" && DistL > wantStepAt)
            {
                StartCoroutine(stepL());
                LastStep = "L";
            }
            else if (LastStep == "L" && DistR > wantStepAt)
            {
                StartCoroutine(stepR());
                LastStep = "R";
            }
        }

        // Smoothly move feet to their targets
        leftTarget.position = Vector3.Lerp(leftTarget.position, ShouldBeL, Time.deltaTime * 15f);
        rightTarget.position = Vector3.Lerp(rightTarget.position, ShouldBeR, Time.deltaTime * 15f);
    }

    // Left foot stepping coroutine
    IEnumerator stepL()
    {
        LStepping = true;
        float extraSpeeder = Mathf.Clamp(Vector3.Distance(ShouldBeL, ShouldReallyBeL) / 0.3f, 1f, 1.5f);
        Vector3 currentShouldReallyBeL = this.ShouldReallyBeL;
        while (timer < 1f)
        {
            timer += Time.deltaTime * legSpeed * extraSpeeder;

            ShouldBeL = CalculatePositionLeft(currentShouldReallyBeL);

            yield return null;
        }
        timer = 0f;
        LStepping = false;
    }

    // Right foot stepping coroutine
    IEnumerator stepR()
    {
        RStepping = true;
        float extraSpeeder = Mathf.Clamp(Vector3.Distance(ShouldBeR, ShouldReallyBeR) / 0.3f, 1f, 1.5f);
        Vector3 currentShouldReallyBeR = this.ShouldReallyBeR;
        while (timer < 1f)
        {
            timer += Time.deltaTime * legSpeed * extraSpeeder;

            ShouldBeR = CalculatePositionRight(currentShouldReallyBeR);

            yield return null;
        }
        timer = 0f;
        RStepping = false;
    }
    Vector3 CalculatePositionLeft(Vector3 ShouldReallyBeL)
    {
        float x = leftTarget.position.x * (1f - timer) + ShouldReallyBeL.x * timer;
        float y = Mathf.Sin(timer * 180f * Mathf.Deg2Rad) * LegStepUp + ShouldReallyBeL.y;
        float z = leftTarget.position.z * (1f - timer) + ShouldReallyBeL.z * timer;
        return new Vector3(x, y, z);
    }

    Vector3 CalculatePositionRight(Vector3 ShouldReallyBeR)
    {
        float x = rightTarget.position.x * (1f - timer) + ShouldReallyBeR.x * timer;
        float y = Mathf.Sin(timer * 180f * Mathf.Deg2Rad) * LegStepUp + ShouldReallyBeR.y;
        float z = rightTarget.position.z * (1f - timer) + ShouldReallyBeR.z * timer;
        return new Vector3(x, y, z);
    }
}