using UnityEngine;

public class GeneradorDeMuros : MonoBehaviour
{
    [Header("Dimensiones del Área de Juego")]
    [SerializeField] private float centroX = 0f;
    [SerializeField] private float anchoInterior = 11.6f;
    [SerializeField] private float techoY = 3.2f;
    [SerializeField] private float pisoY = -5.2f;

    [Header("Sprites Específicos para Muros (64x64)")]
    [SerializeField] private Sprite spriteMuroIzquierdo;
    [SerializeField] private Sprite spriteMuroDerecho;
    [SerializeField] private Sprite spriteMuroSuperior;
    [SerializeField] private Sprite spriteEsquinaIzquierda;
    [SerializeField] private Sprite spriteEsquinaDerecha;

    [Header("Físicas y Ajuste de Colisión")]
    [Tooltip("Grosor real de la barra dibujada dentro del sprite 64x64 (aprox 0.22)")]
    [SerializeField] private float grosorRealTubo = 0.22f;
    [SerializeField] private PhysicsMaterial2D materialRebote;
    [SerializeField] private float escalaMuro = 1f;
    [SerializeField] private int orderInLayer = 5;
    [SerializeField] private string tagMuro = "Pared";

    void Awake()
    {
        GenerarMuros();
    }

    public void GenerarMuros()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // Tamaño completo del tile visual (0.64 unidades para 64x64 px)
        float tamanoTile = 0.64f * escalaMuro;
        float grosorCollider = grosorRealTubo * escalaMuro;

        // Bordes donde chocará la paleta y la pelota
        float bordeInternoIzq = centroX - (anchoInterior / 2f);
        float bordeInternoDer = centroX + (anchoInterior / 2f);

        // Posición exacta de los colliders (justo encima de la barra dibujada)
        float xColIzq = bordeInternoIzq - (grosorCollider / 2f);
        float xColDer = bordeInternoDer + (grosorCollider / 2f);
        float yColArr = techoY + (grosorCollider / 2f);

        // Posición visual de los tiles de 64x64 para que el tubo coincida con el collider
        float xVisualIzq = bordeInternoIzq - grosorCollider + (tamanoTile / 2f);
        float xVisualDer = bordeInternoDer + grosorCollider - (tamanoTile / 2f);
        float yVisualArr = techoY + grosorCollider - (tamanoTile / 2f);

        float altoTotal = techoY - pisoY;
        float yCentroVertical = (techoY + pisoY) / 2f;

        // Crear Colliders físicos ajustados al tubo real
        BoxCollider2D colIzq = CrearContenedorPared("ParedIzquierda", new Vector3(xColIzq, yCentroVertical, 0f), new Vector2(grosorCollider, altoTotal));
        BoxCollider2D colDer = CrearContenedorPared("ParedDerecha", new Vector3(xColDer, yCentroVertical, 0f), new Vector2(grosorCollider, altoTotal));
        BoxCollider2D colArr = CrearContenedorPared("ParedArriba", new Vector3(centroX, yColArr, 0f), new Vector2(anchoInterior + (grosorCollider * 2f), grosorCollider));

        // 1. Generar Muros Verticales
        int cantidadV = Mathf.Max(1, Mathf.RoundToInt(altoTotal / tamanoTile));
        float pasoY = altoTotal / cantidadV;
        float escalaY = pasoY / 0.64f;

        for (int i = 0; i < cantidadV; i++)
        {
            float posY = pisoY + (pasoY / 2f) + (i * pasoY);

            if (spriteMuroIzquierdo != null)
                CrearTileVisual(spriteMuroIzquierdo, new Vector3(xVisualIzq, posY, 0f), new Vector3(escalaMuro, escalaY, 1f), colIzq.transform);

            if (spriteMuroDerecho != null)
                CrearTileVisual(spriteMuroDerecho, new Vector3(xVisualDer, posY, 0f), new Vector3(escalaMuro, escalaY, 1f), colDer.transform);
        }

        // 2. Generar Muro Superior Horizontal
        float anchoEntreEsquinas = (xVisualDer - xVisualIzq) - tamanoTile;
        if (spriteMuroSuperior != null && anchoEntreEsquinas > 0)
        {
            int cantidadH = Mathf.Max(1, Mathf.RoundToInt(anchoEntreEsquinas / tamanoTile));
            float pasoX = anchoEntreEsquinas / cantidadH;
            float escalaX = pasoX / 0.64f;
            float inicioX = xVisualIzq + (tamanoTile / 2f);

            for (int i = 0; i < cantidadH; i++)
            {
                float posX = inicioX + (pasoX / 2f) + (i * pasoX);
                CrearTileVisual(spriteMuroSuperior, new Vector3(posX, yVisualArr, 0f), new Vector3(escalaX, escalaMuro, 1f), colArr.transform);
            }
        }

        // 3. Generar Esquinas Superiores (a tamaño real 64x64 sin deformar)
        Vector3 escalaEsquina = new Vector3(escalaMuro, escalaMuro, 1f);
        if (spriteEsquinaIzquierda != null)
            CrearTileVisual(spriteEsquinaIzquierda, new Vector3(xVisualIzq, yVisualArr, 0f), escalaEsquina, colArr.transform);

        if (spriteEsquinaDerecha != null)
            CrearTileVisual(spriteEsquinaDerecha, new Vector3(xVisualDer, yVisualArr, 0f), escalaEsquina, colArr.transform);

        // Configurar límites de la paleta
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.ConfigurarLimites(colIzq, colDer, centroX);
        }
    }

    private BoxCollider2D CrearContenedorPared(string nombre, Vector3 posicion, Vector2 tamanoCollider)
    {
        GameObject obj = new GameObject(nombre);
        obj.transform.SetParent(transform);
        obj.transform.position = posicion;
        obj.tag = tagMuro;

        BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
        col.size = tamanoCollider;
        if (materialRebote != null)
            col.sharedMaterial = materialRebote;

        return col;
    }

    private void CrearTileVisual(Sprite sprite, Vector3 posicion, Vector3 escala, Transform padre)
    {
        GameObject tile = new GameObject("TileMuro");
        tile.transform.SetParent(padre);
        tile.transform.position = posicion;
        tile.transform.localScale = escala;

        SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = orderInLayer;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        float centroY = (techoY + pisoY) / 2f;
        Gizmos.DrawWireCube(new Vector3(centroX, centroY, 0f), new Vector3(anchoInterior, techoY - pisoY, 0f));
    }
}