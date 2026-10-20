using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Pantalla de muerte: fondo negro, "HAS MUERTO" en rojo.
// R = reiniciar el nivel donde moriste. Escape = cerrar el juego.
// Va en un objeto vacío dentro de la escena "Muerte". Arma la interfaz sola, no necesitas crear Canvas.
// El archivo debe llamarse exactamente "PantallaMuerte.cs".
public class PantallaMuerte : MonoBehaviour
{
    // El enemigo guarda aquí el nombre del nivel antes de cargar esta escena
    public static string nivelAnterior = "";

    [Header("Texto principal")]
    public string titulo = "HAS MUERTO";
    [Tooltip("Arrastra aquí cualquier fuente .ttf u .otf. Vacío = fuente por defecto de Unity")]
    public Font fuente;
    public Color colorTitulo = Color.red;
    public int tamanoTitulo = 120;

    [Header("Instrucciones")]
    public string instrucciones = "Presiona R para reiniciar   |   ESC para salir";
    public Color colorInstrucciones = new Color(0.8f, 0f, 0f);
    public int tamanoInstrucciones = 40;

    [Header("Nivel")]
    [Tooltip("Nivel a cargar con R si no se sabe dónde moriste (por ejemplo, al probar esta escena sola)")]
    public string nivelPorDefecto = "";

    void Start()
    {
        Font f = fuente != null ? fuente : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        if (Camera.main != null)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.black;
        }

        // Canvas
        GameObject canvasGO = new GameObject("Canvas Muerte");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Fondo negro
        GameObject fondo = new GameObject("Fondo");
        fondo.transform.SetParent(canvasGO.transform, false);
        Image img = fondo.AddComponent<Image>();
        img.color = Color.black;
        Estirar(fondo.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);

        CrearTexto(canvasGO.transform, titulo, f, tamanoTitulo, colorTitulo, new Vector2(0, 0.45f), new Vector2(1, 0.75f));
        CrearTexto(canvasGO.transform, instrucciones, f, tamanoInstrucciones, colorInstrucciones, new Vector2(0, 0.25f), new Vector2(1, 0.4f));
    }

    void Update()
    {
        if (TeclaR())
            Reiniciar();
        else if (TeclaEscape())
            Salir();
    }

    void Reiniciar()
    {
        string nivel = !string.IsNullOrEmpty(nivelAnterior) ? nivelAnterior : nivelPorDefecto;
        if (string.IsNullOrEmpty(nivel))
        {
            Debug.LogWarning("PantallaMuerte: no sé qué nivel cargar. Escribe el nombre en 'Nivel Por Defecto'.");
            return;
        }
        SceneManager.LoadScene(nivel);
    }

    void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // en el editor detiene el Play
#else
        Application.Quit();                                 // en el juego exportado lo cierra
#endif
    }

    void CrearTexto(Transform padre, string contenido, Font f, int tamano, Color color, Vector2 anclaMin, Vector2 anclaMax)
    {
        GameObject go = new GameObject(contenido);
        go.transform.SetParent(padre, false);
        Text t = go.AddComponent<Text>();
        t.text = contenido;
        t.font = f;
        t.fontSize = tamano;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        Estirar(go.GetComponent<RectTransform>(), anclaMin, anclaMax);
    }

    void Estirar(RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    bool TeclaR()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }

    bool TeclaEscape()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}
