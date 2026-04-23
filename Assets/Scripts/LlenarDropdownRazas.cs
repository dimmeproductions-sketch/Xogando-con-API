using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // Si usas el Dropdown clásico
using TMPro;           // Si usas TextMeshPro (recomendado)
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LlenarDropdownPerros : MonoBehaviour
{
    public TMP_Dropdown miDropdown; // Arrastra aquí tu Dropdown de TextMeshPro
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
                List<string> razas = ExtraerRazas(json);
                
                // Limpiar y llenar el Dropdown
                miDropdown.ClearOptions();
                miDropdown.AddOptions(razas);
                
                Debug.Log("Dropdown actualizado con " + razas.Count + " razas.");
            }
        }
    }

    // Método "ninja" para sacar las razas sin usar una clase compleja
    List<string> ExtraerRazas(string json)
    {
        List<string> listaFinal = new List<string>();
        
        // Buscamos lo que hay entre comillas antes de los dos puntos ":" 
        // dentro de la sección "message"
        string patron = @"""(\w+)"":\[";
        MatchCollection matches = Regex.Matches(json, patron);

        foreach (Match m in matches)
        {
            listaFinal.Add(m.Groups[1].Value);
        }

        return listaFinal;
    }
}