using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

// Fundido a negro al cambiar de escena.
// Va en el objeto Transicion (Canvas con una Image negra que cubre la pantalla).
public class Transicion : MonoBehaviour
{
    public Image pantallaNegra;
    public float duracion = 1f;
    public bool aclararAlEmpezar = true;   // desmarcalo en Inicio

    void Start()
    {
        if (aclararAlEmpezar)
        {
            StartCoroutine(Aclarar());
        }
        else
        {
            pantallaNegra.color = new Color(0, 0, 0, 0);
            pantallaNegra.raycastTarget = false;
        }
    }

    // Se llama desde el OnClick de un boton, escribiendo el nombre de la escena
    public void IrA(string escena)
    {
        StartCoroutine(Oscurecer(escena));
    }

    // De negro a transparente
    IEnumerator Aclarar()
    {
        for (float t = duracion; t > 0; t -= Time.unscaledDeltaTime)
        {
            pantallaNegra.color = new Color(0, 0, 0, t / duracion);
            yield return null;
        }
        pantallaNegra.color = new Color(0, 0, 0, 0);
        pantallaNegra.raycastTarget = false;
    }

    // De transparente a negro, y luego cambia de escena
    IEnumerator Oscurecer(string escena)
    {
        pantallaNegra.raycastTarget = true;
        for (float t = 0; t < duracion; t += Time.unscaledDeltaTime)
        {
            pantallaNegra.color = new Color(0, 0, 0, t / duracion);
            yield return null;
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(escena);
    }
}