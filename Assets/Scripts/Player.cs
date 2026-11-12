using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 10f;
    [SerializeField] private Collider2D paredIzquierda;
    [SerializeField] private Collider2D paredDerecha;
    [SerializeField] private float limiteIzquierdoX = -8.2f;
    [SerializeField] private float limiteDerechoX = 3.2f;

    public float CentroX { get; private set; } = 0f;

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
    [SerializeField] private float margenBordeLaser = 0.15f;
    [SerializeField] private float alturaDisparoLaser = 0.4f;
    private Coroutine corrutinaLaser;

    void Start()
    {
        escalaOriginal = transform.localScale;
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void ConfigurarLimites(Collider2D izq, Collider2D der, float centroJuegoX)
    {
        paredIzquierda = izq;
        paredDerecha = der;
        CentroX = centroJuegoX;

        Vector3 pos = transform.position;
        pos.x = CentroX;
        transform.position = pos;
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

        float bordeIzq = paredIzquierda != null ? paredIzquierda.bounds.max.x : limiteIzquierdoX;
        float bordeDer = paredDerecha != null ? paredDerecha.bounds.min.x : limiteDerechoX;

        float limiteActualIzq = bordeIzq + mitadAnchoReal;
        float limiteActualDer = bordeDer - mitadAnchoReal;
        nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, limiteActualIzq, limiteActualDer);
        
        transform.position = nuevaPosicion;
    }

    public bool AgrandarPaleta()
    {
        if (corrutinaEfecto != null) return false;

        corrutinaEfecto = StartCoroutine(EfectoAgrandar());
        return true;
    }

    private IEnumerator EfectoAgrandar()
    {
        transform.localScale = new Vector3(escalaOriginal.x * escalaAmpliada, escalaOriginal.y, escalaOriginal.z);
        yield return new WaitForSeconds(duracionPowerUp);
        transform.localScale = escalaOriginal;
        corrutinaEfecto = null;
    }

    public bool ActivarLaser()
    {
        if (corrutinaLaser != null) return false;

        corrutinaLaser = StartCoroutine(EfectoLaser());
        return true;
    }

    private IEnumerator EfectoLaser()
    {
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionLaser)
        {
            DispararLaser();
            yield return new WaitForSeconds(cadenciaDisparo);
            tiempoTranscurrido += cadenciaDisparo;
        }

        corrutinaLaser = null;
    }

    private void DispararLaser()
    {
        if (prefabLaser != null)
        {
            float mitadAncho = boxCollider != null ? boxCollider.bounds.extents.x : 1f;
            float offsetX = Mathf.Max(0.1f, mitadAncho - margenBordeLaser);

            Vector3 posicionIzquierda = transform.position + new Vector3(-offsetX, alturaDisparoLaser, 0f);
            Vector3 posicionDerecha = transform.position + new Vector3(offsetX, alturaDisparoLaser, 0f);

            Instantiate(prefabLaser, posicionIzquierda, Quaternion.identity);
            Instantiate(prefabLaser, posicionDerecha, Quaternion.identity);

            if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoLaser != null)
            {
                ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoLaser);
            }
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
    }
}