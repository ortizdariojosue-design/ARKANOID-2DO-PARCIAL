using UnityEngine;
using TMPro;

public class EfectoRespiracion : MonoBehaviour
{
    private TextMeshProUGUI textoMesh;
    private float tiempo = 0f;

    [Header("Ajustes de Respiración")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float alphaMinima = 0.4f;
    [SerializeField] private float alphaMaxima = 1.0f;

    void Start()
    {
        textoMesh = GetComponent<TextMeshProUGUI>();
    }

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