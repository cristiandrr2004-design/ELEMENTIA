using UnityEngine;

// Cámara que sigue al héroe de forma suave. Va en la Main Camera.
public class Mov_camara : MonoBehaviour
{
    public Transform objetivo;                          // arrastra aquí al HERO
    public float velocidadCamara = 5f;                  // más alto = más pegada
    public Vector3 desplazamiento = new Vector3(0, 1, -10);  // Z debe quedar en -10

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + desplazamiento;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadCamara * Time.deltaTime);
    }
}
