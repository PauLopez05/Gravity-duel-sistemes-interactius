using UnityEngine;

public class HealthPowerUp : MonoBehaviour
{
    [Tooltip("How much health this power-up restores.")]
    public int healAmount = 1;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpaceShip player = other.GetComponent<SpaceShip>();
            
            if (player != null)
            {
                player.Heal(healAmount); 
                Destroy(gameObject); 
            }
        }
    }
}