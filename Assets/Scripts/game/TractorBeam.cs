using System.IO;
using UnityEngine;

public class SpaceTractorBeam : MonoBehaviour
{
    [Header("Team Settings")]
    [Tooltip("El equipo al que pertenece este rayo tractor (debe ser el mismo que el de tu nave).")]
    public int team;
    public string p;

    public Transform beamOrigin;
    public float force = 100f; 
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
    public PlayerMovement pm;
    public float threshold;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        if (beamOrigin == null) beamOrigin = transform;
        if (beamRenderer == null) beamRenderer = GetComponent<Renderer>();

        string filePath = Path.Combine(Application.persistentDataPath, $"CalibratedHeight_{p}.txt");

        if (File.Exists(filePath))
        {
            using (StreamReader sr = new StreamReader(filePath)) {
                string savedHeightText = sr.ReadLine();
                if (float.TryParse(savedHeightText, out float height)) threshold = height;
            }
        }
    }

    private void Update()
    {
        if (beamRenderer != null && beamRenderer.material != null)
        {
            Color targetColor = attract ? pullColor : pushColor;
            beamRenderer.material.color = Color.Lerp(beamRenderer.material.color, targetColor, Time.deltaTime * colorChangeSpeed);
        }

        attract = pm.Y <= threshold;
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null && !rb.isKinematic && (other.CompareTag(targetTag)||other.CompareTag("Missile")) )
        {
            // 1. Buscamos si el objeto tiene un script de equipo (usamos Projectile como base)
            Projectile objProjectile = rb.GetComponent<Projectile>();

            if (objProjectile != null)
            {
                // 2. Si ya tiene un equipo y NO es de nuestro equipo, lo ignoramos.
                // (Permitimos afectar a los de nuestro equipo para que la nave no suelte la roca nada más atraparla).
                if (objProjectile.team != Defines.NO_TEAM && objProjectile.team != this.team)
                {
                    return; // El rayo no le afecta en absoluto
                }

                // 3. Si el objeto es neutral, "lo vuelve del team"
                if (objProjectile.team == Defines.NO_TEAM)
                {
                    objProjectile.ChangeTeam(this.team);
                }
            }

            Vector3 directionTowardsOrigin = beamOrigin.position - rb.position;
            float distance = directionTowardsOrigin.magnitude;
            Vector3 normalizedDirection = directionTowardsOrigin.normalized;

            bool isRockInHoldZone =  distance <= holdDistance && attract;
            

            if (!isRockInHoldZone)
            {
                float f=force;
                if (!attract){
                    normalizedDirection = beamOrigin.transform.forward;
                    if (other.CompareTag("Missile"))
                    {
                        f*=50;
                        other.gameObject.transform.SetPositionAndRotation(other.gameObject.transform.position, beamOrigin.rotation);
                    }
                } 
                rb.AddForce(normalizedDirection * f, ForceMode.Force);
                Vector3 dragForce = -rb.linearVelocity * beamStabilizerDrag;
                rb.AddForce(dragForce, ForceMode.Acceleration); 
            }
            else
            {
                float jellyMultiplier = 5f; 
                Vector3 heavyDrag = -rb.linearVelocity * (beamStabilizerDrag * jellyMultiplier);
                rb.AddForce(heavyDrag, ForceMode.Acceleration);
            }
        }
    }
}