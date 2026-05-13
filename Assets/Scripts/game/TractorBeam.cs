using UnityEngine;
public class SpaceTractorBeam : MonoBehaviour
{
    public Transform beamOrigin;
    public float force = 15f; 
    public bool attract = true;
    public float beamStabilizerDrag = 2f;

    private void Start()
    {
        // Make sure the collider acts as an area (Trigger)
        GetComponent<Collider>().isTrigger = true;
        
        if (beamOrigin == null)
        {
            beamOrigin = transform;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null && !rb.isKinematic)
        {
            // 1. Calculate the direction
            Vector3 directionTowardsOrigin = beamOrigin.position - rb.position;
            Vector3 normalizedDirection = directionTowardsOrigin.normalized;

            if (!attract)
            {
                normalizedDirection = -normalizedDirection;
            }

            // 2. Apply the main pulling/pushing force
            rb.AddForce(normalizedDirection * force, ForceMode.Force);

            // 3. Space Stabilization (Artificial Drag)
            // This applies a gentle counter-force based on the object's current speed.
            // It prevents the object from accelerating endlessly and makes the abduction look smooth.
            Vector3 dragForce = -rb.linearVelocity * beamStabilizerDrag;
            
            // We use ForceMode.Acceleration here so the stabilization affects heavy and light objects equally
            rb.AddForce(dragForce, ForceMode.Acceleration); 
        }
    }
}