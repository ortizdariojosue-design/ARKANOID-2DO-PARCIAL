using UnityEngine;


// Controla el descenso vertical de las cápsulas de power-up y aplica su efecto correspondiente
// (AgrandarPaleta, Multibola, Laser, BolaFuego o VidaExtra) al entrar en contacto con la paleta.

public class PowerUp : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidadCaida = 3f;
    [SerializeField] private TipoPowerUp tipo;
    [SerializeField] private int puntosBonus = 500;

    public enum TipoPowerUp { AgrandarPaleta, Multibola, Laser, BolaFuego, VidaExtra }

    // Desplaza el power-up hacia abajo de forma constante y lo destruye si sale de los límites inferiores
    void Update()
    {
        transform.Translate(Vector3.down * velocidadCaida * Time.deltaTime, Space.World);

        if (transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
    }

    // Detecta si la cápsula es recogida por el jugador (activando el beneficio y puntaje) o si cae al sensor inferior
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player controladorPlayer = collision.GetComponent<Player>();
            if (controladorPlayer != null)
            {
                ControladorDePelota[] pelotasActuales = FindObjectsOfType<ControladorDePelota>();
                bool powerUpActivado = false;

                // Evalúa el tipo de mejora recogida y ejecuta su lógica específica
                switch (tipo)
                {
                    case TipoPowerUp.AgrandarPaleta:
                        // Agranda de forma permanente. NO apaga la bola de fuego para permitir la combinación.
                        powerUpActivado = controladorPlayer.AgrandarPaleta();
                        break;
                    
                    case TipoPowerUp.Multibola:
                        // Se duplica y hereda el estado de fuego si la pelota principal lo tenía activo
                        if (pelotasActuales.Length > 0 && pelotasActuales.Length < 3)
                        {
                            ControladorDePelota pelotaBase = pelotasActuales[0];
                            bool habiaFuegoActivo = false;

                            foreach (var p in pelotasActuales)
                            {
                                if (p != null && p.esBolaFuego)
                                {
                                    pelotaBase = p;
                                    habiaFuegoActivo = true;
                                    break;
                                }
                            }

                            int pelotasFaltantes = 3 - pelotasActuales.Length;
                            for (int i = 0; i < pelotasFaltantes; i++)
                            {
                                if (pelotaBase != null)
                                {
                                    pelotaBase.DuplicarPelota();
                                }
                            }

                            if (habiaFuegoActivo)
                            {
                                ControladorDePelota[] todasLasPelotas = FindObjectsOfType<ControladorDePelota>();
                                foreach (var pelota in todasLasPelotas)
                                {
                                    if (pelota != null) pelota.ActivarBolaFuego();
                                }
                            }

                            powerUpActivado = true;
                        }
                        break;

                    case TipoPowerUp.Laser:
                        // Láser temporal: reemplaza únicamente al Agrandar (respetando el fuego si lo hubiera)
                        powerUpActivado = controladorPlayer.ActivarLaser();
                        break;

                    case TipoPowerUp.BolaFuego:
                        // Fuego permanente en las pelotas, compatible con Agrandar y Multibola
                        foreach (var pelota in pelotasActuales)
                        {
                            if (pelota != null && pelota.ActivarBolaFuego())
                            {
                                powerUpActivado = true;
                            }
                        }
                        break;

                    case TipoPowerUp.VidaExtra:
                        // Suma 1 vida sin afectar ningún poder activo
                        if (ControladorUI.Instance != null)
                        {
                            ControladorUI.Instance.SumarVida(1);
                            powerUpActivado = true;
                        }
                        break;
                }

                // Si el efecto se aplicó correctamente, suma los puntos de bonificación y reproduce el sonido
                if (powerUpActivado)
                {
                    if (ControladorUI.Instance != null)
                    {
                        ControladorUI.Instance.SumarPuntos(puntosBonus);
                    }

                    if (ControladorAudio.Instance != null && ControladorAudio.Instance.sonidoPowerUp != null)
                    {
                        ControladorAudio.Instance.ReproducirSonido(ControladorAudio.Instance.sonidoPowerUp);
                    }
                }
            }
            Destroy(gameObject);
        }
        else if (collision.name == "SensorPerdiste")
        {
            Destroy(gameObject);
        }
    }
}