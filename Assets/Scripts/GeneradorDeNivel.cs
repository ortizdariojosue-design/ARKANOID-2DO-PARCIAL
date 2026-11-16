using UnityEngine;


// Genera de forma procedural la formación de ladrillos de cada nivel según el patrón seleccionado
// (rombo, simétrico aleatorio, fortaleza, pirámide invertida o grilla completa) y configura su probabilidad de power-ups.

public class GeneradorDeNivel : MonoBehaviour
{
    public enum TipoDiseno 
    { 
        RomboClasico, 
        AleatorioSimetrico, 
        MuroFortaleza, 
        PiramideInvertida, 
        GrillaCompleta 
    }

    [Header("Estilo del Nivel")]
    [SerializeField] private TipoDiseno disenoNivel = TipoDiseno.RomboClasico;
    [Range(0.2f, 1f)]
    [SerializeField] private float densidadBloques = 0.75f;

    [Header("Configuración de la Grilla")]
    [Tooltip("Ordena los prefabs de menor a mayor resistencia (0 = blando, últimos = más duros)")]
    [SerializeField] private GameObject[] prefabsBloques;
    [SerializeField] private int columnas = 9; 
    [SerializeField] private int filas = 9;
    [SerializeField] private float centroX = 0f;
    [SerializeField] private float inicioY = 2.6f;
    [SerializeField] private float separacionX = 1.15f;
    [SerializeField] private float separacionY = 0.48f;

    [Header("Configuración de Power-ups por Nivel")]
    [Range(0f, 100f)][SerializeField] private float probabilidadPowerUpNivel = 20f;

    // Inicia la construcción de la matriz de bloques al cargar la escena
    void Start()
    {
        GenerarGrilla();
    }

    // Recorre filas y columnas para instanciar los ladrillos según el patrón elegido y asignarles su tasa de power-ups
    void GenerarGrilla()
    {
        if (prefabsBloques == null || prefabsBloques.Length == 0) return;

        // Calcula el punto inicial izquierdo para centrar toda la grilla respecto a centro X
        float anchoTotal = (columnas - 1) * separacionX;
        float startX = centroX - (anchoTotal / 2f);

        int mitadFila = filas / 2;
        int mitadCol = columnas / 2;
        int indiceMasDuro = prefabsBloques.Length - 1;

        for (int fila = 0; fila < filas; fila++)
        {
            // Precalcula una fila espejada de izquierda a derecha cuando el diseño es AleatorioSimetrico
            int[] filaSimetrica = new int[columnas];
            for (int i = 0; i < columnas; i++) filaSimetrica[i] = -1;

            if (disenoNivel == TipoDiseno.AleatorioSimetrico)
            {
                for (int c = 0; c <= mitadCol; c++)
                {
                    if (Random.value <= densidadBloques)
                    {
                        int idx = Random.Range(0, prefabsBloques.Length);
                        filaSimetrica[c] = idx;
                        filaSimetrica[columnas - 1 - c] = idx;
                    }
                }
            }

            for (int columna = 0; columna < columnas; columna++)
            {
                GameObject prefabAUsar = null;

                // Selecciona el prefab de bloque según la geometría del patrón activo
                if (disenoNivel == TipoDiseno.RomboClasico)
                {
                    int distCentro = Mathf.Abs(fila - mitadFila) + Mathf.Abs(columna - mitadCol);
                    if (distCentro <= mitadCol && Random.value <= densidadBloques)
                    {
                        prefabAUsar = prefabsBloques[Random.Range(0, prefabsBloques.Length)];
                    }
                }
                else if (disenoNivel == TipoDiseno.AleatorioSimetrico)
                {
                    if (filaSimetrica[columna] != -1)
                        prefabAUsar = prefabsBloques[filaSimetrica[columna]];
                }
                else if (disenoNivel == TipoDiseno.MuroFortaleza)
                {
                    bool esPasillo = (columna == mitadCol - 1 || columna == mitadCol + 1) && fila < filas - 2;
                    if (!esPasillo)
                    {
                        if (fila == filas - 1 || columna == 0 || columna == columnas - 1)
                            prefabAUsar = prefabsBloques[indiceMasDuro];
                        else
                            prefabAUsar = prefabsBloques[Random.Range(0, prefabsBloques.Length)];
                    }
                }
                else if (disenoNivel == TipoDiseno.PiramideInvertida)
                {
                    int margen = fila / 2;
                    if (columna >= margen && columna < columnas - margen)
                    {
                        if (fila >= filas - 2)
                            prefabAUsar = prefabsBloques[indiceMasDuro];
                        else
                            prefabAUsar = prefabsBloques[Random.Range(0, prefabsBloques.Length)];
                    }
                }
                else
                {
                    prefabAUsar = prefabsBloques[Random.Range(0, prefabsBloques.Length)];
                }

                // Instancia el bloque en su coordenada correspondiente y le transfiere la probabilidad de soltar power-ups
                if (prefabAUsar != null)
                {
                    float posX = startX + (columna * separacionX);
                    float posY = inicioY - (fila * separacionY);
                    Vector3 posicionBloque = new Vector3(posX, posY, 0f);

                    GameObject nuevoBloque = Instantiate(prefabAUsar, posicionBloque, Quaternion.identity, transform);

                    ControladorDeBloque scriptBloque = nuevoBloque.GetComponent<ControladorDeBloque>();
                    if (scriptBloque != null)
                    {
                        scriptBloque.ConfigurarProbabilidad(probabilidadPowerUpNivel);
                    }
                }
            }
        }
    }
}