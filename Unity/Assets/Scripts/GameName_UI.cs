using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameName_UI : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;


    void Start()
    {
        textComponent.text = GameManager.Instance.gameName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
