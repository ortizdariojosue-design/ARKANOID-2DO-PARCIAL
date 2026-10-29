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
}