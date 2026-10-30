using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidadCaida = 3f;
    [SerializeField] private TipoPowerUp tipo;

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

                switch (tipo)
                {
                    case TipoPowerUp.AgrandarPaleta:
                        controladorPlayer.AgrandarPaleta();
                        break;
                    
                    case TipoPowerUp.Multibola:
                        foreach (var pelota in pelotasActuales)
                        {
                            if (pelota != null && pelota.esBolaFuego)
                            {
                                pelota.esBolaFuego = false;
                                SpriteRenderer sr = pelota.GetComponent<SpriteRenderer>();
                                if (sr != null) sr.color = Color.white;
                            }
                        }

                        if (pelotasActuales.Length > 0 && pelotasActuales[0] != null)
                        {
                            int pelotasFaltantes = 3 - pelotasActuales.Length;
                            for (int i = 0; i < pelotasFaltantes; i++)
                            {
                                pelotasActuales[0].DuplicarPelota();
                            }
                        }
                        break;

                    case TipoPowerUp.Laser:
                        controladorPlayer.ActivarLaser();
                        break;

                    case TipoPowerUp.BolaFuego:
                        if (pelotasActuales.Length > 0)
                        {
                            pelotasActuales[0].ActivarBolaFuego(5f);
                            for (int i = 1; i < pelotasActuales.Length; i++)
                            {
                                if (pelotasActuales[i] != null)
                                {
                                    Destroy(pelotasActuales[i].gameObject);
                                }
                            }
                        }
                        break;
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