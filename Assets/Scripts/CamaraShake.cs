using UnityEngine;
using System.Collections;

public class CamaraShake : MonoBehaviour
{
    public static CamaraShake Instance;

    private Vector3 posicionOriginal;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            posicionOriginal = transform.localPosition;
        }
    }

    private void OnEnable()
    {
        posicionOriginal = transform.localPosition;
    }

    public void LanzarShake(float duracion, float magnitud)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition = posicionOriginal;
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine(duracion, magnitud));
    }

    private IEnumerator ShakeRoutine(float duracion, float magnitud)
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            float x = Random.Range(-1f, 1f) * magnitud;
            float y = Random.Range(-1f, 1f) * magnitud;

            transform.localPosition = new Vector3(posicionOriginal.x + x, posicionOriginal.y + y, posicionOriginal.z);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = posicionOriginal;
        shakeCoroutine = null;
    }
}