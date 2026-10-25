using UnityEngine;
using UnityEngine.SceneManagement;

// Escena "Final": ganaste el juego, creditos y boton VOLVER A JUGAR.
// Va en el Canvas de la escena Final.
public class PantallaFinal : MonoBehaviour
{
    // Boton VOLVER A JUGAR: empieza otra vez el Nivel1 con 3 corazones
    public void VolverAJugar()
    {
        Partida.vidas = Partida.vidasIniciales;
        SceneManager.LoadScene("Nivel1");
    }

    // Boton MENU (opcional): vuelve a la escena de inicio
    public void IrAlInicio()
    {
        SceneManager.LoadScene("Inicio");
    }
}
