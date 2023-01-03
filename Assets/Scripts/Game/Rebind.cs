using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class Rebind : MonoBehaviour
{
    [SerializeField] Controler _playerControler;
    [SerializeField] InputActionReference _ref;
    [SerializeField] TextMeshProUGUI _text;

    public void RefreshText()
    {
        var p = _ref.action.bindings[0].effectivePath;
        var text = InputControlPath.ToHumanReadableString(p, InputControlPath.HumanReadableStringOptions.OmitDevice);
        _text.SetText(text);
    }

    void bind(InputActionRebindingExtensions.RebindingOperation operation)
    {
        var p = operation.action.bindings[0].effectivePath;
        var text = InputControlPath.ToHumanReadableString(p, InputControlPath.HumanReadableStringOptions.OmitDevice);
        _text.transform.parent.GetComponent<Image>().color = Color.white;
        _text.SetText(text);
        _playerControler.SaveBind();
        operation.Dispose();
    }

    public void Rebinding()
    {
        _text.transform.parent.GetComponent<Image>().color = new Color(255, 140, 0, 255);
        _text.SetText("½Ð¿é¤J...");
        _ref.action.PerformInteractiveRebinding()
        .OnMatchWaitForAnother(.1f)
        .WithCancelingThrough("<Keyboard>/escape")
        .OnComplete(bind)
        .OnCancel(bind)
        .Start();
    }
}
