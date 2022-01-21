using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipsManager : MonoBehaviour
{
    public GameObject[] coordinates;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < coordinates.Length; i++)
        {
            coordinates[i].GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
