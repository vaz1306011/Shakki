using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControler : MonoBehaviour
{
    [SerializeField] Controler _player1Input;
    [SerializeField] Controler _player2Input;

    static List<Canvas> _canvases = new List<Canvas>();
    static Stack<Canvas> _UIstack = new Stack<Canvas>();

    public static bool IsEnabled
    {
        get { return _UIstack.Count > 0; }
    }

    void Start()
    {
        foreach (Transform child in transform)
            _canvases.Add(child.GetComponent<Canvas>());

        GoMenu();
    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
            return;

        OpenUI("Pause");
    }

    public void Back(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
            return;

        if (_UIstack.Peek().name == "Menu")
            return;

        BackUI();
    }

    public void GoMenu()
    {
        foreach (var canvas in _canvases)
            canvas.enabled = false;
        _UIstack.Clear();
        OpenUI("Menu");
    }

    void SwitchPlayersInput(string mapName)
    {
        _player1Input.SwitchInput(mapName);
        _player2Input.SwitchInput(mapName);
    }

    public void BackUI()
    {
        _UIstack.Pop().enabled = false;
        if (_UIstack.Count == 0)
            SwitchPlayersInput("Default");
    }

    public void OpenUI(string name)
    {
        var canvas = _canvases.Find(canvas => canvas.name == name);
        canvas.enabled = true;
        _UIstack.Push(canvas);
        if (canvas.name == "Setting")
            canvas.gameObject.GetComponent<UpdateBinding>().UpdateBindings();
        SwitchPlayersInput("Pause");
    }

    public void CloseGame()
    {
        Application.Quit();
        //EditorApplication.isPlaying = false;
    }
}
