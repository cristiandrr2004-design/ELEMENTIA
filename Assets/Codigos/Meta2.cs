using UnityEngine;
using UnityEngine.SceneManagement;

// Meta final (puerta del Nivel2): al tocarla el Jugador gana el juego
// y pasa a la escena "Final".
// Necesita un Collider2D con "Is Trigger" marcado.
public class Meta2 : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            SceneManager.LoadScene("Final");
        }
    }
}