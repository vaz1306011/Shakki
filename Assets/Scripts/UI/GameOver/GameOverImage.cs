using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverImage : MonoBehaviour
{
    [SerializeField] Sprite P1Win, P2Win;
    public void SetImage(string winner)
    {
        if (winner == "White")
        {
            GetComponent<Image>().sprite = P1Win;
        }
        else if (winner == "Black")
        {
            GetComponent<Image>().sprite = P2Win;
        }
    }
}
