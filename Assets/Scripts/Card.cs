using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private GameObject prefab;
    private int point;
    public int Point { get { return this.point; } }
    public GameObject Prefab { get { return this.prefab; } }

    public Card(GameObject prefab) {
        this.prefab = prefab;
        string name = prefab.name;
        int point;
        string sub_name = name.Substring(name.Length - 5,2);
        switch (sub_name)
        {
            // ace
            case "01":
                point = 11;
                break;
            case "10": // jacK
            case "11": // kinG
            case "12": // queeN
            case "13": // 10
                point = 10;
                break;
            default:
                // other remaining possible cards, 2 - 9
                point = Convert.ToInt16(sub_name);
                break;
        }
        this.point = point;
    }
}