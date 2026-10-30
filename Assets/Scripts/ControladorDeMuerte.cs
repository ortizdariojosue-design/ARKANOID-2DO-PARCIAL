using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMuerte : MonoBehaviour
{
    [Header("Configuración de Vidas")]
    [SerializeField] private int vidasJugador = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pelota"))
        {
            vidasJugador--;
            Debug.Log("Vidas restantes: " + vidasJugador);

            if (vidasJugador > 0)
            {
                ReiniciarPelota(collision.gameObject);
            }
            else
            {
                SceneManager.LoadScene("PantallaDerrota");
            }
        }
    }

    private void ReiniciarPelota(GameObject pelota)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            ControladorDePelota controlPelota = pelota.GetComponent<ControladorDePelota>();
            if (controlPelota != null)
            {
                controlPelota.ReiniciarEnPaleta(player.transform);
            }
        }
    }
}