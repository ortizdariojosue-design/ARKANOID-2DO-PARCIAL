using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 10f;
    [SerializeField] private float bordeParedX = 7.5f; 

    [Header("Power-up Agrandar")]
    [SerializeField] private float escalaAmpliada = 1.5f;
    [SerializeField] private float duracionPowerUp = 5f;
    private Vector3 escalaOriginal;
    private Coroutine corrutinaEfecto;
    private BoxCollider2D boxCollider;

    [Header("Power-up Láser")]
    [SerializeField] private GameObject prefabLaser; 
    [SerializeField] private float duracionLaser = 5f;
    [SerializeField] private float cadenciaDisparo = 0.4f; 
    private bool tieneLaser = false;
    private Coroutine corrutinaLaser;

    void Start()
    {
        escalaOriginal = transform.localScale;
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        float entradaX = Input.GetAxis("Horizontal");
        Vector3 nuevaPosicion = transform.position + Vector3.right * (entradaX * velocidadMovimiento * Time.deltaTime);
        
        float mitadAnchoReal = 1f;
        if (boxCollider != null)
        {
            mitadAnchoReal = boxCollider.bounds.extents.x;
        }

        float limiteActualX = bordeParedX - mitadAnchoReal;
        nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, -limiteActualX, limiteActualX);
        
        transform.position = nuevaPosicion;
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
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionLaser)
        {
            DispararLaser();
            yield return new WaitForSeconds(cadenciaDisparo);
            tiempoTranscurrido += cadenciaDisparo;
        }

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