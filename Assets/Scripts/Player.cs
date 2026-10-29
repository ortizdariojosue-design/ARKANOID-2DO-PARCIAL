using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 10f;
    [SerializeField] private float limiteX = 7.5f; // 

    void Update()
    {
        float entradaX = Input.GetAxis("Horizontal");
        Vector3 nuevaPosicion = transform.position + Vector3.right * (entradaX * velocidadMovimiento * Time.deltaTime);
        nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, -limiteX, limiteX);
        transform.position = nuevaPosicion;
    }
}