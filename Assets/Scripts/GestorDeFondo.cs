using UnityEngine;


// Asigna la textura o sprite de fondo correspondiente a la escena actual
// sobre el SpriteRenderer designado antes de iniciar la partida.

public class GestorDeFondo : MonoBehaviour
{
    [Header("Configuración de Fondo Retro")]
    [SerializeField] private SpriteRenderer spriteRendererFondo; 
    [SerializeField] private Sprite imagenFondo; 

    // Ejecuta la asignación visual del fondo al inicializar el objeto en la escena
    void Awake()
    {
        AplicarFondo();
    }

    // Verifica las referencias y aplica el sprite de fondo configurado al SpriteRenderer
    void AplicarFondo()
    {
        if (spriteRendererFondo != null && imagenFondo != null)
        {
            spriteRendererFondo.sprite = imagenFondo;
        }
    }
}