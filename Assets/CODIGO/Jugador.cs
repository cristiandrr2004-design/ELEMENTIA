using UnityEngine;
using UnityEngine.SceneManagement;

// Jugador: camina con flechas o A/D, salta con Espacio.
// Enemigo de lado: pierde 1 corazon y parpadea un momento (no le pueden pegar otra vez).
// Puas o caida: pierde todos los corazones y va a la pantalla de muerte.
public class Jugador : MonoBehaviour
{
    public float FuerzaM = 6f;              // velocidad al caminar
    public float FuersaSalto = 12f;         // fuerza del salto
    public float gravedad = 3f;             // mas alto = cae mas rapido
    public float esperaMuerte = 1f;         // segundos antes de la pantalla de muerte
    public float limiteCaida = -10f;        // si baja de esta altura Y, muere
    public float tiempoInvencible = 1f;     // segundos sin recibir dano despues de un golpe
    public float saltoAlGolpe = 6f;         // pequeno salto al recibir un golpe
    public bool enSuelo = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private bool muerto = false;
    private float invencibleHasta = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
        rb.gravityScale = gravedad;
    }

    void Update()
    {
        if (muerto || Time.timeScale == 0) return;   // muerto o en pausa: no se mueve

        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * FuerzaM, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, FuersaSalto);

        // Voltear hacia donde camina
        if (x != 0)
            transform.localScale = new Vector3(Mathf.Sign(x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);

        if (animator != null)
        {
            animator.SetBool("Movimiento", x != 0);
            animator.SetBool("SALTO", !enSuelo);
        }

        // Parpadeo mientras es invencible
        if (sprite != null)
            sprite.enabled = Time.time > invencibleHasta || Mathf.Repeat(Time.time * 10, 1) > 0.5f;

        // Se cayo del mapa
        if (transform.position.y < limiteCaida)
            Morir();
    }

    // Solo cuenta como suelo lo que esta debajo (no paredes)
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.GetContact(0).normal.y > 0.5f)
            enSuelo = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        enSuelo = false;
    }

    // La llama el enemigo cuando te toca de lado: quita 1 corazon
    public void RecibirGolpe()
    {
        if (muerto || Time.time < invencibleHasta) return;   // ya muerto o todavia parpadeando

        Partida.vidas = Partida.vidas - 1;

        if (Partida.vidas <= 0)
        {
            Morir();
            return;
        }

        invencibleHasta = Time.time + tiempoInvencible;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, saltoAlGolpe);
    }

    // La llaman las puas y la caida: quita todos los corazones
    public void Morir()
    {
        if (muerto) return;
        muerto = true;
        Partida.vidas = 0;
        Partida.nivelActual = SceneManager.GetActiveScene().name;   // para reintentar este nivel

        if (sprite != null) sprite.enabled = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (animator != null) animator.SetTrigger("MUERTE");
        Invoke("IrAPantallaMuerte", esperaMuerte);
    }

    void IrAPantallaMuerte()
    {
        SceneManager.LoadScene("Muerte");
    }
}
