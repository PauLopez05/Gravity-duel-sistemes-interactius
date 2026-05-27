using UnityEngine;
using System.Collections;

public class SpaceShip : MonoBehaviour
{
    public int team;
    public int hp;
    private bool isInvincible = false;
    public Renderer pr;

    [Header("Weapon Settings")]
    [Tooltip("Drag your 'test' beam child object here in the Inspector.")]
    public GameObject BeamObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    public void TakeDamage(int team, int damage)
    {
        if(this.team == team || isInvincible) return;

        hp -= damage;
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
        hp +=healAmount;
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
        BeamObject.SetActive(false);
        Debug.Log("Beam offline!");
        
        yield return new WaitForSeconds(duration);
        
        BeamObject.SetActive(true);
        Debug.Log("Beam online!");
    }
}
