using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorCreditos : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal"; 
    [SerializeField] private string nombreEscenaCaos = "NivelCaos";

    public void VolverAlMenu()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    public void IrAlNivelCaos()
    {
        SceneManager.LoadScene(nombreEscenaCaos);
    }
}