using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CommunicationButtons : MonoBehaviour
{
    public const string COORDINATES_URL_ENDPOINT = "https://localhost:44386/Game";

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(SendCoordinatesToServer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Funkcja wywoływana przez przycisk
    /// </summary>
    public void SendCoordinatesToServer_Event()
    {
        StartCoroutine(SendCoordinatesToServer());
    }

    /// <summary>
    /// Wysłane danych do serwera
    /// </summary>
    /// <returns></returns>
    IEnumerator SendCoordinatesToServer()
    {
        Debug.Log("Sending coordinates");

        WWWForm form = new WWWForm();
        form.AddField("coordinate", "A2");

        UnityWebRequest uwr = UnityWebRequest.Post(COORDINATES_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Result: " + uwr.downloadHandler.text);
        }
    }
}
