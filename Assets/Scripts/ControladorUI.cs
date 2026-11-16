using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


// Administra la interfaz de usuario (HUD) durante la partida y conserva el estado global
// de puntaje, vidas restantes y última escena jugada entre cambios de nivel o reintentos.

public class ControladorUI : MonoBehaviour
{
    public static ControladorUI Instance;

    [Header("Textos de los Marcadores")]
    [SerializeField] private TextMeshProUGUI textoScore;
    [SerializeField] private TextMeshProUGUI textoStage;
    [SerializeField] private TextMeshProUGUI textoVidas;

    [Header("Cartel Inferior de Disparo")]
    [SerializeField] private GameObject cajaDisparo;
    [SerializeField] private TextMeshProUGUI textoDisparo;

    [Header("Configuración del Nivel Actual")]
    [SerializeField] private int numeroNivel = 1;
    [SerializeField] private string nombreStagePersonalizado = ""; 
    [SerializeField] private int vidasBaseNivel = 3;

    public static int puntajeGlobal = 0;
    public static int puntajeAlIniciarNivel = 0;
    public static int vidasGlobales = 3;
    public static string ultimaEscenaJugada = "Nivel1";

    // Inicializa el Singleton de escena, registra la escena actual y guarda el puntaje base por si se reintenta
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        string escenaActual = SceneManager.GetActiveScene().name;
        ultimaEscenaJugada = escenaActual;
        puntajeAlIniciarNivel = puntajeGlobal;
    }

    // Configura el mensaje inicial de lanzamiento y sincroniza todos los marcadores visuales
    private void Start()
    {
        if (textoDisparo != null)
            textoDisparo.text = "PRESS SPACE OR CLICK TO FIRE";

        ActualizarTodaLaInterfaz();
    }

    // Reinicia todos los valores globales para comenzar una partida nueva desde cero
    public static void ReiniciarPartida()
    {
        puntajeGlobal = 0;
        puntajeAlIniciarNivel = 0;
        vidasGlobales = 3;
    }

    // Restaura las vidas y el puntaje que tenía el jugador al comenzar el nivel actual para reintentarlo
    public static void PrepararReintentoMismoNivel()
    {
        puntajeGlobal = puntajeAlIniciarNivel;
        vidasGlobales = 3;
    }

    // Incrementa el puntaje global y actualiza el marcador en pantalla
    public void SumarPuntos(int cantidad)
    {
        puntajeGlobal += cantidad;
        ActualizarScore();
    }

    // Añade vidas extra al contador global y refresca la interfaz
    public void SumarVida(int cantidad = 1)
    {
        vidasGlobales += cantidad;
        ActualizarTextoVidas();
    }

    // Descuenta vidas al perder la pelota evitando valores negativos y actualiza el HUD
    public void RestarVida(int cantidad = 1)
    {
        vidasGlobales = Mathf.Max(0, vidasGlobales - cantidad);
        ActualizarTextoVidas();
    }

    // Establece directamente una cantidad específica de vidas y actualiza su texto
    public void ActualizarVidas(int vidas)
    {
        vidasGlobales = Mathf.Max(0, vidas);
        ActualizarTextoVidas();
    }

    // Muestra u oculta el aviso inferior que indica cómo lanzar la pelota
    public void MostrarCartelDisparo(bool mostrar)
    {
        if (cajaDisparo != null)
            cajaDisparo.SetActive(mostrar);

        if (textoDisparo != null)
            textoDisparo.gameObject.SetActive(mostrar);
    }

    // Refresca simultáneamente el puntaje, el número de nivel y las vidas en pantalla
    public void ActualizarTodaLaInterfaz()
    {
        ActualizarScore();
        ActualizarStage();
        ActualizarTextoVidas();
    }

    // Formatea y actualiza el texto del puntaje con 6 dígitos
    private void ActualizarScore()
    {
        if (textoScore != null)
            textoScore.text = $"SCORE\n<color=#FFCC00>{puntajeGlobal:D6}</color>";
    }

    // Muestra el nombre personalizado del escenario (ej. CAOS) o el número de nivel con 2 dígitos
    private void ActualizarStage()
    {
        if (textoStage != null)
        {
            if (!string.IsNullOrEmpty(nombreStagePersonalizado))
            {
                textoStage.text = $"STAGE\n<color=#38BDF8>{nombreStagePersonalizado}</color>";
            }
            else
            {
                textoStage.text = $"STAGE\n<color=#38BDF8>{numeroNivel:D2}</color>";
            }
        }
    }

    // Formatea y actualiza el contador de vidas/pelotas disponibles
    private void ActualizarTextoVidas()
    {
        if (textoVidas != null)
            textoVidas.text = $"BALL\n<color=#FFCC00>{vidasGlobales:D2}</color>";
    }
}