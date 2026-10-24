using UnityEngine;

// Runa: cuando el Jugador la toca, se transforma en la forma elegida y la runa desaparece.
// En el Inspector elige la forma: Fuego, Piedra o Aire.
public class Runa : MonoBehaviour
{
    public Forma forma = Forma.Fuego;

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            otro.GetComponent<Jugador>().Transformar(forma);
            Destroy(gameObject);
        }
    }
}
