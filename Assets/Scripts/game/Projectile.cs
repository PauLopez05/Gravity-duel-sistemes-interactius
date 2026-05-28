using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public int team = Defines.NO_TEAM;

    [Header("Outline Visuals")]
    [ColorUsage(true, true)]
    public Color team1Color = Color.blue;
    [ColorUsage(true, true)]
    public Color team2Color = Color.red;

    [Range(0f, 5f)]
    public float activeIntensity = 2.5f;

    private Renderer asteroidRenderer;
    private MaterialPropertyBlock propBlock;
    public GameObject hitParticles;
    public float hitFxLifetime = 2f;

    void Awake()
    {
        asteroidRenderer = GetComponentInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        ApplyTeamOutline();
    }

    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("wall"))
        {
            Destroy(gameObject);
            return;
        }

        if (team == Defines.NO_TEAM) return;

        if (other.gameObject.CompareTag("Player") && team != other.gameObject.GetComponent<SpaceShip>().team)
        {
            if (other.contactCount > 0 && hitParticles != null)
            {
            ContactPoint contact = other.contacts[0];
            GameObject fx = Instantiate(
                hitParticles,
                contact.point,
                Quaternion.LookRotation(contact.normal)
            );

            // Optional: force play if the prefab has a ParticleSystem component
            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            if (ps == null) ps = fx.GetComponentInChildren<ParticleSystem>();
            if (ps != null) ps.Play();

            Destroy(fx, hitFxLifetime);
            }
            
            other.gameObject.GetComponent<SpaceShip>().TakeDamage(team, damage);
            Destroy(gameObject);
        }
    }

    public void ChangeTeam(int newTeam)
    {
        team = newTeam;
        ApplyTeamOutline();
    }

    private void ApplyTeamOutline()
    {
        if (asteroidRenderer == null) return;

        asteroidRenderer.GetPropertyBlock(propBlock);

        if (team == 1)
        {
            propBlock.SetColor("_OutlineColor", team1Color);
            propBlock.SetFloat("_OutlineIntensity", activeIntensity);
        }
        else if (team == 2)
        {
            propBlock.SetColor("_OutlineColor", team2Color);
            propBlock.SetFloat("_OutlineIntensity", activeIntensity);
        }
        else 
        {
            propBlock.SetFloat("_OutlineIntensity", 0f);
        }

        asteroidRenderer.SetPropertyBlock(propBlock);
    }
}