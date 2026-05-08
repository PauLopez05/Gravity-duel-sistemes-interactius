using UnityEngine;

[CreateAssetMenu(fileName = "asteroid_prefab", menuName = "Scriptable Objects/asteroid_prefab")]
public class asteroid_prefab : ScriptableObject
{
    [SerializeField] public GameObject[] asteroids_l;
}
