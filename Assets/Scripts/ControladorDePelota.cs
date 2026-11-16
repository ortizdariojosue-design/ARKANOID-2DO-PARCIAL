using UnityEngine;

// Controla el comportamiento físico de la pelota: lanzamiento inicial desde la paleta,
// ángulos de rebote direccional, prevención de bucles horizontales y estados de power-up (Bola de Fuego y Multibola).

public class ControladorDePelota : MonoBehaviour
{
    [SerializeField] private float velocidadInicial = 10f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool estaIniciado = false;
    private Vector3 posicionLocalInicial;
    
    public bool esBolaFuego = false;
    private Vector2 velocidadGuardada; 
    private float tiempoAtrapada = 0f; 

    // Obtiene los componentes físicos/visuales y guarda el offset inicial respecto a la paleta
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionLocalInicial = transform.localPosition;
    }

    // Configura la inmunidad de colisión entre pelotas y muestra el cartel de disparo si aún no se lanzó
    void Start()
    {
        IgnorarColisionesEntrePelotas();
        if (!estaIniciado && ControladorUI.Instance != null)
        {
            ControladorUI.Instance.MostrarCartelDisparo(true);
        }
    }

    // Gestiona la entrada de lanzamiento, evita que la pelota quede rebotando horizontalmente y mantiene su velocidad constante
    void Update()
    {
        // Detecta Espacio o Clic Izquierdo para despegar la pelota de la paleta
        if (!estaIniciado && Time.timeScale > 0f && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            LanzarPelota();
        }

        if (estaIniciado && rb != null)
        {
            // Corrige la trayectoria si la velocidad vertical es casi nula durante más de 2 segundos
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

            // Normaliza la magnitud de la velocidad para evitar que la pelota se frene tras múltiples rebotes
            if (rb.velocity.magnitude < velocidadInicial * 0.9f && rb.velocity.magnitude > 0.1f)
            {
                rb.velocity = rb.velocity.normalized * velocidadInicial;
            }
        }
    }

    // Almacena la velocidad previa al impacto para conservarla si atraviesa bloques con Bola de Fuego
    void FixedUpdate()
    {
        if (rb != null)
        {
            velocidadGuardada = rb.velocity;
        }
    }

    // Desacopla la pelota de la paleta, activa su simulación física y le aplica un impulso ascendente aleatorio
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

    // Calcula el ángulo de rebote según la zona de impacto en la paleta o mantiene la trayectoria si es Bola de Fuego
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoRebotePelota != null)
            {
                ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoRebotePelota);
            }

            // Calcula el desplazamiento relativo (-1 a 1) desde el centro de la paleta para dirigir el rebote
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
            // Si la Bola de Fuego está activa, conserva la velocidad previa para atravesar el bloque sin rebotar
            if (esBolaFuego)
            {
                rb.velocity = velocidadGuardada;
            }
        }
    }

    // Asegura un componente vertical mínimo tras cualquier rebote para evitar ángulos demasiado planos
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

    // Desactiva las colisiones físicas entre todas las pelotas activas durante el modo Multibola
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

    // Centra la paleta, vuelve a acoplar la pelota sobre ella y restablece su estado inicial tras perder una vida
    public void ReiniciarEnPaleta(Transform playerTransform)
    {
        estaIniciado = false;
        DesactivarBolaFuego();

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

    // Instancia una copia de la pelota en movimiento para el power-up Multibola, heredando el estado de fuego si estaba activo
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
            scriptNuevaPelota.posicionLocalInicial = this.posicionLocalInicial;

            // Si la bola original era de fuego, la nueva hereda el fuego permanente
            if (this.esBolaFuego)
            {
                scriptNuevaPelota.ActivarBolaFuego();
            }
            else
            {
                scriptNuevaPelota.DesactivarBolaFuego();
            }
        }
    }

    // Activa el estado de Bola de Fuego cambiando el tinte visual de la pelota a rojo
    public bool ActivarBolaFuego()
    {
        if (esBolaFuego) return false;

        esBolaFuego = true;
        if (spriteRenderer != null) spriteRenderer.color = Color.red;
        return true;
    }

    // Desactiva el estado de Bola de Fuego y restaura el color original de la pelota
    public void DesactivarBolaFuego()
    {
        esBolaFuego = false;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
    }
}