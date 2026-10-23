using UnityEngine;

// Escena de inicio: empieza una partida nueva con 3 vidas.
// Va en cualquier objeto de la escena "Inicio" (por ejemplo el Canvas).
// El botón JUGAR no necesita script: en su OnClick llama a Transicion.IrA("Nivel1").
public class MenuInicio : MonoBehaviour
{
    void Start()
    {
        Partida.vidas = 3;
    }
}
