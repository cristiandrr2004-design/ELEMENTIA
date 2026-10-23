using UnityEngine;
using UnityEngine.UI;

// Muestra los corazones del Jugador (de 1 a 5).
// Va en el Canvas del nivel. Necesita 5 Images de corazon.
public class HUD : MonoBehaviour
{
    public Image[] corazones;   // arrastra aqui los 5 corazones, en orden

    void Update()
    {
        for (int i = 0; i < corazones.Length; i++)
        {
            // se ven solo los corazones que tiene el Jugador
            corazones[i].enabled = i < Partida.vidas;
        }
    }
}
