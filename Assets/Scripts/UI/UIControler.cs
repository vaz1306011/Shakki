using System.Collections.Generic;
using UnityEngine;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (IsEnabled)
            {
                if (_UIstack.Peek().name == "Menu")
                    return;
                BackUI();
            }
            else
                OpenUI("Pause");
        }
    }

    public void GoMenu()
    {
        while (_UIstack.Count > 0)
            BackUI();
        OpenUI("Menu");
    }

    void EnablePlayersInput()
    {
        _player1Input.EnableInput();
        _player2Input.EnableInput();
    }

    void DisablePlayersInput()
    {
        _player1Input.DisableInput();
        _player2Input.DisableInput();
    }

    public void BackUI()
    {
        _UIstack.Pop().enabled = false;
        if (_UIstack.Count == 0)
            EnablePlayersInput();
    }

    public void OpenUI(string name)
    {
        var canvas = _canvases.Find(canvas => canvas.name == name);
        canvas.enabled = true;
        _UIstack.Push(canvas);
        DisablePlayersInput();
    }

    public void CloseGame()
    {
        Application.Quit();
        //EditorApplication.isPlaying = false;
    }
}
