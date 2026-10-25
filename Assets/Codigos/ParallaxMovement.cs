using UnityEngine;

// Parallax: el fondo sigue a la camara y cada capa (hija) se desliza a distinta velocidad.
// Cada hija es un Quad con su imagen. La capa con Z mas grande es la mas lejana y se mueve mas lento.
public class ParallaxMovement : MonoBehaviour
{
    [Range(0.01f, 1f)]
    public float parallaxSpeed = 0.1f;   // velocidad general del efecto
    public float alturaY = 0f;           // sube o baja todo el fondo respecto a la camara

    private Transform cam;
    private float camInicioX;
    private Material[] mat;
    private float[] velocidad;

    void Start()
    {
        cam = Camera.main.transform;
        camInicioX = cam.position.x;

        int cantidad = transform.childCount;
        mat = new Material[cantidad];
        velocidad = new float[cantidad];

        // Buscar la capa mas lejana (la de Z mas grande)
        float masLejos = 0;
        for (int i = 0; i < cantidad; i++)
        {
            float z = transform.GetChild(i).position.z - cam.position.z;
            if (z > masLejos) masLejos = z;
        }

        // Lejos = lento, cerca = rapido
        for (int i = 0; i < cantidad; i++)
        {
            Transform capa = transform.GetChild(i);
            mat[i] = capa.GetComponent<Renderer>().material;
            float z = capa.position.z - cam.position.z;
            velocidad[i] = 1 - z / masLejos;
        }
    }

    void LateUpdate()
    {
        // El fondo va pegado a la camara (no se le acaba la imagen)
        transform.position = new Vector3(cam.position.x, cam.position.y + alturaY, transform.position.z);

        // Cada capa corre su imagen segun cuanto se movio la camara
        float distancia = cam.position.x - camInicioX;
        for (int i = 0; i < mat.Length; i++)
            mat[i].mainTextureOffset = new Vector2(distancia * velocidad[i] * parallaxSpeed, 0);
    }
}
