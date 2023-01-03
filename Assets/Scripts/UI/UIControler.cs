using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControler : MonoBehaviour
{
    public Controler Player1Controler;
    public Controler Player2Controler;

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

    public void OpenUI(string name)
    {
        var canvas = _canvases.Find(canvas => canvas.name == name);
        canvas.enabled = true;
        _UIstack.Push(canvas);
        if (canvas.name == "Setting")
            canvas.gameObject.GetComponent<UpdateBinding>().UpdateBindings();
        Player1Controler.SwitchInput("Pause");
        Player2Controler.SwitchInput("Pause");
    }

    public void BackUI()
    {
        _UIstack.Pop().enabled = false;
        if (_UIstack.Count == 0)
        {
            Player1Controler.SwitchInput("Default");
            Player2Controler.SwitchInput("Default");
        }
    }

    public void CloseAllUI()
    {
        foreach (var canvas in _canvases)
            canvas.enabled = false;
        _UIstack.Clear();
    }

    public void PauseUI(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
            return;

        OpenUI("Pause");
    }

    public void BackUI(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
            return;

        if (_UIstack.Peek().name == "Menu" || _UIstack.Peek().name == "GameOver")
            return;

        BackUI();
    }

    public void GoMenu()
    {
        CloseAllUI();
        OpenUI("Menu");
    }

    public void GameOver(string winner)
    {
        OpenUI("GameOver");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
