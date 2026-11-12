using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidadCaida = 3f;
    [SerializeField] private TipoPowerUp tipo;
    [SerializeField] private int puntosBonus = 500;

    public enum TipoPowerUp { AgrandarPaleta, Multibola, Laser, BolaFuego }

    void Update()
    {
        transform.Translate(Vector3.down * velocidadCaida * Time.deltaTime, Space.World);

        if (transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player controladorPlayer = collision.GetComponent<Player>();
            if (controladorPlayer != null)
            {
                ControladorDePelota[] pelotasActuales = FindObjectsOfType<ControladorDePelota>();
                bool powerUpActivado = false;

                switch (tipo)
                {
                    case TipoPowerUp.AgrandarPaleta:
                        powerUpActivado = controladorPlayer.AgrandarPaleta();
                        break;
                    
                    case TipoPowerUp.Multibola:
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
                                    if (pelota != null && !pelota.esBolaFuego)
                                    {
                                        pelota.ActivarBolaFuego(3f);
                                    }
                                }
                            }

                            powerUpActivado = true;
                        }
                        break;

                    case TipoPowerUp.Laser:
                        powerUpActivado = controladorPlayer.ActivarLaser();
                        break;

                    case TipoPowerUp.BolaFuego:
                        foreach (var pelota in pelotasActuales)
                        {
                            if (pelota != null && pelota.ActivarBolaFuego(3f))
                            {
                                powerUpActivado = true;
                            }
                        }
                        break;
                }

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