using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

        float volumenActual = (ControladorAudio.Instance != null)
            ? ControladorAudio.Instance.MultiplicadorVolumenGeneral
            : AudioListener.volume;

        if (volumenActual >= 0.9f) nivelVolumen = 3;
        else if (volumenActual >= 0.5f) nivelVolumen = 2;
        else if (volumenActual >= 0.1f) nivelVolumen = 1;
        else nivelVolumen = 0;

        ActualizarTextoAudio();
    }

    void Update()
    {
        if (tituloArkanoid != null)
        {
            float factor = 1f + Mathf.Sin(Time.time * velocidadRespiracion) * intensidadRespiracion;
            tituloArkanoid.localScale = escalaBaseTitulo * factor;
        }
    }

    public void Jugar()
    {
        ControladorUI.ReiniciarPartida();
        SceneManager.LoadScene(nombreEscenaJuego);
    }

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
    
    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}