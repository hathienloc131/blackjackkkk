using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Table : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] LayerMask character;
    [SerializeField] private TMP_Text text;
    private bool canPressEnter = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isCollisionCharacter())
        {
            text.text = "Press ENTER to PLAY!";
            GlobalVariable.IS_CHANGE_SCENCE_PLAY_CARD = true;
        }
        else
        {
            text.text = "";
            GlobalVariable.IS_CHANGE_SCENCE_PLAY_CARD = false;
        }
    }

    bool isCollisionCharacter()
    {
        return Physics.CheckSphere(transform.position, 10, character);
    }
}
