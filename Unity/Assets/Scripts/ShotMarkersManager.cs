using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMarkersManager : MonoBehaviour
{
    // Marker display time
    private float markingTime = 3;
    private float runningTime = 3;
    bool timerIsRunning = false;

    public GameObject[] markers;

    //ustawianie wspolrzednych
    public string coordinates;
    // Start is called before the first frame update
    void Start()
    {
        coordinates = "";
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
            TurnOnMarker(coordinates);
            if (timerIsRunning)
            {
                if (runningTime > 0)
                {
                    runningTime -= Time.deltaTime;
                }
                else
                {
                    TurnOffMarker();
                }
            }
        }
    }

    void TurnOnMarker(string coordinates)
    {
        
        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i].name.Equals(coordinates.ToLower()))
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
