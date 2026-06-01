using UnityEngine;
using UnityEngine.UI;

public class ContadorVida : MonoBehaviour
{
    [Header("Configuración de UI")]
    public Image imagenVida; 

    [Header("Estilo de Animación")]
    [Tooltip("If true, health instantly chunks. If false, it drains smoothly.")]
    public bool snapToParts = true;
    public float velocidadDrenado = 5f;

    private SpaceShip parentShip;
    private int maxVida;
    private float targetFill = 1f;

    void Start()
    {
        parentShip = GetComponentInParent<SpaceShip>();

        if (parentShip != null)
        {
            maxVida = parentShip.hp;
        }
        else
        {
            Debug.LogError("ContadorVida: No SpaceShip script found on any parent object!");
        }

        ActualizarUI(inmediato: true);
    }

    void Update()
    {
        if (parentShip == null) return;

        targetFill = (float)parentShip.hp / maxVida;

        if (snapToParts)
        {
            imagenVida.fillAmount = targetFill;
        }
        else
        {
            imagenVida.fillAmount = Mathf.Lerp(imagenVida.fillAmount, targetFill, Time.deltaTime * velocidadDrenado);
        }

        ActualizarColor(parentShip.hp);
    }

    void ActualizarUI(bool inmediato)
    {
        if (parentShip == null) return;

        targetFill = (float)parentShip.hp / maxVida;

        if (inmediato)
        {
            imagenVida.fillAmount = targetFill;
        }

        ActualizarColor(parentShip.hp);
    }

    void ActualizarColor(int currentHp)
    {
        float alphaValue = 0.4f;

        if (currentHp >= 3)
            imagenVida.color = new Color(0f, 1f, 0f, alphaValue);     
        else if (currentHp == 2)
            imagenVida.color = new Color(1f, 1f, 0f, alphaValue);    
        else if (currentHp == 1)
            imagenVida.color = new Color(1f, 0f, 0f, alphaValue);      
        else if (currentHp <= 0)
            imagenVida.color = Color.clear;                          
    }
}
