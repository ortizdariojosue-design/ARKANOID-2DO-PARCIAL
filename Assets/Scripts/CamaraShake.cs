using UnityEngine;
using System.Collections;

public class CamaraShake : MonoBehaviour
{
    public static CamaraShake Instance;

    [Header("Ajustes de Shake Leve")]
    [Tooltip("Multiplica la fuerza que piden los scripts (0.6 = 60% de fuerza)")]
    [Range(0.1f, 1.5f)]
    [SerializeField] private float multiplicadorIntensidad = 0.6f;

    [Tooltip("Fuerza mínima garantizada para que el movimiento se note bien")]
    [SerializeField] private float magnitudMinimaVisible = 0.055f;

    [Tooltip("Multiplica la duración original")]
    [Range(0.5f, 1.5f)]
    [SerializeField] private float multiplicadorDuracion = 0.95f;

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

        float duracionFinal = Mathf.Max(0.1f, duracion * multiplicadorDuracion);
        float magnitudFinal = Mathf.Max(magnitudMinimaVisible, magnitud * multiplicadorIntensidad);

        shakeCoroutine = StartCoroutine(ShakeRoutine(duracionFinal, magnitudFinal));
    }

    private IEnumerator ShakeRoutine(float duracion, float magnitud)
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            float progreso = tiempoTranscurrido / duracion;
            float amortiguacion = Mathf.Lerp(1f, 0.35f, progreso);

            float x = Random.Range(-1f, 1f) * magnitud * amortiguacion;
            float y = Random.Range(-1f, 1f) * magnitud * amortiguacion;

            transform.localPosition = new Vector3(posicionOriginal.x + x, posicionOriginal.y + y, posicionOriginal.z);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = posicionOriginal;
        shakeCoroutine = null;
    }
}