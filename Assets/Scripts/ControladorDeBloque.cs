using UnityEngine;

public class ControladorDeBloque : MonoBehaviour
{
    [Header("Vida del Bloque")]
    [SerializeField] private int vida = 2;

    [Header("Colores según la vida")]
    [SerializeField] private Color colorDañado = Color.yellow; 
    private SpriteRenderer spriteRenderer;

    [Header("Efectos")]
    [SerializeField] private GameObject prefabParticulas; 

    [Header("Power-ups")]
    [SerializeField] private GameObject[] prefabsPowerUps; 
    [Range(0f, 100f)][SerializeField] private float probabilidadPowerUp = 20f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pelota"))
        {
            ControladorDePelota pelota = collision.gameObject.GetComponent<ControladorDePelota>();
            
            // Si la pelota es de fuego, lo destruye de un golpe. Si no, le quita 1 vida.
            if (pelota != null && pelota.esBolaFuego)
            {
                vida = 0;
            }
            else
            {
                vida--; 
            }
            
            if (vida <= 0)
            {
                if (CamaraShake.Instance != null)
                {
                    CamaraShake.Instance.LanzarShake(0.1f, 0.15f);
                }

                if (prefabParticulas != null)
                {
                    GameObject particulas = Instantiate(prefabParticulas, transform.position, Quaternion.identity);
                    ParticleSystem ps = particulas.GetComponent<ParticleSystem>();
                    if (ps != null && spriteRenderer != null)
                    {
                        var main = ps.main;
                        main.startColor = spriteRenderer.color;
                    }

                    Destroy(particulas, 1f);
                }

                if (prefabsPowerUps != null && prefabsPowerUps.Length > 0)
                {
                    float suerte = Random.Range(0f, 100f);
                    if (suerte <= probabilidadPowerUp)
                    {
                        GameObject powerUpElegido = prefabsPowerUps[Random.Range(0, prefabsPowerUps.Length)];
                        Instantiate(powerUpElegido, transform.position, Quaternion.identity);
                    }
                }

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