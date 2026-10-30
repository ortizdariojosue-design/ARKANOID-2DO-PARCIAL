using UnityEngine;

public class ProyectilLaser : MonoBehaviour
{
    [SerializeField] private float velocidad = 15f;

    void Update()
    {
        transform.Translate(Vector3.up * velocidad * Time.deltaTime);
        if (transform.position.y > 6f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bloque") || collision.GetComponent<ControladorDeBloque>() != null)
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}