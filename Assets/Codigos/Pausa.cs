using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// Pausa: el boton de pausa se ve siempre durante el juego (menos cuando el Jugador muere).
// Al pausar: el juego se congela y aparece el panel negro transparente con
// REANUDAR, REINICIAR y MENU. Tambien se pausa con la tecla Escape.
// Va en el Canvas del nivel (el mismo de los corazones).
public class Pausa : MonoBehaviour
{
    public GameObject menuPausa;    // el panel negro con los 3 botones
    public GameObject botonPausa;   // el boton de pausa de la esquina

    private Jugador jugador;

    void Start()
    {
        menuPausa.SetActive(false);
        botonPausa.SetActive(true);
        Time.timeScale = 1;
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Jugador>();
    }

    void Update()
    {
        // Si el Jugador murio, se esconde el boton y no se puede pausar
        if (jugador.EstaMuerto())
        {
            botonPausa.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuPausa.activeSelf) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        menuPausa.SetActive(true);
        botonPausa.SetActive(false);                        // mientras esta en pausa no se ve
        Time.timeScale = 0;                                 // 0 = el juego se congela
        EventSystem.current.SetSelectedGameObject(null);    // para que Espacio no presione el boton
    }

    public void Reanudar()
    {
        menuPausa.SetActive(false);
        botonPausa.SetActive(true);
        Time.timeScale = 1;                                 // 1 = el juego sigue
        EventSystem.current.SetSelectedGameObject(null);
    }

    // Reinicia el nivel en el que estas (Nivel1 o Nivel2) con 3 corazones
    public void Reiniciar()
    {
        Time.timeScale = 1;
        Partida.vidas = Partida.vidasIniciales;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Vuelve a la primera escena (Inicio)
    public void IrAlInicio()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Inicio");
    }
}
