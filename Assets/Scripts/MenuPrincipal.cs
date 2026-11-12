using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Nombre de la escena de juego")]
    [SerializeField] private string nombreEscenaJuego = "Tutorial";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Saliendo del juego...");
            Application.Quit();
        }
        else if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("Tutorial");
        }
    }
}