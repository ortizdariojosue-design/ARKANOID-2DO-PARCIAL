using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// Controla el comportamiento individual de cada ladrillo: resistencia (vida), cambio de color al recibir daño,
// suma de puntaje, efectos visuales/sonoros y generación aleatoria de power-ups al destruirse.

public class ControladorDeBloque : MonoBehaviour
{
    [Header("Vida y Puntos del Bloque")]
    [SerializeField] private int vida = 2;
    [SerializeField] private int puntosAlRomper = 100;

    [Header("Colores según la vida")]
    [SerializeField] private Color colorDañado = Color.yellow; 
    private SpriteRenderer spriteRenderer;

    [Header("Efectos")]
    [SerializeField] private GameObject prefabParticulas; 

    [Header("Power-ups")]
    [SerializeField] private GameObject[] prefabsPowerUps; 
    private float probabilidadPowerUp = 20f; 

    // Permite que el generador de niveles ajuste el porcentaje de aparición de power-ups de este bloque
    public void ConfigurarProbabilidad(float nuevaProbabilidad)
    {
        probabilidadPowerUp = nuevaProbabilidad;
    }

    // Obtiene el SpriteRenderer y filtra el power-up de vida extra si el nivel actual es NivelCaos
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (SceneManager.GetActiveScene().name == "NivelCaos")
        {
            if (prefabsPowerUps != null && prefabsPowerUps.Length > 0)
            {
                List<GameObject> listaFiltrada = new List<GameObject>();
                foreach (var p in prefabsPowerUps)
                {
                    if (p != null && !p.name.Contains("VidaExtra"))
                    {
                        listaFiltrada.Add(p);
                    }
                }
                prefabsPowerUps = listaFiltrada.ToArray();
            }
        }
    }

    // Detecta el choque contra la pelota y verifica si tiene activo el estado de Bola de Fuego
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pelota"))
        {
            ControladorDePelota pelota = collision.gameObject.GetComponent<ControladorDePelota>();
            bool esFuego = pelota != null && pelota.esBolaFuego;
            RecibirImpacto(esFuego);
        }
    }

    // Descuenta vida al bloque; si llega a cero dispara puntaje, sonido, shake, partículas y posible power-up
    public void RecibirImpacto(bool destruirInstantaneo = false)
    {
        if (vida <= 0) return;

        if (destruirInstantaneo)
        {
            vida = 0;
        }
        else
        {
            vida--;
        }

        if (vida <= 0)
        {
            // Actualiza el marcador global de puntaje
            if (ControladorUI.Instance != null)
            {
                ControladorUI.Instance.SumarPuntos(puntosAlRomper);
            }

            // Reproduce el efecto sonoro de destrucción y activa la sacudida de cámara
            if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoExplosionBloque != null)
            {
                ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoExplosionBloque);
            }

            if (CamaraShake.Instance != null)
            {
                CamaraShake.Instance.LanzarShake(0.1f, 0.15f);
            }

            // Instancia las partículas de explosión heredando el color actual del ladrillo
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

            // Evalúa la probabilidad aleatoria para soltar un power-up
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
            // Cambia el color del bloque para indicar visualmente que quedó con 1 punto de vida
            if (spriteRenderer != null && vida == 1)
            {
                spriteRenderer.color = colorDañado;
            }
        }
    }
}