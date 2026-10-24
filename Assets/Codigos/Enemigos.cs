using UnityEngine;

// Enemigo: camina de un lado a otro y persigue al Jugador cuando lo ve.
// Si el Jugador en forma de PIEDRA le cae encima, el enemigo muere (normal o acorazado).
// En cualquier otro caso, el Jugador pierde 1 corazon.
// Marca "Acorazado" en el Inspector para que las bolas de fuego no le hagan nada.
public class Enemigos : MonoBehaviour
{
    public bool acorazado = false;        // true = inmune a las bolas de fuego
    public float speed = 2f;              // velocidad
    public float detectionRadius = 5f;    // distancia a la que ve al Jugador
    public float distanciaPatrulla = 3f;  // cuanto camina a cada lado
    public float rebote = 10f;            // salto del Jugador al aplastarlo

    private Transform jugador;
    private Rigidbody2D rb;
    private float inicioX;
    private float direccion = 1;          // 1 = derecha, -1 = izquierda

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        inicioX = transform.position.x;
        jugador = GameObject.FindGameObjectWithTag("Player").transform;   // busca al objeto con Tag Player
    }

    void Update()
    {
        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia < detectionRadius)
            direccion = Mathf.Sign(jugador.position.x - transform.position.x);   // perseguir
        else if (transform.position.x > inicioX + distanciaPatrulla)
            direccion = -1;                                                       // dar la vuelta
        else if (transform.position.x < inicioX - distanciaPatrulla)
            direccion = 1;                                                        // dar la vuelta

        rb.linearVelocity = new Vector2(direccion * speed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D col) { Tocar(col); }
    void OnCollisionStay2D(Collision2D col) { Tocar(col); }   // si sigue pegado, vuelve a pegar cuando deja de parpadear

    void Tocar(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        Jugador j = col.gameObject.GetComponent<Jugador>();

        // Los pies del Jugador estan por encima del enemigo?
        bool cayoEncima = col.collider.bounds.min.y > GetComponent<Collider2D>().bounds.max.y - 0.2f;

        if (cayoEncima && j.forma == Forma.Piedra)
        {
            col.rigidbody.linearVelocity = new Vector2(col.rigidbody.linearVelocity.x, rebote);
            Destroy(gameObject);
        }
        else
        {
            j.RecibirGolpe();   // quita 1 corazon
        }
    }
}

