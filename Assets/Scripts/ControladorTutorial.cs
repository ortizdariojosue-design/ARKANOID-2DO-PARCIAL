using UnityEngine;


// Gestiona la ventana emergente de instrucciones iniciales en la escena de Tutorial,
// ocultándola automáticamente en cuanto el jugador realiza su primera acción de movimiento o disparo.

public class ControladorTutorial : MonoBehaviour
{
    [SerializeField] private GameObject cajaTutorial; 

    // Detecta si el jugador mueve la paleta o presiona Espacio/Clic para cerrar el cartel de instrucciones
    void Update()
    {
        if (cajaTutorial != null && cajaTutorial.activeSelf)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                cajaTutorial.SetActive(false);
            }
        }
    }
}