using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public Quaternion q;
    public bool manual;

    public SpaceTractorBeam stb;
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Setter for position
    public void SetPosition(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, 0.0f, pos.z);
        stb.y = pos.y;
    }

    // Getter for position
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    // Setter for rotation
    public void SetRotation(Quaternion rot)
    {
        //transform.rotation = rot;
        Vector3 newrotation= rot.eulerAngles;
        newrotation.z = 0;
        newrotation.x = 0;
        transform.rotation = Quaternion.Euler(newrotation);
    }

    // Getter for rotation
    public Quaternion GetRotation()
    {
        return transform.rotation;
    }
}
