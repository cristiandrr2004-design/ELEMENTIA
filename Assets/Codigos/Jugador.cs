using UnityEngine;
using UnityEngine.SceneManagement;

// Jugador: camina con flechas o A/D, salta con Espacio.
// Enemigo de lado: pierde 1 corazon y parpadea un momento (no le pueden pegar otra vez).
// Puas o caida: pierde todos los corazones y va a la pantalla de muerte.
// Formas (las da una Runa): Fuego = bola de fuego con F, Piedra = aplasta enemigos, Aire = doble salto.
public class Jugador : MonoBehaviour
{
    [Header("Forma actual")]
    public Forma forma = Forma.Rosa;

    [Header("Color de cada forma")]
    public Color colorRosa = Color.white;                    // blanco = el color original del sprite
    public Color colorFuego = new Color(1f, 0.5f, 0.1f);     // naranja
    public Color colorPiedra = new Color(0.55f, 0.55f, 0.55f); // gris
    public Color colorAire = new Color(0.6f, 0.9f, 1f);      // celeste

    [Header("Fuego")]
    public GameObject bolaFuego;            // el prefab de la bola de fuego
    public float velocidadBola = 10f;
    public float tiempoEntreBolas = 0.4f;   // espera entre disparos

    [Header("Movimiento")]
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
    private bool dobleSaltoListo = false;   // forma Aire: puede saltar otra vez en el aire
    private float proximaBola = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
        rb.gravityScale = gravedad;
        Transformar(forma);   // pone el color de la forma inicial
    }

    void Update()
    {
        if (muerto || Time.timeScale == 0) return;   // muerto o en pausa: no se mueve

        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * FuerzaM, rb.linearVelocity.y);

        if (enSuelo) dobleSaltoListo = true;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (enSuelo)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, FuersaSalto);
            else if (forma == Forma.Aire && dobleSaltoListo)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, FuersaSalto);   // doble salto
                dobleSaltoListo = false;
            }
        }

        // Forma Fuego: tecla F lanza una bola hacia donde mira
        if (forma == Forma.Fuego && Input.GetKeyDown(KeyCode.F) && Time.time > proximaBola)
            LanzarBola();

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

    // La llama la Runa: cambia de forma y de color
    public void Transformar(Forma nueva)
    {
        forma = nueva;
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();

        if (forma == Forma.Rosa) sprite.color = colorRosa;
        if (forma == Forma.Fuego) sprite.color = colorFuego;
        if (forma == Forma.Piedra) sprite.color = colorPiedra;
        if (forma == Forma.Aire) sprite.color = colorAire;
    }

    void LanzarBola()
    {
        if (bolaFuego == null) return;   // falta poner el prefab en el Inspector
        proximaBola = Time.time + tiempoEntreBolas;

        float lado = Mathf.Sign(transform.localScale.x);   // 1 = mira a la derecha, -1 = izquierda
        Vector3 salida = transform.position + new Vector3(lado * 0.7f, 0, 0);

        GameObject bola = Instantiate(bolaFuego, salida, Quaternion.identity);
        bola.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(lado * velocidadBola, 0);
    }

    // La usa la pausa para esconder su boton cuando el Jugador muere
    public bool EstaMuerto()
    {
        return muerto;
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
