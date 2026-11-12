using UnityEngine;
using System.Collections;

public class ControladorDePelota : MonoBehaviour
{
    [SerializeField] private float velocidadInicial = 10f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool estaIniciado = false;
    private Vector3 posicionLocalInicial;
    
    public bool esBolaFuego = false;
    private Vector2 velocidadGuardada; 
    private Coroutine corrutinaFuego;
    private float tiempoAtrapada = 0f; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionLocalInicial = transform.localPosition;
    }

    void Start()
    {
        IgnorarColisionesEntrePelotas();
        if (!estaIniciado && ControladorUI.Instance != null)
        {
            ControladorUI.Instance.MostrarCartelDisparo(true);
        }
    }

    void Update()
    {
        if (!estaIniciado && Time.timeScale > 0f && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            LanzarPelota();
        }

        if (estaIniciado && rb != null)
        {
            if (Mathf.Abs(rb.velocity.y) < 0.2f)
            {
                tiempoAtrapada += Time.deltaTime;
                if (tiempoAtrapada > 2f) 
                {
                    float direccionY = Random.value > 0.5f ? 3f : -3f;
                    rb.velocity = new Vector2(rb.velocity.x, direccionY);
                    tiempoAtrapada = 0f;
                }
            }
            else
            {
                tiempoAtrapada = 0f;
            }

            if (rb.velocity.magnitude < velocidadInicial * 0.9f && rb.velocity.magnitude > 0.1f)
            {
                rb.velocity = rb.velocity.normalized * velocidadInicial;
            }
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            velocidadGuardada = rb.velocity;
        }
    }

    void LanzarPelota()
    {
        estaIniciado = true;
        transform.parent = null; 
        rb.simulated = true; 

        if (ControladorUI.Instance != null)
        {
            ControladorUI.Instance.MostrarCartelDisparo(false);
        }

        float randomX = Random.Range(-0.5f, 0.5f);
        Vector2 direccion = new Vector2(randomX, 1f).normalized;
        rb.velocity = direccion * velocidadInicial; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoRebotePelota != null)
            {
                ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoRebotePelota);
            }

            Collider2D colliderPaleta = collision.collider;
            float anchoPaleta = colliderPaleta.bounds.size.x;
            float puntoImpactoX = transform.position.x - collision.transform.position.x;
            float porcentaje = puntoImpactoX / (anchoPaleta / 2f);
            porcentaje = Mathf.Clamp(porcentaje, -1f, 1f);

            Vector2 direccionRebote = new Vector2(porcentaje, 1f).normalized;
            rb.velocity = direccionRebote * velocidadInicial;
        }
        else if (collision.gameObject.GetComponent<ControladorDeBloque>() != null)
        {
            if (esBolaFuego)
            {
                rb.velocity = velocidadGuardada;
            }
        }
    }

    void OnCollisionExit2D(Collision2D colision)
    {
        if (estaIniciado)
        {
            if (Mathf.Abs(rb.velocity.y) < 3f) 
            {
                float dirY = rb.velocity.y >= 0 ? 1f : -1f;
                Vector2 velocidadActual = rb.velocity;
                velocidadActual.y = dirY * 4f; 
                rb.velocity = velocidadActual.normalized * velocidadInicial;
            }                                         
        }
    }

    public void IgnorarColisionesEntrePelotas()
    {
        Collider2D miCollider = GetComponent<Collider2D>();
        if (miCollider == null) return;

        ControladorDePelota[] otrasPelotas = FindObjectsOfType<ControladorDePelota>();
        foreach (ControladorDePelota otra in otrasPelotas)
        {
            if (otra != null && otra != this)
            {
                Collider2D otroCollider = otra.GetComponent<Collider2D>();
                if (otroCollider != null)
                {
                    Physics2D.IgnoreCollision(miCollider, otroCollider, true);
                }
            }
        }
    }

    public void ReiniciarEnPaleta(Transform playerTransform)
    {
        estaIniciado = false;
        esBolaFuego = false;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        if (corrutinaFuego != null)
        {
            StopCoroutine(corrutinaFuego);
            corrutinaFuego = null;
        }

        Player scriptPlayer = playerTransform.GetComponent<Player>();
        Vector3 posicionPlayer = playerTransform.position;
        posicionPlayer.x = scriptPlayer != null ? scriptPlayer.CentroX : 0f;
        playerTransform.position = posicionPlayer;
        
        transform.SetParent(playerTransform);
        transform.localPosition = posicionLocalInicial; 
        rb.velocity = Vector2.zero;
        rb.simulated = false; 

        if (ControladorUI.Instance != null)
        {
            ControladorUI.Instance.MostrarCartelDisparo(true);
        }
    }      

    public void DuplicarPelota()
    {
        Vector3 posicionSpawn = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0f);

        GameObject nuevaPelota = Instantiate(gameObject, posicionSpawn, Quaternion.identity);
        ControladorDePelota scriptNuevaPelota = nuevaPelota.GetComponent<ControladorDePelota>();
        Rigidbody2D rbNueva = nuevaPelota.GetComponent<Rigidbody2D>();
        
        if (rbNueva != null)
        {
            float fuerzaX = Random.Range(-5f, 5f);
            float fuerzaY = Random.Range(5f, 8f);
            rbNueva.velocity = new Vector2(fuerzaX, fuerzaY).normalized * velocidadInicial; 
        }

        if (scriptNuevaPelota != null)
        {
            scriptNuevaPelota.IgnorarColisionesEntrePelotas();
            scriptNuevaPelota.estaIniciado = true;
            scriptNuevaPelota.corrutinaFuego = null;
            scriptNuevaPelota.esBolaFuego = false;
            scriptNuevaPelota.posicionLocalInicial = this.posicionLocalInicial;

            SpriteRenderer srNueva = nuevaPelota.GetComponent<SpriteRenderer>();
            if (srNueva != null)
            {
                srNueva.color = Color.white;
            }

            if (this.esBolaFuego)
            {
                scriptNuevaPelota.ActivarBolaFuego(3f);
            }
        }
    }

    public bool ActivarBolaFuego(float duracion)
    {
        if (esBolaFuego) return false;

        corrutinaFuego = StartCoroutine(RutinaFuego(duracion));
        return true;
    }

    private IEnumerator RutinaFuego(float duracion)
    {
        esBolaFuego = true;
        if (spriteRenderer != null) spriteRenderer.color = Color.red; 
        
        yield return new WaitForSeconds(duracion);
        
        esBolaFuego = false;
        if (spriteRenderer != null) spriteRenderer.color = Color.white; 
        corrutinaFuego = null;
    }
}