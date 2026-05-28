using UnityEngine;

public class DisableBeamPowerUp : MonoBehaviour
{
    [SerializeField] private float duration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        SpaceShip ship = other.GetComponent<SpaceShip>();
        if (ship == null)
        {
            ship = other.GetComponentInParent<SpaceShip>();
        }

        if (ship == null)
        {
            return;
        }

        ship.JamWeapon(duration);
        Destroy(gameObject);
    }
}