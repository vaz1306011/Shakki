using System.Collections.Generic;
using UnityEngine;

public class UIControler : MonoBehaviour
{
    static List<Canvas> _canvas = new List<Canvas>();
    static Stack<Canvas> _UIstack = new Stack<Canvas>();
    public static bool IsEnabled
    {
        get { return _UIstack.Count > 0; }
    }

    void Start()
    {
        foreach (Transform child in transform)
            _canvas.Add(child.GetComponent<Canvas>());
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

        var canva = _canvas.Find(canva => canva.name == name);
        canva.enabled = true;
        //if (IsEnabled)
        //    _UIstack.Peek().enabled = false;
        _UIstack.Push(canva);
    }

    public void CloseGame()
    {
        Application.Quit();
        //EditorApplication.isPlaying = false;
    }
}
