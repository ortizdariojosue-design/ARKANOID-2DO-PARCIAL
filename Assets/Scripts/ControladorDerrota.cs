using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


// Controla la pantalla de Game Over (PantallaDerrota): muestra el último nivel alcanzado y el puntaje final,
// y gestiona tanto los botones de interfaz como los atajos de teclado para reintentar o volver al menú principal.

public class ControladorDerrota : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoNivelAlcanzado;
    [SerializeField] private TextMeshProUGUI textoPuntajeFinal;

    [Header("Escenas")]
    [SerializeField] private string escenaMenu = "MenuPrincipal";

    // Restablece la escala de tiempo y muestra en pantalla el nivel donde perdió el jugador junto con su puntaje final
    void Start()
    {
        Time.timeScale = 1f;

        if (textoNivelAlcanzado != null)
        {
            string nivel = ControladorUI.ultimaEscenaJugada.ToUpper();
            textoNivelAlcanzado.text = $"FAILED ON <color=#38BDF8>{nivel}</color>";
        }

        if (textoPuntajeFinal != null)
        {
            textoPuntajeFinal.text = $"FINAL SCORE\n<color=#FFCC00>{ControladorUI.puntajeGlobal:D6}</color>";
        }
    }

    // Detecta atajos rápidos de teclado para reintentar el nivel (R, Enter o Espacio) o regresar al menú (Escape)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ReintentarNivel();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            IrAlMenu();
        }
    }

    // Restaura las vidas y el puntaje inicial del nivel alcanzado y recarga esa misma escena
    public void ReintentarNivel()
    {
        ControladorUI.PrepararReintentoMismoNivel();
        SceneManager.LoadScene(ControladorUI.ultimaEscenaJugada);
    }

    // Carga la escena del menú principal
    public void IrAlMenu()
    {
        SceneManager.LoadScene(escenaMenu);
    }
}