using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform camaraTransform;

    void Start()
    {
        if (Camera.main != null)
        {
            camaraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (camaraTransform == null) return;

        transform.LookAt(transform.position + camaraTransform.forward);
    }
}