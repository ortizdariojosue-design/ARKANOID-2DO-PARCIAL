using UnityEngine;

public class GeneradorDeNivel : MonoBehaviour
{
    [Header("Configuración de la Grilla")]
    [SerializeField] private GameObject[] prefabsBloques;
    [SerializeField] private int columnas = 10;
    [SerializeField] private int filas = 5;
    [SerializeField] private float separacionX = 1.1f;
    [SerializeField] private float separacionY = 0.6f;
    [SerializeField] private Vector2 posicionInicial = new Vector2(-5.5f, 4f);

    void Start()
    {
        GenerarGrilla();
    }

    void GenerarGrilla()
    {
        for (int fila = 0; fila < filas; fila++)
        {
            for (int columna = 0; columna < columnas; columna++)
            {
                GameObject prefabAUsar = prefabsBloques[Random.Range(0, prefabsBloques.Length)];

                float posX = posicionInicial.x + (columna * separacionX);
                float posY = posicionInicial.y - (fila * separacionY);
                Vector3 posicionBloque = new Vector3(posX, posY, 0f);

                Instantiate(prefabAUsar, posicionBloque, Quaternion.identity, transform);
            }
        }
    }
}