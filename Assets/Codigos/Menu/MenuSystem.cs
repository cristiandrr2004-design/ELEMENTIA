using UnityEngine;
using UnityEngine.SceneManagement;

// Menu de inicio: boton Jugar (va al Nivel1) y boton Salir (cierra el juego).
// Va en el objeto "Menu" de la escena Inicio.
public class MenuSystem : MonoBehaviour
{
    // Boton JUGAR
    public void Jugar()
    {
        Partida.vidas = Partida.vidasIniciales;   // partida nueva con 3 corazones
        SceneManager.LoadScene("Nivel1");
    }

    // Boton SALIR
    public void Salir()
    {
        Debug.Log("Saliendo del juego...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // en Unity: detiene el Play
#else
        Application.Quit();                                // en el juego exportado: cierra la ventana
#endif
    }
}
