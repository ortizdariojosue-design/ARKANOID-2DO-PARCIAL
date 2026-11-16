using UnityEngine;
using UnityEngine.SceneManagement;


// Administra la navegación desde la pantalla de Créditos Finales, permitiendo regresar
// al menú principal o acceder al nivel secreto (NivelCaos) desde sus botones de interfaz.

public class GestorCreditos : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal"; 
    [SerializeField] private string nombreEscenaCaos = "NivelCaos";

    // Carga la escena del menú principal
    public void VolverAlMenu()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    // Carga la escena oculta NivelCaos
    public void IrAlNivelCaos()
    {
        SceneManager.LoadScene(nombreEscenaCaos);
    }
}