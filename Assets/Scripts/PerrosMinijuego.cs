using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // Necesario para mostrar la imagen en la interfaz
using System.Collections;

public class LogicaPerros : MonoBehaviour
{
    public RawImage contenedorImagen; // Arrastra aquí un RawImage de tu UI
    private string urlApi = "https://dog.ceo/api/breeds/image/random";

    void Start()
    {
        // Primera carga al empezar
        CargarNuevoPerro();
    }

    // Método que llamarás desde un botón de la UI
    public void CargarNuevoPerro()
    {
        StopAllCoroutines(); // Evita que se solapen si el usuario pulsa muy rápido
        StartCoroutine(ObtenerPerrito());
    }

    IEnumerator ObtenerPerrito()
    {
        // 1. Llamada a la API para obtener el JSON
        using (UnityWebRequest solicitudJson = UnityWebRequest.Get(urlApi))
        {
            yield return solicitudJson.SendWebRequest();

            if (solicitudJson.result == UnityWebRequest.Result.Success)
            {
                RespuestaImagen datos = JsonUtility.FromJson<RespuestaImagen>(solicitudJson.downloadHandler.text);
                string urlImagen = datos.message;

                // 2. Llamada multimedia para descargar la imagen (Textura)
                yield return StartCoroutine(DescargarImagen(urlImagen));
            }
        }
    }

    IEnumerator DescargarImagen(string url)
    {
        using (UnityWebRequest solicitudImagen = UnityWebRequestTexture.GetTexture(url))
        {
            yield return solicitudImagen.SendWebRequest();

            if (solicitudImagen.result == UnityWebRequest.Result.Success)
            {
                // Convertimos la descarga en una textura y la aplicamos al RawImage
                Texture2D textura = DownloadHandlerTexture.GetContent(solicitudImagen);
                contenedorImagen.texture = textura;
            }
        }
    }
}