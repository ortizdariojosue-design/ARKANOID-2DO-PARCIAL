using UnityEngine;
using System.Collections.Generic;

public class ControladorAudio : MonoBehaviour
{
    public static ControladorAudio Instance { get; private set; }

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource efectosAudioSource; 
    [SerializeField] private AudioSource musicaAudioSource; 

    [Header("Clips de Sonido (Efectos FX)")]
    public AudioClip sonidoLaser;
    public AudioClip sonidoExplosionBloque;
    public AudioClip sonidoRebotePelota;
    public AudioClip sonidoPowerUp;
    public AudioClip sonidoPerderVida;

    [Header("Música de Fondo General")]
    public AudioClip musicaFondo;

    [Header("Control de Saturación")]
    [SerializeField] private float tiempoMinimoMismoSonido = 0.05f;
    private Dictionary<AudioClip, float> ultimoTiempoClip = new Dictionary<AudioClip, float>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (musicaFondo != null && musicaAudioSource != null)
        {
            musicaAudioSource.clip = musicaFondo;
            musicaAudioSource.loop = true;
            musicaAudioSource.Play();
        }
    }

    public void ReproducirSonido(AudioClip clip)
    {
        if (clip == null || efectosAudioSource == null) return;

        if (ultimoTiempoClip.TryGetValue(clip, out float ultimoTiempo))
        {
            if (Time.unscaledTime - ultimoTiempo < tiempoMinimoMismoSonido)
            {
                return;
            }
        }

        ultimoTiempoClip[clip] = Time.unscaledTime;
        efectosAudioSource.PlayOneShot(clip);
    }
}