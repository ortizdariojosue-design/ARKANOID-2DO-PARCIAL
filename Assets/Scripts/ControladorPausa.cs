using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorPausa : MonoBehaviour
{
    private bool juegoPausado = false;

    [Header("Referencias de UI de Pausa")]
    [SerializeField] private GameObject ventanaPausaUI; 
    [SerializeField] private TextMeshProUGUI textoBotonAudio; 
    private int nivelVolumen = 3;

    void Start()
    {
        if (ventanaPausaUI != null)
        {
            ventanaPausaUI.SetActive(false);
        }
        Time.timeScale = 1f;
        juegoPausado = false;

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

    public void Reanudar()
    {
        juegoPausado = false;
        Time.timeScale = 1f;

        if (ventanaPausaUI != null)
        {
            ventanaPausaUI.SetActive(false);
        }
    }

    public void VolverAlMenu()
    {
        juegoPausado = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

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