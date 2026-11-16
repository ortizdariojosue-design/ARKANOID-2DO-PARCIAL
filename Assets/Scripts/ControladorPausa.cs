using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


// Administra el menú de pausa durante los niveles: congela o reanuda el tiempo de juego mediante la tecla Escape,
// permite regresar al menú principal y sincroniza el control cíclico de volumen global con el controlador de audio.

public class ControladorPausa : MonoBehaviour
{
    private bool juegoPausado = false;

    [Header("Referencias de UI de Pausa")]
    [SerializeField] private GameObject ventanaPausaUI; 
    [SerializeField] private TextMeshProUGUI textoBotonAudio; 
    private int nivelVolumen = 3;

    // Oculta el panel de pausa al iniciar el nivel y sincroniza el indicador de volumen con el nivel global actual
    void Start()
    {
        if (ventanaPausaUI != null)
        {
            ventanaPausaUI.SetActive(false);
        }
        Time.timeScale = 1f;
        juegoPausado = false;

        // Lee el multiplicador de volumen actual para reflejar el estado correcto en el botón de audio
        float volumenActual = (ControladorAudio.Instance != null)
            ? ControladorAudio.Instance.MultiplicadorVolumenGeneral
            : AudioListener.volume;

        if (volumenActual >= 0.9f) nivelVolumen = 3;
        else if (volumenActual >= 0.5f) nivelVolumen = 2;
        else if (volumenActual >= 0.1f) nivelVolumen = 1;
        else nivelVolumen = 0;

        ActualizarTextoAudio();
    }

    // Alterna entre pausar y reanudar la partida al presionar la tecla Escape
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    // Detiene el tiempo del juego (Time.timeScale = 0), muestra el panel de pausa y libera el cursor
    public void Pausar()
    {
        if (juegoPausado) return;

        juegoPausado = true;
        Time.timeScale = 0f;

        if (ventanaPausaUI != null)
        {
            ventanaPausaUI.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Restaura la velocidad normal del juego y oculta la interfaz de pausa
    public void Reanudar()
    {
        juegoPausado = false;
        Time.timeScale = 1f;

        if (ventanaPausaUI != null)
        {
            ventanaPausaUI.SetActive(false);
        }
    }

    // Restablece el tiempo normal de ejecución y regresa a la escena del menú principal
    public void VolverAlMenu()
    {
        juegoPausado = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    // Rota el nivel de volumen global entre 4 estados (HIGH, MID, LOW, OFF) y lo aplica al sistema de audio
    public void CambiarAudioCiclico()
    {
        nivelVolumen = (nivelVolumen + 1) % 4;

        float volumenFinal = 1f;

        switch (nivelVolumen)
        {
            case 3:
                volumenFinal = 1.0f;
                break;
            case 2:
                volumenFinal = 0.6f;
                break;
            case 1:
                volumenFinal = 0.3f;
                break;
            case 0:
                volumenFinal = 0.0f;
                break;
        }

        if (ControladorAudio.Instance != null)
        {
            ControladorAudio.Instance.EstablecerVolumenGeneral(volumenFinal);
        }
        else
        {
            AudioListener.volume = volumenFinal;
        }

        ActualizarTextoAudio();
    }

    // Actualiza la etiqueta de texto del botón según el nivel de volumen seleccionado
    private void ActualizarTextoAudio()
    {
        if (textoBotonAudio == null) return;

        switch (nivelVolumen)
        {
            case 3:
                textoBotonAudio.text = "SOUND HIGH";
                break;
            case 2:
                textoBotonAudio.text = "SOUND MID";
                break;
            case 1:
                textoBotonAudio.text = "SOUND LOW";
                break;
            case 0:
                textoBotonAudio.text = "SOUND OFF";
                break;
        }
    }
}