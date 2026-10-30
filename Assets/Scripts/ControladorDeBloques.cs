using UnityEngine;

public class ControladorDeBloque : MonoBehaviour
{
    [Header("Vida del Bloque")]
    [SerializeField] private int vida = 2;

    [Header("Colores según la vida (Opcional)")]
    [SerializeField] private Color colorDañado = Color.yellow; 
    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pelota"))
        {
            vida--; 
            if (vida <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                if (spriteRenderer != null && vida == 1)
                {
                    spriteRenderer.color = colorDañado;
                }
            }
        }
    }
}