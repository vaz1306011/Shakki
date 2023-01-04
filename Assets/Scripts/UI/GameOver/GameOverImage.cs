using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverImage : MonoBehaviour
{
    [SerializeField] Sprite p1Win, p2Win;
    public void SetImage(string winner)
    {
        if (winner == "White")
        {
            GetComponent<Image>().sprite = p1Win;
        }
        else if (winner == "Black")
        {
            GetComponent<Image>().sprite = p2Win;
        }
    }
}
