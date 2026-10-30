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
            GameObject[] pelotasActuales = GameObject.FindGameObjectsWithTag("Pelota");

            if (pelotasActuales.Length > 1)
            {
                Destroy(collision.gameObject);
            }
            else
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
    }

    private void ReiniciarPelota(GameObject pelota)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // Limpia los power-ups de la paleta al perder una vida
            Player scriptPlayer = player.GetComponent<Player>();
            if (scriptPlayer != null)
            {
                scriptPlayer.ReiniciarEstadoPlayer();
            }

            ControladorDePelota controlPelota = pelota.GetComponent<ControladorDePelota>();
            if (controlPelota != null)
            {
                controlPelota.ReiniciarEnPaleta(player.transform);
            }
        }
    }
}