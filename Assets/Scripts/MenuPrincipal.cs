using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Nombre de la escena de juego")]
    [SerializeField] private string nombreEscenaJuego = "Nivel1";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Saliendo del juego...");
            Application.Quit();
        }
        else if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(nombreEscenaJuego);
        }
    }
}