using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Rebind : MonoBehaviour
{
    [SerializeField] Controler _playerControler;
    [SerializeField] InputActionReference _ref;
    [SerializeField] TextMeshProUGUI _text;

    public void UpdateText()
    {
        var p = _ref.action.bindings[0].effectivePath;
        var text = InputControlPath.ToHumanReadableString(p, InputControlPath.HumanReadableStringOptions.OmitDevice);
        _text.SetText(text);
    }

    void bind(InputActionRebindingExtensions.RebindingOperation operation)
    {
        var p = operation.action.bindings[0].effectivePath;
        var text = InputControlPath.ToHumanReadableString(p, InputControlPath.HumanReadableStringOptions.OmitDevice);
        _text.SetText(text);
        _playerControler.SaveBind();
        operation.Dispose();
    }

    public void Rebinding()
    {
        _text.SetText("�п�J...");
        _ref.action.PerformInteractiveRebinding()
        .WithCancelingThrough("<Keyboard>/escape")
        .OnComplete(bind)
        .OnCancel(bind)
        .Start();
    }
}
