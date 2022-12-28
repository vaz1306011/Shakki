using System.Collections.Generic;
using UnityEngine;

public class UIControler : MonoBehaviour
{
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
        OpenUI("Menu");
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

    public void BackUI()
    {
        _UIstack.Pop().enabled = false;
    }

    public void OpenUI(string name)
    {

        var canvas = _canvases.Find(canvas => canvas.name == name);
        canvas.enabled = true;
        //if (IsEnabled)
        //    _UIstack.Peek().enabled = false;
        _UIstack.Push(canvas);
    }

    public void CloseGame()
    {
        Application.Quit();
        //EditorApplication.isPlaying = false;
    }
}
