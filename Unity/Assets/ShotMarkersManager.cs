using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMarkersManager : MonoBehaviour
{
    // Marker display time
    private float markingTime = 2;
    private float runningTime = 2;
    bool timerIsRunning = false;

    public GameObject[] markers;

    //ustawianie wspolrzednych
    public string coordinates = "a5";
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < markers.Length; i++)
        {
            markers[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!coordinates.Equals(""))
        {
            Debug.Log("if");
            TurnOnMarker(coordinates);
            if (timerIsRunning)
            {
                if (runningTime > 0)
                {
                    runningTime -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("Time has run out!");
                    TurnOffMarker();
                }
            }
        }
    }

    void TurnOnMarker(string coordinates)
    {
        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i].name.Equals(coordinates))
            {
                if (markers[i].activeInHierarchy == false)
                {
                    markers[i].SetActive(true);
                    timerIsRunning = true;
                }
            }
        }
    }

    void TurnOffMarker()
    {
        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i].activeInHierarchy == true)
            {
                markers[i].SetActive(false);
                timerIsRunning = false;
                coordinates = "";
                runningTime = markingTime;
            }      
        }
    }

}
