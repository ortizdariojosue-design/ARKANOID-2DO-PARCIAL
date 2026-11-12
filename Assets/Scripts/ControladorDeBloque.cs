using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    public void ConfigurarProbabilidad(float nuevaProbabilidad)
    {
        probabilidadPowerUp = nuevaProbabilidad;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Si estamos en el Nivel Caos, filtramos los power-ups para que nunca salga VidaExtra
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pelota"))
        {
            ControladorDePelota pelota = collision.gameObject.GetComponent<ControladorDePelota>();
            bool esFuego = pelota != null && pelota.esBolaFuego;
            RecibirImpacto(esFuego);
        }
    }

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
            if (ControladorUI.Instance != null)
            {
                ControladorUI.Instance.SumarPuntos(puntosAlRomper);
            }

            if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoExplosionBloque != null)
            {
                ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoExplosionBloque);
            }

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