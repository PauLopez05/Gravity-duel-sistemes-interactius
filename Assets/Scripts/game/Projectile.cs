using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public int team = Defines.NO_TEAM;

    void OnCollisionEnter(Collision other)
    {
        if(team == Defines.NO_TEAM) return;

        if(other.gameObject.tag == "Player" && team != other.gameObject.GetComponent<SpaceShip>().team) 
        {
            other.gameObject.GetComponent<SpaceShip>().TakeDamage(team, damage);
            Destroy(gameObject);
        }
            
        
    }
}


