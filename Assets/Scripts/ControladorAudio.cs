using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
// Define los parámetros individuales de cada pista musical (clip, volumen base y bucle).
[System.Serializable]
public class ConfiguracionMusica
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volumenBase = 1f;
    public bool enLoop = true;
}
// Gestor global y persistente Singleton que administra la música de fondo por escena
// y los efectos de sonido (FX), sincronizando los niveles de volumen en todo el juego.
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
    public AudioClip sonidoHoverBoton;
    public AudioClip sonidoClickBoton;

    [Header("Músicas por Escena (Con Control Individual)")]
    [SerializeField] private ConfiguracionMusica musicaMenu = new ConfiguracionMusica { volumenBase = 1f, enLoop = true };
    [SerializeField] private ConfiguracionMusica musicaTutorial = new ConfiguracionMusica { volumenBase = 1f, enLoop = true };
    [SerializeField] private ConfiguracionMusica musicaNivel1 = new ConfiguracionMusica { volumenBase = 1f, enLoop = true };
    [SerializeField] private ConfiguracionMusica musicaNivel2 = new ConfiguracionMusica { volumenBase = 1f, enLoop = true };
    [SerializeField] private ConfiguracionMusica musicaNivel3 = new ConfiguracionMusica { volumenBase = 1f, enLoop = true };
    [SerializeField] private ConfiguracionMusica musicaCaos = new ConfiguracionMusica { volumenBase = 1f, enLoop = true };
    [SerializeField] private ConfiguracionMusica musicaDerrota = new ConfiguracionMusica { volumenBase = 1f, enLoop = false };
    [SerializeField] private ConfiguracionMusica musicaCreditos = new ConfiguracionMusica { volumenBase = 1f, enLoop = true };
    [SerializeField] private ConfiguracionMusica musicaVictoria = new ConfiguracionMusica { volumenBase = 1f, enLoop = false };

    [Header("Control de Saturación")]
    [SerializeField] private float tiempoMinimoMismoSonido = 0.05f;
    private Dictionary<AudioClip, float> ultimoTiempoClip = new Dictionary<AudioClip, float>();

    // Multiplicador general controlado por los botones de Pausa / Menú (1.0, 0.6, 0.3, 0.0)
    public float MultiplicadorVolumenGeneral { get; private set; } = 1f;
    private ConfiguracionMusica pistaActual;

    // Inicializa la instancia única persistente entre escenas y suscribe el evento de cambio de escena
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Cancela la suscripción al evento de cambio de escena al destruir la instancia principal
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // Selecciona y reproduce automáticamente la pista musical correspondiente a la escena cargada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PantallaPausa") return;

        ConfiguracionMusica configElegida = musicaNivel1;

        switch (scene.name)
        {
            case "MenuPrincipal":
                configElegida = musicaMenu;
                break;
            case "Tutorial":
                configElegida = musicaTutorial;
                break;
            case "Nivel1":
                configElegida = musicaNivel1;
                break;
            case "Nivel2":
                configElegida = musicaNivel2;
                break;
            case "Nivel3":
                configElegida = musicaNivel3;
                break;
            case "NivelCaos":
                configElegida = musicaCaos;
                break;
            case "PantallaDerrota":
                configElegida = musicaDerrota;
                break;
            case "CreditosFinales":
                configElegida = musicaCreditos;
                break;
            case "PantallaVictoria":
                configElegida = musicaVictoria;
                break;
        }

        CambiarMusica(configElegida);
    }

    // Reemplaza la pista en reproducción aplicando su configuración de volumen base y bucle
    public void CambiarMusica(ConfiguracionMusica nuevaConfig)
    {
        if (musicaAudioSource == null || nuevaConfig == null || nuevaConfig.clip == null) return;

        pistaActual = nuevaConfig;

        if (musicaAudioSource.clip == nuevaConfig.clip && musicaAudioSource.isPlaying)
        {
            ActualizarVolumenes();
            return;
        }

        musicaAudioSource.clip = nuevaConfig.clip;
        musicaAudioSource.loop = nuevaConfig.enLoop;
        ActualizarVolumenes();
        musicaAudioSource.Play();
    }

    // Actualiza el multiplicador global de volumen desde los controles de UI
    public void EstablecerVolumenGeneral(float nuevoMultiplicador)
    {
        MultiplicadorVolumenGeneral = Mathf.Clamp01(nuevoMultiplicador);
        ActualizarVolumenes();
    }

    // Sincroniza el volumen final de música (respetando su volumen base) y de efectos FX
    private void ActualizarVolumenes()
    {
        if (musicaAudioSource != null)
        {
            float baseMusica = (pistaActual != null) ? pistaActual.volumenBase : 1f;
            musicaAudioSource.volume = baseMusica * MultiplicadorVolumenGeneral;
        }

        if (efectosAudioSource != null)
        {
            efectosAudioSource.volume = MultiplicadorVolumenGeneral;
        }
    }

    // Reproduce un efecto de sonido puntual evitando saturación por repeticiones simultáneas
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