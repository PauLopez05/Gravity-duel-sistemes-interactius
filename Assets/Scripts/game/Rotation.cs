using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Vector3 rotation_speed = new(.0f, .0f, .0f);

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation_speed*Time.deltaTime);
    }
}
