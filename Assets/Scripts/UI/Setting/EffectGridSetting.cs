using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectGridSetting : MonoBehaviour
{
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        ReFreshText();
    }

    public void ReFreshText()
    {
        if (BoardManager.IsEffectGridEnable)
            text.SetText("ON");
        else
            text.SetText("OFF");
    }
}
