using UnityEngine;

// Bola de fuego: vuela recto. Mata a los enemigos normales.
// A los acorazados no les hace nada. Se rompe al chocar con algo.
public class BolaFuego : MonoBehaviour
{
    public float duracion = 3f;   // segundos antes de desaparecer sola

    void Start()
    {
        Destroy(gameObject, duracion);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player")) return;   // no se quema el Jugador
        if (otro.isTrigger) return;              // ignora agua, runas, metas

        Enemigos enemigo = otro.GetComponent<Enemigos>();
        if (enemigo != null && !enemigo.acorazado)
            enemigo.Morir();                     // enemigo normal: muere con animacion

        Destroy(gameObject);                     // la bola se rompe (piso, pared o acorazado)
    }
}
