using UnityEngine;
using UnityEngine.SceneManagement;

// Enemigo para Unity 6:
// - Persigue al jugador (solo izquierda/derecha) cuando está dentro del radio de detección.
// - Si el jugador le cae encima, el enemigo se destruye y el jugador rebota.
// - Si el jugador lo toca de lado o por debajo, el jugador muere.
// El archivo debe llamarse exactamente "ENEMIGOS.cs". Va en el cubo del enemigo.
[RequireComponent(typeof(Rigidbody2D))]
public class ENEMIGOS : MonoBehaviour
{
    [Header("Persecución")]
    [Tooltip("El jugador. Si lo dejas vacío, busca el objeto con el Tag 'Player'")]
    public Transform player;

    [Tooltip("Distancia a la que el enemigo ve al jugador y empieza a seguirlo")]
    public float detectionRadius = 5f;

    [Tooltip("Velocidad del enemigo en unidades por segundo")]
    public float speed = 2f;

    [Tooltip("Si el jugador está casi encima en X, el enemigo se queda quieto para no temblar")]
    public float zonaMuerta = 0.1f;

    [Header("Patrulla cuando no ve al jugador (opcional)")]
    [Tooltip("Si está desactivado, el enemigo se queda quieto cuando no ve al jugador (como en tu captura)")]
    public bool patrullar = false;

    [Tooltip("Cuánto se aleja hacia cada lado desde donde empieza")]
    public float distanciaPatrulla = 3f;

    [Header("Pisotón")]
    [Tooltip("Qué tan alto rebota el jugador al aplastar al enemigo")]
    public float reboteJugador = 10f;

    [Tooltip("Margen para decidir si el jugador cayó por encima")]
    public float margenPisoton = 0.2f;

    private Rigidbody2D rb;
    private Collider2D miCollider;
    private Vector2 movement;
    private float inicioX;
    private int direccionPatrulla = 1;
    private float escalaX;
    private bool muerto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        miCollider = GetComponent<Collider2D>();
        inicioX = transform.position.x;
        escalaX = Mathf.Abs(transform.localScale.x);

        if (player == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
                player = jugador.transform;
        }
    }

    void Update()
    {
        movement = Vector2.zero;

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer < detectionRadius)
            {
                float diferenciaX = player.position.x - transform.position.x;
                if (Mathf.Abs(diferenciaX) > zonaMuerta)
                    movement = new Vector2(Mathf.Sign(diferenciaX), 0);
                return;
            }
        }

        if (patrullar)
        {
            if (transform.position.x > inicioX + distanciaPatrulla) direccionPatrulla = -1;
            if (transform.position.x < inicioX - distanciaPatrulla) direccionPatrulla = 1;
            movement = new Vector2(direccionPatrulla, 0);
        }
    }

    void FixedUpdate()
    {
        // Mueve con velocidad para que la gravedad siga funcionando
        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);

        if (movement.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = movement.x > 0 ? escalaX : -escalaX;
            transform.localScale = escala;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (muerto || !collision.gameObject.CompareTag("Player")) return;

        Collider2D colJugador = collision.collider;
        Rigidbody2D rbJugador = collision.rigidbody;

        // ¿Los pies del jugador están a la altura de la parte de arriba del enemigo?
        bool cayoEncima = colJugador.bounds.min.y >= miCollider.bounds.max.y - margenPisoton;

        if (cayoEncima)
        {
            muerto = true;
            if (rbJugador != null)
                rbJugador.linearVelocity = new Vector2(rbJugador.linearVelocity.x, reboteJugador);
            Destroy(gameObject);
        }
        else
        {
            MatarJugador(collision.gameObject);
        }
    }

    void MatarJugador(GameObject jugador)
    {
        // El héroe hace su animación de muerte y luego pasa a la pantalla de muerte
        s1 heroe = jugador.GetComponent<s1>();
        if (heroe != null)
            heroe.Morir();
        else
            Destroy(jugador);
    }

    // Dibuja el radio de detección y la zona de patrulla en la vista Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (patrullar)
        {
            float x = Application.isPlaying ? inicioX : transform.position.x;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(x - distanciaPatrulla, transform.position.y, 0),
                            new Vector3(x + distanciaPatrulla, transform.position.y, 0));
        }
    }
}
