using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorDerrota : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoNivelAlcanzado;
    [SerializeField] private TextMeshProUGUI textoPuntajeFinal;

    [Header("Escenas")]
    [SerializeField] private string escenaMenu = "MenuPrincipal";

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

    public void ReintentarNivel()
    {
        ControladorUI.PrepararReintentoMismoNivel();
        SceneManager.LoadScene(ControladorUI.ultimaEscenaJugada);
    }

    public void IrAlMenu()
    {
        SceneManager.LoadScene(escenaMenu);
    }
}