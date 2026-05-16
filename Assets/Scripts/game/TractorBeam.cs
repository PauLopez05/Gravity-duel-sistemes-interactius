using UnityEngine;

public class SpaceTractorBeam : MonoBehaviour
{
    public Transform beamOrigin;
    public float force = 15f; 
    public bool attract = true;
    public float beamStabilizerDrag = 2f;

    [Header("Hover/Accumulate Settings")]
    [Tooltip("The distance from the origin where rocks stop moving and just float.")]
    public float holdDistance = 3f; 
    public string targetTag = "Rocks";

    // Visual Feedback variables...
    public Renderer beamRenderer; 
    public Color pullColor = new Color(0f, 0.5f, 1f, 0.5f); 
    public Color pushColor = new Color(1f, 0.2f, 0f, 0.5f); 
    public float colorChangeSpeed = 5f;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        if (beamOrigin == null) beamOrigin = transform;
        if (beamRenderer == null) beamRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (beamRenderer != null && beamRenderer.material != null)
        {
            Color targetColor = attract ? pullColor : pushColor;
            beamRenderer.material.color = Color.Lerp(beamRenderer.material.color, targetColor, Time.deltaTime * colorChangeSpeed);
        }
    }

private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null && !rb.isKinematic)
        {
            Vector3 directionTowardsOrigin = beamOrigin.position - rb.position;
            float distance = directionTowardsOrigin.magnitude;
            Vector3 normalizedDirection = directionTowardsOrigin.normalized;

            bool isRockInHoldZone = other.CompareTag(targetTag) && distance <= holdDistance && attract;

            if (!isRockInHoldZone)
            {
                // --- MOVING ZONE ---
                if (!attract)
                {
                    normalizedDirection = -normalizedDirection;
                }
                
                // Pull or Push
                rb.AddForce(normalizedDirection * force, ForceMode.Force);

                // Normal space drag
                Vector3 dragForce = -rb.linearVelocity * beamStabilizerDrag;
                rb.AddForce(dragForce, ForceMode.Acceleration); 
            }
            else
            {
                // --- HOLDING ZONE ---
                // We removed the inward pull completely. 
                // Instead, we apply a massive multiplier to the drag so they get stuck in "anti-gravity jelly".
                // They will bump into each other gently and stop.
                float jellyMultiplier = 5f; 
                Vector3 heavyDrag = -rb.linearVelocity * (beamStabilizerDrag * jellyMultiplier);
                
                rb.AddForce(heavyDrag, ForceMode.Acceleration);
            }
        }
    }
}