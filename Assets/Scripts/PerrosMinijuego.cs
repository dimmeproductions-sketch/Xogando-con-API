using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LogicaPerros : MonoBehaviour
{
    public RawImage contenedorImagen;
    public TMP_Dropdown dropdownRazas;
    public string razaCorrecta; // Aquí guardaremos el nombre formateado (ej: "afghan hound")
    public TextMeshProUGUI textoResultado;
    private string urlApi = "https://dog.ceo/api/breeds/image/random";

    void Start()
    {
        CargarNuevoPerro();
    }

    public void CargarNuevoPerro()
    {
        BorrarResultado();
        StopAllCoroutines();
        StartCoroutine(ObtenerPerro());
    }

    IEnumerator ObtenerPerro()
    {
        using (UnityWebRequest solicitudJson = UnityWebRequest.Get(urlApi))
        {
            yield return solicitudJson.SendWebRequest();

            if (solicitudJson.result == UnityWebRequest.Result.Success)
            {
                RespuestaImagen datos = JsonUtility.FromJson<RespuestaImagen>(solicitudJson.downloadHandler.text);
                string urlImagen = datos.message;

                // 1. Extraemos el texto de la URL (ej: "hound-afghan" o "beagle")
                string[] partes = urlImagen.Split('/');
                string razaCruda = partes[4];

                // 2. FORMATEO: Convertimos "raza-subraza" a "subraza raza"
                razaCorrecta = FormatearNombreRaza(razaCruda);

                yield return StartCoroutine(DescargarImagen(urlImagen));
            }
        }
    }

    // Función auxiliar para que el formato coincida con el Dropdown
    string FormatearNombreRaza(string texto)
    {
        if (texto.Contains("-"))
        {
            // Si contiene "-", es compuesta: ej "hound-afghan"
            string[] subPartes = texto.Split('-');
            string razaPrincipal = subPartes[0];
            string subRaza = subPartes[1];
            // Devolvemos "afghan hound" para que coincida con el Dropdown
            return subRaza + " " + razaPrincipal;
        }
        else
        {
            // Si no tiene "-", es simple: ej "beagle"
            return texto;
        }
    }

    IEnumerator DescargarImagen(string url)
    {
        using (UnityWebRequest solicitudImagen = UnityWebRequestTexture.GetTexture(url))
        {
            yield return solicitudImagen.SendWebRequest();

            if (solicitudImagen.result == UnityWebRequest.Result.Success)
            {
                Texture2D textura = DownloadHandlerTexture.GetContent(solicitudImagen);
                contenedorImagen.texture = textura;
            }
        }
    }

    // Este es el método que debes conectar al evento OnValueChanged del Dropdown
    public void ComprobarRespuestaAutomatica(int indice)
    {
        // 1. Limpiamos el texto del usuario (quitamos espacios y pasamos a minúsculas)
        string seleccionUsuario = dropdownRazas.options[indice].text.ToLower().Trim();

        // 2. Limpiamos la respuesta correcta (por si acaso quedó algún espacio)
        string respuestaLimpia = razaCorrecta.ToLower().Trim();

        // LOG DE SEGURIDAD: Esto te permite ver en la consola qué está pasando exactamente
        Debug.Log("Comparando: [" + seleccionUsuario + "] con [" + respuestaLimpia + "]");

        if (seleccionUsuario == respuestaLimpia)
        {
            textoResultado.text = "<color=green>¡CORRECTO!</color> Es un " + razaCorrecta;
        }
        else
        {
            textoResultado.text = "<color=red>FALLASTE.</color> Era un " + razaCorrecta;
        }
    }

    public void BorrarResultado()
    {
        if (textoResultado != null) textoResultado.text = "Adivina la raza...";
        if (dropdownRazas != null) dropdownRazas.value = 0;
    }
}