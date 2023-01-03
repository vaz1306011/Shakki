using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillKingWin : MonoBehaviour
{
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        ReFreshText();
    }

    public void Switch()
    {
        BoardManager.KillKingWin = !BoardManager.KillKingWin;
        ReFreshText();
    }

    void ReFreshText()
    {
        if (BoardManager.KillKingWin)
            text.SetText("ON");
        else
            text.SetText("OFF");
    }
}
