using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPausa : MonoBehaviour
{
    private bool juegoPausado = false;

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

    public void Reanudar()
    {
        juegoPausado = false;
        Time.timeScale = 1f; 
    }

    void Pausar()
    {
        juegoPausado = true;
        Time.timeScale = 0f; 
        
        SceneManager.LoadScene("PantallaPausa", LoadSceneMode.Additive);
    }
}