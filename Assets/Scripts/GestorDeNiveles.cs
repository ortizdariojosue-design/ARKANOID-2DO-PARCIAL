using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorDeNiveles : MonoBehaviour
{
    [Header("Escena Destino Post-Caos")]
    [SerializeField] private string escenaMenuPrincipal = "MenuPrincipal"; 

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