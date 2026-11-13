using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SonidoBotonUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button boton;

    private void Awake()
    {
        boton = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (boton != null && !boton.interactable) return;

        if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoHoverBoton != null)
        {
            ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoHoverBoton);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (boton != null && !boton.interactable) return;

        if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoClickBoton != null)
        {
            ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoClickBoton);
        }
    }
}