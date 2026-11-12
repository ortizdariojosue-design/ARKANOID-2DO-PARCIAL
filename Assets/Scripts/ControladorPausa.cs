using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPausa : MonoBehaviour
{
    public static ControladorPausa Instance;
    private bool juegoPausado = false;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
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

        if (!SceneManager.GetSceneByName("PantallaPausa").isLoaded)
        {
            SceneManager.LoadScene("PantallaPausa", LoadSceneMode.Additive);
        }
    }

    public void Reanudar()
    {
        if (!juegoPausado) return;

        juegoPausado = false;
        Time.timeScale = 1f;

        if (SceneManager.GetSceneByName("PantallaPausa").isLoaded)
        {
            SceneManager.UnloadSceneAsync("PantallaPausa");
        }
    }

    public void VolverAlMenu()
    {
        juegoPausado = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}