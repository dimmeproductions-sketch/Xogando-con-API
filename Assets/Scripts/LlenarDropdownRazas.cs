using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LlenarDropdownPerros : MonoBehaviour
{
    public TMP_Dropdown miDropdown;
    private string urlLista = "https://dog.ceo/api/breeds/list/all";

    void Start()
    {
        StartCoroutine(ObtenerTodasLasRazas());
    }

    IEnumerator ObtenerTodasLasRazas()
    {
        using (UnityWebRequest web = UnityWebRequest.Get(urlLista))
        {
            yield return web.SendWebRequest();

            if (web.result == UnityWebRequest.Result.Success)
            {
                string json = web.downloadHandler.text;
                List<string> razas = ExtraerRazasCompuestas(json);
                
                miDropdown.ClearOptions();
                // Añadimos una opción neutra al principio
                miDropdown.AddOptions(new List<string> { "Selecciona una raza..." });
                miDropdown.AddOptions(razas);
                
                Debug.Log("Dropdown actualizado con " + razas.Count + " variantes de razas.");
            }
        }
    }

    List<string> ExtraerRazasCompuestas(string json)
    {
        List<string> listaFinal = new List<string>();

        // Este Regex busca la raza principal y el contenido de su lista de sub-razas
        // Ejemplo: "hound":["afghan","basset"...]
        string patronPrincipal = @"""(\w+)"":\[([^\]]*)\]";
        MatchCollection matches = Regex.Matches(json, patronPrincipal);

        foreach (Match m in matches)
        {
            string razaPrincipal = m.Groups[1].Value; // ej: hound
            string subRazasRaw = m.Groups[2].Value;   // ej: "afghan","basset"

            if (string.IsNullOrEmpty(subRazasRaw))
            {
                // Si no tiene sub-razas, añadimos solo la principal
                listaFinal.Add(razaPrincipal);
            }
            else
            {
                // Si tiene sub-razas, las extraemos y creamos el nombre compuesto
                // Buscamos palabras entre comillas dentro de la lista
                MatchCollection subMatches = Regex.Matches(subRazasRaw, @"""(\w+)""");
                
                if (subMatches.Count == 0)
                {
                    listaFinal.Add(razaPrincipal);
                }
                else
                {
                    foreach (Match sm in subMatches)
                    {
                        string subRaza = sm.Groups[1].Value;
                        // Creamos la palabra compuesta (ej: afghan hound)
                        // Nota: La API suele identificar las fotos como "raza-subraza"
                        listaFinal.Add(subRaza + " " + razaPrincipal);
                    }
                }
            }
        }

        listaFinal.Sort(); // Opcional: Ordenar alfabéticamente
        return listaFinal;
    }
}