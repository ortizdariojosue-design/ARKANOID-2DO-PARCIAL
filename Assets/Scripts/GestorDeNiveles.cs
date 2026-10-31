using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorDeNiveles : MonoBehaviour
{
    void Update()
    {
        ControladorDeBloque[] bloquesRestantes = FindObjectsOfType<ControladorDeBloque>();

        if (bloquesRestantes.Length == 0)
        {
            string escenaActual = SceneManager.GetActiveScene().name;

            if (escenaActual == "Nivel1")
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
        }
    }
}