using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    [SerializeField] private int vidasIniciales = 3;

    // Estado persistente entre escenas
    public static int puntajeGlobal = 0;
    public static int puntajeAlIniciarNivel = 0;
    public static int vidasGlobales = 3;
    public static string ultimaEscenaJugada = "Nivel1";

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

        ultimaEscenaJugada = SceneManager.GetActiveScene().name;

        if (numeroNivel == 1)
        {
            puntajeGlobal = 0;
            puntajeAlIniciarNivel = 0;
            vidasGlobales = vidasIniciales;
        }
        else
        {
            puntajeAlIniciarNivel = puntajeGlobal;
            if (vidasGlobales <= 0) vidasGlobales = vidasIniciales;
        }
    }

    private void Start()
    {
        if (textoDisparo != null)
            textoDisparo.text = "PRESS SPACE OR CLICK TO FIRE";

        ActualizarTodaLaInterfaz();
    }

    public static void PrepararReintentoMismoNivel()
    {
        puntajeGlobal = puntajeAlIniciarNivel;
        vidasGlobales = 3;
    }

    public void SumarPuntos(int cantidad)
    {
        puntajeGlobal += cantidad;
        ActualizarScore();
    }

    public void SumarVida(int cantidad = 1)
    {
        vidasGlobales += cantidad;
        ActualizarTextoVidas();
    }

    public void RestarVida(int cantidad = 1)
    {
        vidasGlobales = Mathf.Max(0, vidasGlobales - cantidad);
        ActualizarTextoVidas();
    }

    public void ActualizarVidas(int vidas)
    {
        vidasGlobales = Mathf.Max(0, vidas);
        ActualizarTextoVidas();
    }

    public void MostrarCartelDisparo(bool mostrar)
    {
        if (cajaDisparo != null)
            cajaDisparo.SetActive(mostrar);

        if (textoDisparo != null)
            textoDisparo.gameObject.SetActive(mostrar);
    }

    public void ActualizarTodaLaInterfaz()
    {
        ActualizarScore();
        ActualizarStage();
        ActualizarTextoVidas();
    }

    private void ActualizarScore()
    {
        if (textoScore != null)
            textoScore.text = $"SCORE\n<color=#FFCC00>{puntajeGlobal:D6}</color>";
    }

    private void ActualizarStage()
    {
        if (textoStage != null)
            textoStage.text = $"STAGE\n<color=#38BDF8>{numeroNivel:D2}</color>";
    }

    private void ActualizarTextoVidas()
    {
        if (textoVidas != null)
            textoVidas.text = $"BALL\n<color=#FFCC00>{vidasGlobales:D2}</color>";
    }
}