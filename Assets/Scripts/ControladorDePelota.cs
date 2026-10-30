using UnityEngine;
using System.Collections;

public class ControladorDePelota : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidadInicial = 10f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool estaIniciado = false;
    
    [Header("Bola de Fuego")]
    public bool esBolaFuego = false;
    private Vector2 velocidadGuardada; 
    private Coroutine corrutinaFuego;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!estaIniciado && Input.GetKeyDown(KeyCode.Space))
        {
            LanzarPelota();
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
        float randomX = Random.Range(-0.5f, 0.5f);
        Vector2 direccion = new Vector2(randomX, 1f).normalized;
        rb.velocity = direccion * velocidadInicial; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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
            if (Mathf.Abs(rb.velocity.y) < 2f) 
            {
                float dirY = rb.velocity.y > 0 ? 2f : -2f; 
                rb.velocity = new Vector2(rb.velocity.x, dirY).normalized * velocidadInicial; 
            }                                         
        }
    }

    public void ReiniciarEnPaleta(Transform playerTransform)
    {
        estaIniciado = false;
        
        // Apagamos la bola de fuego y restauramos el color blanco al perder/reiniciar
        esBolaFuego = false;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        if (corrutinaFuego != null)
        {
            StopCoroutine(corrutinaFuego);
            corrutinaFuego = null;
        }

        Vector3 posicionPlayer = playerTransform.position;
        posicionPlayer.x = 0f;
        playerTransform.position = posicionPlayer;
        
        transform.SetParent(playerTransform);
        transform.localPosition = new Vector3(0f, 1.8f, 0f); 
        rb.velocity = Vector2.zero;
        rb.simulated = false; 
    }      

    public void DuplicarPelota()
    {
        GameObject nuevaPelota = Instantiate(gameObject, transform.position, Quaternion.identity);
        Rigidbody2D rbNueva = nuevaPelota.GetComponent<Rigidbody2D>();
        
        if (rbNueva != null)
        {
            float fuerzaX = Random.Range(-5f, 5f);
            float fuerzaY = Random.Range(5f, 8f);
            rbNueva.velocity = new Vector2(fuerzaX, fuerzaY).normalized * velocidadInicial; 
        }                                                                                                      
    }

    public void ActivarBolaFuego(float duracion)
    {
        if (corrutinaFuego != null) StopCoroutine(corrutinaFuego);
        corrutinaFuego = StartCoroutine(RutinaFuego(duracion));
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