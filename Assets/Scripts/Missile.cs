using UnityEngine;

public class MissileHazard : MonoBehaviour
{
    [Tooltip("How fast the missile moves.")]
    public float speed = 15f;
    
    [Tooltip("The amount of health this affects (damage or big heal).")]
    public int healthEffectAmount = 35;

     

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

/*    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            
            if (player != null)
            {
                // Assuming it's a hazard that damages the player
                player.TakeDamage(healthEffectAmount); 
                Destroy(gameObject);
            }
        }
        // Optional: Destroy the missile if it hits a wall/obstacle
        else if (other.CompareTag("Obstacle")) 
        {
            Destroy(gameObject);
        }
    }*/
}