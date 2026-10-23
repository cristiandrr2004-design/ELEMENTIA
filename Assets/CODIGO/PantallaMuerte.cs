using UnityEngine;
using UnityEngine.SceneManagement;

// Pantalla de muerte: REINTENTAR vuelve al nivel donde moriste con 3 corazones.
// INICIO vuelve al menu. Tambien puedes reintentar con la tecla R.
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
}
