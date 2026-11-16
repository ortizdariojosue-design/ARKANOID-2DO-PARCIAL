using UnityEngine;
using TMPro;


// Aplica una animación cíclica de parpadeo suave (efecto respiración) modificando la opacidad (alpha)
// de un componente TextMeshProUGUI, funcionando incluso cuando el juego está en pausa.

public class EfectoRespiracion : MonoBehaviour
{
    private TextMeshProUGUI textoMesh;
    private float tiempo = 0f;

    [Header("Ajustes de Respiración")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float alphaMinima = 0.4f;
    [SerializeField] private float alphaMaxima = 1.0f;

    // Obtiene la referencia al componente de texto TextMeshProUGUI del objeto
    void Start()
    {
        textoMesh = GetComponent<TextMeshProUGUI>();
    }

    // Calcula la interpolación senoidal del canal alpha utilizando tiempo sin escalar 
    void Update()
    {
        if (textoMesh != null)
        {
            tiempo += Time.unscaledDeltaTime * velocidad;
            float alphaActual = Mathf.Lerp(alphaMinima, alphaMaxima, (Mathf.Sin(tiempo) + 1f) / 2f);
            
            Color color = textoMesh.color;
            color.a = alphaActual;
            textoMesh.color = color;
        }
    }
}