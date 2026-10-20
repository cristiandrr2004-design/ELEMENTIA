using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Movimiento del héroe para Unity 6: izquierda/derecha, salto y animaciones.
// El archivo debe llamarse exactamente "s1.cs" (igual que la clase).
// Funciona con el Input System nuevo o con el Input Manager clásico, sin cambiar nada.
[RequireComponent(typeof(Rigidbody2D))]
public class s1 : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad horizontal en unidades por segundo")]
    public float FuerzaM = 6f;

    [Tooltip("Velocidad hacia arriba al saltar")]
    public float FuersaSalto = 12f;

    [Tooltip("Si está activado, el personaje se voltea hacia donde camina")]
    public bool voltearAlCaminar = true;

    [Header("Gravedad y caída")]
    [Tooltip("Gravedad normal del personaje (1 = gravedad de Unity). Más alto = cae más rápido")]
    public float gravedad = 3f;

    [Tooltip("Multiplica la gravedad solo mientras cae. 1 = igual que al subir, 2 = cae el doble de fuerte")]
    public float multiplicadorCaida = 2f;

    [Tooltip("Velocidad máxima de caída, para que no caiga infinitamente rápido")]
    public float velocidadMaxCaida = 20f;

    [Header("Suelo")]
    [Tooltip("Solo lectura: indica si el personaje está pisando algo")]
    public bool enSuelo = false;

    [Tooltip("Qué tan plana debe ser una superficie para contar como suelo (0.5 = hasta 60° de inclinación)")]
    [Range(0f, 1f)]
    public float inclinacionSuelo = 0.5f;

    [Header("Animación (opcional)")]
    [Tooltip("Puedes dejarlo vacío mientras el personaje sea un cubo")]
    public Animator animator;

    [Tooltip("Nombre del parámetro Bool que se activa al caminar")]
    public string paramMovimiento = "Movimiento";

    [Tooltip("Nombre del parámetro Bool que se activa mientras está en el aire")]
    public string paramSalto = "SALTO";

    [Tooltip("Nombre del parámetro Trigger que se dispara al morir")]
    public string paramMuerte = "MUERTE";

    [Header("Muerte")]
    [Tooltip("Segundos que espera para que se vea la animación de muerte antes de ir a la pantalla de muerte")]
    public float esperaAntesDePantallaMuerte = 1.5f;

    [Tooltip("Nombre de la escena de muerte. Vacío = reinicia el nivel")]
    public string escenaMuerte = "Muerte";

    [Tooltip("Solo lectura: indica si el personaje ya murió")]
    public bool muerto = false;

    private Rigidbody2D rb;
    private float entradaHorizontal;
    private bool saltoPedido;
    private bool tocandoSuelo;
    private float escalaX = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;                                   // el cubo no rueda
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;    // movimiento suave en pantalla
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;             // para que la detección de suelo no se apague

        if (animator == null)
            animator = GetComponent<Animator>();

        escalaX = Mathf.Abs(transform.localScale.x);
    }

    void Update()
    {
        if (muerto) return;

        entradaHorizontal = LeerHorizontal();

        if (LeerSalto() && enSuelo)
            saltoPedido = true;
    }

    void FixedUpdate()
    {
        if (muerto) return;

        // Lo que detectaron las colisiones del paso anterior
        enSuelo = tocandoSuelo;
        tocandoSuelo = false;

        Vector2 velocidad = rb.linearVelocity;
        velocidad.x = entradaHorizontal * FuerzaM;

        if (saltoPedido)
        {
            velocidad.y = FuersaSalto;
            enSuelo = false;
            saltoPedido = false;
        }

        // Más gravedad al caer que al subir: el salto se siente menos "flotante"
        rb.gravityScale = velocidad.y < 0 ? gravedad * multiplicadorCaida : gravedad;

        if (velocidad.y < -velocidadMaxCaida)
            velocidad.y = -velocidadMaxCaida;

        rb.linearVelocity = velocidad;

        Voltear();
        ActualizarAnimaciones();
    }

    void Voltear()
    {
        if (!voltearAlCaminar || entradaHorizontal == 0) return;

        Vector3 escala = transform.localScale;
        escala.x = entradaHorizontal > 0 ? escalaX : -escalaX;      // conserva el tamaño que le diste
        transform.localScale = escala;
    }

    void ActualizarAnimaciones()
    {
        PonerParametro(paramMovimiento, entradaHorizontal != 0);
        PonerParametro(paramSalto, !enSuelo);
    }

    // La llama el enemigo cuando toca al personaje de lado
    public void Morir()
    {
        if (muerto) return;
        muerto = true;

        entradaHorizontal = 0;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;   // se queda quieto donde murió mientras se ve la animación

        PonerParametro(paramMovimiento, false);
        PonerParametro(paramSalto, false);
        PonerParametro(paramMuerte, true);

        StartCoroutine(IrAPantallaMuerte());
    }

    IEnumerator IrAPantallaMuerte()
    {
        yield return new WaitForSeconds(esperaAntesDePantallaMuerte);

        string nivel = SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(escenaMuerte))
        {
            PantallaMuerte.nivelAnterior = nivel;   // para que R vuelva a este nivel
            SceneManager.LoadScene(escenaMuerte);
        }
        else
        {
            SceneManager.LoadScene(nivel);
        }
    }

    // Funciona sin importar si el parámetro es Bool, Int, Float o Trigger. Si no existe, no hace nada.
    void PonerParametro(string nombre, bool valor)
    {
        if (animator == null || string.IsNullOrEmpty(nombre)) return;

        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name != nombre) continue;

            switch (p.type)
            {
                case AnimatorControllerParameterType.Bool: animator.SetBool(nombre, valor); break;
                case AnimatorControllerParameterType.Int: animator.SetInteger(nombre, valor ? 1 : 0); break;
                case AnimatorControllerParameterType.Float: animator.SetFloat(nombre, valor ? 1f : 0f); break;
                case AnimatorControllerParameterType.Trigger: if (valor) animator.SetTrigger(nombre); break;
            }
            return;
        }
    }

    // Solo cuenta como suelo lo que está debajo del personaje (no paredes ni techos)
    void OnCollisionStay2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y >= inclinacionSuelo)
            {
                tocandoSuelo = true;
                return;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionStay2D(collision);
    }

    // Flechas o A/D
    float LeerHorizontal()
    {
        float x = 0f;
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        if (k == null) return 0f;
        if (k.leftArrowKey.isPressed || k.aKey.isPressed) x -= 1f;
        if (k.rightArrowKey.isPressed || k.dKey.isPressed) x += 1f;
#else
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) x += 1f;
#endif
        return x;
    }

    // Espacio
    bool LeerSalto()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Space);
#endif
    }
}

