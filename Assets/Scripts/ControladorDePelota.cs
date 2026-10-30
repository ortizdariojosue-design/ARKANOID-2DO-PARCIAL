using UnityEngine;

public class ControladorDePelota : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidadInicial = 10f;
    private Rigidbody2D rb;
    private bool estaIniciado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!estaIniciado && Input.GetKeyDown(KeyCode.Space))
        {
            LanzarPelota();
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

        Vector3 posicionPlayer = playerTransform.position;
        posicionPlayer.x = 0f;
        playerTransform.position = posicionPlayer;
        
        transform.SetParent(playerTransform);

        transform.localPosition = new Vector3(0f, 1.8f, 0f); 
        rb.velocity = Vector2.zero;
        rb.simulated = false; 
    }                                                                                                                                          
}