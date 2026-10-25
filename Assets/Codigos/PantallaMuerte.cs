using UnityEngine;
using UnityEngine.SceneManagement;

// Pantalla de muerte: REINTENTAR vuelve al nivel donde moriste con 3 corazones.
// INICIO vuelve al menu. SALIR cierra el juego. Tambien puedes reintentar con la tecla R.
// Va en el Canvas de la escena "Muerte".
public class PantallaMuerte : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Reintentar();
    }

    public void Reintentar()
    {
        Partida.vidas = Partida.vidasIniciales;
        SceneManager.LoadScene(Partida.nivelActual);
    }

    public void IrAlInicio()
    {
        SceneManager.LoadScene("Inicio");
    }

    // Cierra el juego (en Unity solo detiene el Play)
    public void Salir()
    {
        Debug.Log("Salir del juego");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
