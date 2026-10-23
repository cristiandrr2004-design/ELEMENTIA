using UnityEngine;

// Corazon de agua: al tocarlo el Jugador gana 1 corazon, hasta un maximo de 5.
// Necesita un Collider2D con "Is Trigger" marcado.
public class Agua : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            if (Partida.vidas < Partida.vidasMaximas)
                Partida.vidas = Partida.vidas + 1;

            Destroy(gameObject);
        }
    }
}
