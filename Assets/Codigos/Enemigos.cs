using UnityEngine;

// Enemigo: camina de un lado a otro y persigue al Jugador cuando lo ve.
// Si el Jugador en forma de PIEDRA le cae encima, el enemigo muere (normal o acorazado).
// En cualquier otro caso, el Jugador pierde 1 corazon.
// Marca "Acorazado" en el Inspector para que las bolas de fuego no le hagan nada.
// Animator: parametro Trigger "Muerte" para la animacion de morir.
public class Enemigos : MonoBehaviour
{
    public bool acorazado = false;        // true = inmune a las bolas de fuego
    public float speed = 2f;              // velocidad
    public float detectionRadius = 5f;    // distancia a la que ve al Jugador
    public float distanciaPatrulla = 3f;  // cuanto camina a cada lado
    public float rebote = 10f;            // salto del Jugador al aplastarlo
    public float duracionMuerte = 0.8f;   // segundos que dura la animacion de morir
    public bool dibujoMiraDerecha = true; // como esta dibujado el sprite original

    private Transform jugador;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private float inicioX;
    private float direccion = 1;          // 1 = derecha, -1 = izquierda
    private bool muerto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
        inicioX = transform.position.x;
        jugador = GameObject.FindGameObjectWithTag("Player").transform;   // busca al objeto con Tag Player
    }

    void Update()
    {
        if (muerto) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia < detectionRadius)
            direccion = Mathf.Sign(jugador.position.x - transform.position.x);   // perseguir
        else if (transform.position.x > inicioX + distanciaPatrulla)
            direccion = -1;                                                       // dar la vuelta
        else if (transform.position.x < inicioX - distanciaPatrulla)
            direccion = 1;                                                        // dar la vuelta

        rb.linearVelocity = new Vector2(direccion * speed, rb.linearVelocity.y);

        // Mirar hacia donde camina
        if (sprite != null)
            sprite.flipX = dibujoMiraDerecha ? direccion < 0 : direccion > 0;
    }

    void OnCollisionEnter2D(Collision2D col) { Tocar(col); }
    void OnCollisionStay2D(Collision2D col) { Tocar(col); }   // si sigue pegado, vuelve a pegar cuando deja de parpadear

    void Tocar(Collision2D col)
    {
        if (muerto) return;
        if (!col.gameObject.CompareTag("Player")) return;

        Jugador j = col.gameObject.GetComponent<Jugador>();

        // Los pies del Jugador estan por encima del enemigo?
        bool cayoEncima = col.collider.bounds.min.y > GetComponent<Collider2D>().bounds.max.y - 0.2f;

        if (cayoEncima && j.forma == Forma.Piedra)
        {
            col.rigidbody.linearVelocity = new Vector2(col.rigidbody.linearVelocity.x, rebote);
            Morir();
        }
        else
        {
            j.RecibirGolpe();   // quita 1 corazon
        }
    }

    // La llaman el Jugador de piedra y la bola de fuego
    public void Morir()
    {
        if (muerto) return;
        muerto = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;      // ya no cae ni empuja
        GetComponent<Collider2D>().enabled = false;   // ya no hace dano

        if (animator != null) animator.SetTrigger("Muerte");
        Destroy(gameObject, duracionMuerte);          // desaparece cuando termina la animacion
    }
}
