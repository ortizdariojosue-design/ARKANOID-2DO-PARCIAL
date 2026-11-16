using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


// Administra la escena del Menú Principal: reinicia las estadísticas globales de partida,
// anima el título con un efecto de escala cíclico, gestiona el nivel de audio general
// y permite iniciar el juego (mediante botón o tecla Enter) o cerrar la aplicación.

public class MenuPrincipal : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [SerializeField] private string nombreEscenaJuego = "Tutorial";

    [Header("Título Animado (Efecto Respiración)")]
    [SerializeField] private RectTransform tituloArkanoid;
    [SerializeField] private float velocidadRespiracion = 2.5f;
    [SerializeField] private float intensidadRespiracion = 0.06f;
    private Vector3 escalaBaseTitulo;

    [Header("Control de Audio Cíclico")]
    [SerializeField] private TextMeshProUGUI textoBotonAudio;
    private int nivelVolumen = 3;

    // Restablece el estado inicial del menú, limpia puntaje/vidas previas y sincroniza el botón de audio
    void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Limpia puntaje y vidas anteriores apenas vuelves al menú principal
        ControladorUI.ReiniciarPartida();

        if (tituloArkanoid != null)
        {
            escalaBaseTitulo = tituloArkanoid.localScale;
        }

        // Lee el multiplicador de volumen global para mostrar el estado actual en pantalla
        float volumenActual = (ControladorAudio.Instance != null)
            ? ControladorAudio.Instance.MultiplicadorVolumenGeneral
            : AudioListener.volume;

        if (volumenActual >= 0.9f) nivelVolumen = 3;
        else if (volumenActual >= 0.5f) nivelVolumen = 2;
        else if (volumenActual >= 0.1f) nivelVolumen = 1;
        else nivelVolumen = 0;

        ActualizarTextoAudio();
    }

    // Ejecuta la animación de escala del título y detecta la tecla Enter para comenzar a jugar
    void Update()
    {
        if (tituloArkanoid != null)
        {
            float factor = 1f + Mathf.Sin(Time.time * velocidadRespiracion) * intensidadRespiracion;
            tituloArkanoid.localScale = escalaBaseTitulo * factor;
        }

        // Permite comenzar la partida presionando Enter (requisito de la consigna)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Jugar();
        }
    }

    // Reinicia las variables de partida y carga la escena de Tutorial
    public void Jugar()
    {
        ControladorUI.ReiniciarPartida();
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    // Alterna de forma cíclica el nivel de volumen global (HIGH, MID, LOW, OFF)
    public void CambiarAudioCiclico()
    {
        nivelVolumen = (nivelVolumen + 1) % 4;

        float volumenFinal = 1f;
        switch (nivelVolumen)
        {
            case 3: volumenFinal = 1.0f; break;
            case 2: volumenFinal = 0.6f; break;
            case 1: volumenFinal = 0.3f; break;
            case 0: volumenFinal = 0.0f; break;
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

    // Actualiza la etiqueta visual del botón de audio según el nivel seleccionado
    private void ActualizarTextoAudio()
    {
        if (textoBotonAudio == null) return;

        switch (nivelVolumen)
        {
            case 3: textoBotonAudio.text = "SOUND HIGH"; break;
            case 2: textoBotonAudio.text = "SOUND MID"; break;
            case 1: textoBotonAudio.text = "SOUND LOW"; break;
            case 0: textoBotonAudio.text = "SOUND OFF"; break;
        }
    }
    
    // Cierra la aplicación compilada o detiene la ejecución si se prueba desde el editor de Unity
    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}