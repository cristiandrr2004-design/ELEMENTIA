using UnityEngine;
using UnityEngine.SceneManagement;

// Meta al final del nivel: al tocarla cambia de escena.
// En el Nivel1 escribe "Nivel2". En el Nivel2 escribe "Final".
// Necesita un Collider2D con "Is Trigger" marcado.
public class Meta : MonoBehaviour
{
    public string siguienteEscena = "Nivel2";

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            SceneManager.LoadScene(siguienteEscena);
        }
    }
}
