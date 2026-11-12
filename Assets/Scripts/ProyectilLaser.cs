using UnityEngine;

public class ProyectilLaser : MonoBehaviour
{
    [SerializeField] private float velocidad = 15f;
    private bool yaImpacto = false;

    void Update()
    {
        transform.Translate(Vector3.up * velocidad * Time.deltaTime);
        if (transform.position.y > 6f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaImpacto) return;

        ControladorDeBloque bloque = collision.GetComponent<ControladorDeBloque>();
        if (bloque != null)
        {
            yaImpacto = true;

            // Ejecuta los impactos necesarios hasta destruir el bloque de una vez,
            // manteniendo intactos los puntos, sonidos, partículas y caída de power-ups.
            int seguridad = 0;
            while (bloque != null && bloque.gameObject != null && bloque.gameObject.activeInHierarchy && seguridad < 20)
            {
                bloque.RecibirImpacto(false);
                seguridad++;
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Bloque"))
        {
            yaImpacto = true;
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Pared"))
        {
            yaImpacto = true;
            Destroy(gameObject);
        }
    }
}