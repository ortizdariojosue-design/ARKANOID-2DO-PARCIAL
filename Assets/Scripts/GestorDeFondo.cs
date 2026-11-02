using UnityEngine;

public class GestorDeFondo : MonoBehaviour
{
    [Header("Configuración de Fondo Retro")]
    [SerializeField] private SpriteRenderer spriteRendererFondo; 
    [SerializeField] private Sprite imagenFondo; 

    void Awake()
    {
        AplicarFondo();
    }

    void AplicarFondo()
    {
        if (spriteRendererFondo != null && imagenFondo != null)
        {
            spriteRendererFondo.sprite = imagenFondo;
        }
    }
}