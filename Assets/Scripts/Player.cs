using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 10f;
    [SerializeField] private float limiteX = 7.5f;

    [Header("Power-up Agrandar")]
    [SerializeField] private float escalaAmpliada = 1.5f;
    [SerializeField] private float duracionPowerUp = 5f;
    private Vector3 escalaOriginal;
    private Coroutine corrutinaEfecto;

    [Header("Power-up Láser")]
    [SerializeField] private GameObject prefabLaser; 
    [SerializeField] private float duracionLaser = 5f;
    private bool tieneLaser = false;
    private Coroutine corrutinaLaser;

    void Start()
    {
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        float entradaX = Input.GetAxis("Horizontal");
        Vector3 nuevaPosicion = transform.position + Vector3.right * (entradaX * velocidadMovimiento * Time.deltaTime);
        nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, -limiteX, limiteX);
        transform.position = nuevaPosicion;

        if (tieneLaser && Input.GetKeyDown(KeyCode.Space))
        {
            DispararLaser();
        }
    }

    public void AgrandarPaleta()
    {
        if (corrutinaEfecto != null) StopCoroutine(corrutinaEfecto);
        corrutinaEfecto = StartCoroutine(EfectoAgrandar());
    }

    private IEnumerator EfectoAgrandar()
    {
        transform.localScale = new Vector3(escalaOriginal.x * escalaAmpliada, escalaOriginal.y, escalaOriginal.z);
        yield return new WaitForSeconds(duracionPowerUp);
        transform.localScale = escalaOriginal;
        corrutinaEfecto = null;
    }

    public void ActivarLaser()
    {
        if (corrutinaLaser != null) StopCoroutine(corrutinaLaser);
        corrutinaLaser = StartCoroutine(EfectoLaser());
    }

    private IEnumerator EfectoLaser()
    {
        tieneLaser = true;
        yield return new WaitForSeconds(duracionLaser);
        tieneLaser = false;
        corrutinaLaser = null;
    }

    private void DispararLaser()
    {
        if (prefabLaser != null)
        {
            Vector3 posicionDisparo = transform.position + new Vector3(0f, 0.5f, 0f);
            Instantiate(prefabLaser, posicionDisparo, Quaternion.identity);
        }
    }

    // Método para limpiar los power-ups de la paleta al perder una vida
    public void ReiniciarEstadoPlayer()
    {
        if (corrutinaEfecto != null)
        {
            StopCoroutine(corrutinaEfecto);
            corrutinaEfecto = null;
        }
        transform.localScale = escalaOriginal;

        if (corrutinaLaser != null)
        {
            StopCoroutine(corrutinaLaser);
            corrutinaLaser = null;
        }
        tieneLaser = false;
    }
}