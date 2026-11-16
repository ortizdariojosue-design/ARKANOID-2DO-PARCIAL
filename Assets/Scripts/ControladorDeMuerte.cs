using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


// Gestiona la zona inferior de pérdida: detecta cuando la pelota cae fuera del área de juego,
// descuenta vidas si no hay multibola activa, reinicia el estado de la paleta y redirige a Game Over al quedarse sin vidas.

public class ControladorDeMuerte : MonoBehaviour
{
    [Header("Configuración de Muerte")]
    [SerializeField] private float pausaAlMorir = 1.1f;

    private bool procesandoMuerte = false;

    // Detecta el ingreso de una pelota al sensor inferior; si hay más de una (multibola), solo destruye la que cayó
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Pelota") || procesandoMuerte) return;

        GameObject[] pelotasActuales = GameObject.FindGameObjectsWithTag("Pelota");

        if (pelotasActuales.Length > 1)
        {
            collision.gameObject.tag = "Untagged";
            Destroy(collision.gameObject);
        }
        else
        {
            StartCoroutine(SecuenciaPerderVida(collision.gameObject));
        }
    }

    // Ejecuta la secuencia completa al perder la última pelota en pantalla (descuento de vida, limpieza, parpadeo y reinicio o derrota)
    private IEnumerator SecuenciaPerderVida(GameObject pelota)
    {
        procesandoMuerte = true;

        // Descuenta una vida en el controlador global de interfaz
        if (ControladorUI.Instance != null)
        {
            ControladorUI.Instance.RestarVida(1);
        }
        else
        {
            ControladorUI.vidasGlobales = Mathf.Max(0, ControladorUI.vidasGlobales - 1);
        }

        // 1. Destruye power-ups cayendo y láseres en vuelo
        LimpiarObjetosEnVuelo();

        // 2. Reproduce el sonido de pérdida de vida
        if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoPerderVida != null)
        {
            ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoPerderVida);
        }

        // 3. Oculta la pelota y congela la paleta limpiando sus poderes activos
        pelota.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Player scriptPlayer = null;
        SpriteRenderer srPlayer = null;

        if (player != null)
        {
            scriptPlayer = player.GetComponent<Player>();
            if (scriptPlayer != null)
            {
                scriptPlayer.ReiniciarEstadoPlayer();
                scriptPlayer.enabled = false;
            }
            srPlayer = player.GetComponent<SpriteRenderer>();
        }

        // 4. Pausa breve con efecto de parpadeo clásico en la paleta
        float tiempo = 0f;
        float intervaloParpadeo = 0.15f;
        while (tiempo < pausaAlMorir)
        {
            if (srPlayer != null) srPlayer.enabled = !srPlayer.enabled;
            yield return new WaitForSeconds(intervaloParpadeo);
            tiempo += intervaloParpadeo;
        }

        if (srPlayer != null) srPlayer.enabled = true;

        // 5. Si quedan vidas, reposiciona la pelota sobre la paleta; de lo contrario, carga la pantalla de derrota
        if (ControladorUI.vidasGlobales > 0)
        {
            ReiniciarPelota(pelota, player);
            pelota.SetActive(true);
            if (scriptPlayer != null) scriptPlayer.enabled = true;
        }
        else
        {
            SceneManager.LoadScene("PantallaDerrota");
        }

        procesandoMuerte = false;
    }

    // Elimina todos los power-ups y proyectiles láser activos en la escena para limpiar el tablero
    private void LimpiarObjetosEnVuelo()
    {
        PowerUp[] powerUps = FindObjectsOfType<PowerUp>();
        foreach (PowerUp p in powerUps)
        {
            if (p != null) Destroy(p.gameObject);
        }

        ProyectilLaser[] laseres = FindObjectsOfType<ProyectilLaser>();
        foreach (ProyectilLaser l in laseres)
        {
            if (l != null) Destroy(l.gameObject);
        }
    }

    // Devuelve la pelota a su posición inicial acoplada sobre la paleta del jugador
    private void ReiniciarPelota(GameObject pelota, GameObject player)
    {
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