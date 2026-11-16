using UnityEngine;
using UnityEngine.SceneManagement;


// Supervisa en cada fotograma la cantidad de bloques activos en el escenario y avanza
// automáticamente a la siguiente escena del flujo de juego cuando todos son destruidos.

public class GestorDeNiveles : MonoBehaviour
{
    [Header("Escena Destino Post-Caos")]
    [SerializeField] private string escenaMenuPrincipal = "MenuPrincipal"; 

    // Comprueba si quedan bloques por destruir y carga la escena correspondiente según el nivel actual
    void Update()
    {
        ControladorDeBloque[] bloquesRestantes = FindObjectsOfType<ControladorDeBloque>();

        if (bloquesRestantes.Length == 0)
        {
            string escenaActual = SceneManager.GetActiveScene().name;

            if (escenaActual == "Tutorial")
            {
                SceneManager.LoadScene("Nivel1");
            }
            else if (escenaActual == "Nivel1")
            {
                SceneManager.LoadScene("Nivel2");
            }
            else if (escenaActual == "Nivel2")
            {
                SceneManager.LoadScene("Nivel3");
            }
            else if (escenaActual == "Nivel3")
            {
                SceneManager.LoadScene("PantallaVictoria"); 
            }
            else if (escenaActual == "NivelCaos")
            {
                SceneManager.LoadScene(escenaMenuPrincipal);
            }
        }
    }
}