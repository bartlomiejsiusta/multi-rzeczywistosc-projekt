using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationButtons : MonoBehaviour
{
    public const string COORDINATES_URL_ENDPOINT = "";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getRequest("http:///www.yoururl.com"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SendCoordinatesToServer()
    {
        Debug.Log("Sending coordinates");

        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");
        form.AddField("Game Name", "Mario Kart");

        UnityWebRequest uwr = UnityWebRequest.Post(COORDINATES_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }
}
