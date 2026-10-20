using UnityEngine;

// Cámara que sigue al personaje de forma suave en 2D.
// Va en la Main Camera. El archivo debe llamarse exactamente "Mov_camara.cs".
public class Mov_camara : MonoBehaviour
{
    [Tooltip("El personaje a seguir. Si lo dejas vacío, busca el objeto con el Tag 'Player'")]
    public Transform objetivo;

    [Tooltip("Qué tan rápido alcanza la cámara al personaje. Más alto = más pegada")]
    public float velocidadCamara = 5f;

    [Tooltip("Corrimiento respecto al personaje. Ej: Y = 1 para ver un poco más arriba")]
    public Vector3 desplazamiento = new Vector3(0f, 1f, 0f);

    [Tooltip("Si se desactiva, la cámara solo sigue al personaje de izquierda a derecha")]
    public bool seguirVertical = true;

    [Header("Límites del nivel (opcional)")]
    public bool usarLimites = false;
    public float limiteIzquierdo = -10f;
    public float limiteDerecho = 50f;
    public float limiteAbajo = -5f;
    public float limiteArriba = 10f;

    private float alturaInicial;
    private float profundidad;   // la Z de la cámara, para que siempre vea la escena

    void Start()
    {
        profundidad = transform.position.z;
        alturaInicial = transform.position.y;

        if (objetivo == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
                objetivo = jugador.transform;
        }
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + desplazamiento;
        if (!seguirVertical)
            posicionDeseada.y = alturaInicial;

        if (usarLimites)
        {
            posicionDeseada.x = Mathf.Clamp(posicionDeseada.x, limiteIzquierdo, limiteDerecho);
            posicionDeseada.y = Mathf.Clamp(posicionDeseada.y, limiteAbajo, limiteArriba);
        }

        posicionDeseada.z = profundidad;

        // Suavizado igual en cualquier computador, sin importar los FPS
        float t = 1f - Mathf.Exp(-velocidadCamara * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, t);
    }
}
