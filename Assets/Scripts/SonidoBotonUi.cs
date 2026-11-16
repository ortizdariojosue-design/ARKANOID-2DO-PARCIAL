using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// Detecta las interacciones del puntero sobre los botones de la interfaz (pasar el cursor y hacer clic)
// y dispara sus respectivos efectos de sonido a través del ControladorAudio global.

public class SonidoBotonUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button boton;

    // Obtiene la referencia al componente Button para verificar si está habilitado
    private void Awake()
    {
        boton = GetComponent<Button>();
    }

    // Reproduce el efecto sonoro de hover cuando el cursor entra en el área de un botón interactuable
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (boton != null && !boton.interactable) return;

        if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoHoverBoton != null)
        {
            ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoHoverBoton);
        }
    }

    // Reproduce el efecto sonoro de confirmación cuando el usuario hace clic sobre un botón interactuable
    public void OnPointerClick(PointerEventData eventData)
    {
        if (boton != null && !boton.interactable) return;

        if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoClickBoton != null)
        {
            ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoClickBoton);
        }
    }
}