using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 10f;
    [SerializeField] private bool permitirControlMouse = true;
    [SerializeField] private Collider2D paredIzquierda;
    [SerializeField] private Collider2D paredDerecha;
    [SerializeField] private float limiteIzquierdoX = -8.2f;
    [SerializeField] private float limiteDerechoX = 3.2f;

    public float CentroX { get; private set; } = 0f;

    [Header("Power-up Agrandar (Permanente hasta morir o cambiar poder)")]
    [SerializeField] private float escalaAmpliada = 1.5f;
    private Vector3 escalaOriginal;
    private bool estaAgrandada = false;
    private BoxCollider2D boxCollider;
    private Vector3 ultimaPosicionMouse;
    private bool usandoMouse = false;

    [Header("Power-up Láser (Temporal)")]
    [SerializeField] private GameObject prefabLaser; 
    [SerializeField] private float duracionLaser = 6f;
    [SerializeField] private float cadenciaDisparo = 0.4f;
    [SerializeField] private float margenBordeLaser = 0.15f;
    [SerializeField] private float alturaDisparoLaser = 0.4f;
    private Coroutine corrutinaLaser;

    void Start()
    {
        escalaOriginal = transform.localScale;
        boxCollider = GetComponent<BoxCollider2D>();
        ultimaPosicionMouse = Input.mousePosition;
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
        if (Time.timeScale <= 0f) return;

        float entradaX = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(entradaX) > 0.01f)
        {
            usandoMouse = false;
        }
        else if (permitirControlMouse && (Input.mousePosition - ultimaPosicionMouse).sqrMagnitude > 1f)
        {
            usandoMouse = true;
        }
        ultimaPosicionMouse = Input.mousePosition;

        Vector3 nuevaPosicion = transform.position;

        if (usandoMouse && Camera.main != null)
        {
            Vector3 mundoMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            nuevaPosicion.x = mundoMouse.x;
        }
        else
        {
            nuevaPosicion.x += entradaX * velocidadMovimiento * Time.deltaTime;
        }

        float mitadAnchoReal = boxCollider != null ? boxCollider.bounds.extents.x : 1f;
        float bordeIzq = paredIzquierda != null ? paredIzquierda.bounds.max.x : limiteIzquierdoX;
        float bordeDer = paredDerecha != null ? paredDerecha.bounds.min.x : limiteDerechoX;

        nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, bordeIzq + mitadAnchoReal, bordeDer - mitadAnchoReal);
        transform.position = nuevaPosicion;
    }

    public bool AgrandarPaleta()
    {
        bool esModoCaos = (SceneManager.GetActiveScene().name == "NivelCaos");

        // Si NO estamos en modo caos, reemplazamos el láser por el agrandado (comportamiento normal)
        if (!esModoCaos)
        {
            DetenerLaser();
        }

        if (estaAgrandada) return false;

        estaAgrandada = true;
        transform.localScale = new Vector3(escalaOriginal.x * escalaAmpliada, escalaOriginal.y, escalaOriginal.z);
        return true;
    }

    public bool ActivarLaser()
    {
        bool esModoCaos = (SceneManager.GetActiveScene().name == "NivelCaos");

        // Si NO estamos en modo caos, el láser quita el agrandado (comportamiento normal)
        if (!esModoCaos)
        {
            QuitarAgrandar();
        }

        if (corrutinaLaser != null)
        {
            StopCoroutine(corrutinaLaser);
        }

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

    public void QuitarAgrandar()
    {
        estaAgrandada = false;
        transform.localScale = escalaOriginal;
    }

    public void DetenerLaser()
    {
        if (corrutinaLaser != null)
        {
            StopCoroutine(corrutinaLaser);
            corrutinaLaser = null;
        }
    }

    public void ReiniciarEstadoPlayer()
    {
        QuitarAgrandar();
        DetenerLaser();
    }
}