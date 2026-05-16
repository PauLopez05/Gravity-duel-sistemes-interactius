using UnityEngine;
using System.Collections;

public class SpaceShip : MonoBehaviour
{
    public int team;
    public int hp;
    private bool isInvincible = false;
    private Renderer pr;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pr = gameObject.GetComponent<Renderer>();
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
}
