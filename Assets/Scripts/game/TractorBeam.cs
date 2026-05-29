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

    [Header("Visual Feedback Settings")]
    public Renderer beamRenderer; 
    [ColorUsage(true, true)] public Color pullColor = new Color(0f, 0.5f, 1f, 1f); // Activado soporte HDR
    [ColorUsage(true, true)] public Color pushColor = new Color(1f, 0.2f, 0f, 1f); // Activado soporte HDR
    public float colorChangeSpeed = 5f;
    
    [Tooltip("Velocidad del flujo visual hacia adentro al atraer (Suele ser negativo).")]
    public float pullScrollSpeed = -1.5f;
    [Tooltip("Velocidad del flujo visual hacia afuera al empujar (Suele ser positivo).")]
    public float pushScrollSpeed = 1.5f;

    [Header("References & Calibration")]
    public PlayerMovement pm;
    public float threshold;

    private Material beamMaterial;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        if (beamOrigin == null) beamOrigin = transform;
        
        if (beamRenderer == null) beamRenderer = GetComponent<Renderer>();
        
        // Cacheamos el material para modificar sus propiedades de forma óptima
        if (beamRenderer != null) beamMaterial = beamRenderer.material;

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
        // 1. Primero actualizamos el estado de atracción
        attract = pm.Y <= threshold;

        // 2. Controlamos las propiedades del Shader de forma dinámica
        if (beamMaterial != null)
        {
            // Definimos objetivos según el estado actual
            Color targetColor = attract ? pullColor : pushColor;
            float targetSpeed = attract ? pullScrollSpeed : pushScrollSpeed;

            // Transición suave para el color del shader (_Color)
            Color currentColor = beamMaterial.GetColor("_Color");
            Color lerpedColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorChangeSpeed);
            beamMaterial.SetColor("_Color", lerpedColor);

            // Cambiamos la velocidad de scroll de la textura (_ScrollSpeed)
            beamMaterial.SetFloat("_ScrollSpeed", targetSpeed);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null && !rb.isKinematic && (other.CompareTag(targetTag) || other.CompareTag("Missile")))
        {
            // 1. Buscamos si el objeto tiene un script de equipo
            Projectile objProjectile = rb.GetComponent<Projectile>();

            if (objProjectile != null)
            {
                // 2. Si ya tiene un equipo y NO es de nuestro equipo, lo ignoramos.
                if (objProjectile.team != Defines.NO_TEAM && objProjectile.team != this.team)
                {
                    return; 
                }

                // 3. Si el objeto es neutral, lo asignamos a nuestro equipo
                if (objProjectile.team == Defines.NO_TEAM)
                {
                    objProjectile.ChangeTeam(this.team);
                }
            }

            Vector3 directionTowardsOrigin = beamOrigin.position - rb.position;
            float distance = directionTowardsOrigin.magnitude;
            Vector3 normalizedDirection = directionTowardsOrigin.normalized;

            bool isRockInHoldZone = distance <= holdDistance && attract;

            if (!isRockInHoldZone)
            {
                float f = force;
                if (!attract)
                {
                    normalizedDirection = beamOrigin.transform.forward;
                    if (other.CompareTag("Missile"))
                    {
                        f *= 50;
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