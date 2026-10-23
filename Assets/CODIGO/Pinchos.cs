using UnityEngine;

// Puas: apenas el heroe las toca, pierde una vida y va a la pantalla de muerte.
// Funciona en un objeto suelto o en un Tilemap de puas.
// Necesita un collider (Box Collider 2D o Tilemap Collider 2D).
public class Pinchos : MonoBehaviour
{
    // Si el collider tiene "Is Trigger" marcado
    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
            otro.GetComponent<Jugador>().Morir();
    }

    // Si el collider NO tiene "Is Trigger" marcado
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            col.gameObject.GetComponent<Jugador>().Morir();
    }
}

