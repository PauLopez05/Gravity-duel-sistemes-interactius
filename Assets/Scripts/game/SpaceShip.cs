using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class SpaceShip : MonoBehaviour
{
    public int team;
    public int hp;
    private bool isInvincible = false;
    public Renderer pr;

    [Header("Weapon Settings")]
    [Tooltip("Drag your 'test' beam child object here in the Inspector.")]
    public GameObject BeamObject;

    public GameObject healthparticle;
    public float healthParticleLifetime = 3f;

    public AudioClip healSfx;
    public AudioClip damageSfx;
    public AudioClip beamOffSfx;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void TakeDamage(int team, int damage)
    {
        if(this.team == team || isInvincible) return;

        hp -= damage;

        // Play the damage sound only if the player survived the hit
        if (damageSfx != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSfx, 3f);
        }

        if(hp <= 0)
        {
            EventManager.TriggerPlayerDeath(team);
            gameObject.SetActive(false);
            return;
        } 

        StartCoroutine(InvincibilityCoroutine());
    }
        private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float timer = 0;
        while (timer < 0.5)
        {
            if (pr != null)
            {
                pr.enabled = !pr.enabled; // Alterna entre invisible y visible
            }
            yield return new WaitForSeconds(0.1f); // Espera un instante antes de volver a parpadear
            timer += 0.1f;
        }

        if (pr != null) pr.enabled = true;
        isInvincible = false; 
    }

    public void Heal(int healAmount)
    {

        if (healAmount <= 0) return;
        int before = hp;
        hp += healAmount;
        hp = math.min(hp, 3);

        if (hp > before && healSfx != null && audioSource != null)
        {
            audioSource.PlayOneShot(healSfx,5f);
        }

        if (healthparticle != null)
        {
            GameObject fx = Instantiate(healthparticle, transform.position, Quaternion.identity);
            ParticleSystem ps = fx.GetComponent<ParticleSystem>() ?? fx.GetComponentInChildren<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(fx, healthParticleLifetime);
        }
    }


    public void JamWeapon(float duration)
    {
        if (BeamObject != null)
        {
            StartCoroutine(DisableBeamRoutine(duration));
        }
        else
        {
            Debug.LogWarning("You forgot to assign the 'test' object in the Inspector!");
        }
    }
    private IEnumerator DisableBeamRoutine(float duration)
    {

        if (audioSource != null && beamOffSfx != null)
        {
            audioSource.PlayOneShot(beamOffSfx,3);
        }
        BeamObject.SetActive(false);
        Debug.Log("Beam offline!");
        
        yield return new WaitForSeconds(duration);
        
        BeamObject.SetActive(true);
        Debug.Log("Beam online!");
    }
}
