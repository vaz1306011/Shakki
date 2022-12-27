using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
    List<Canvas> _canvas = new List<Canvas>();
    static bool _isEnabled;
    public static bool IsEnabled
    {
        get { return _isEnabled; }
    }

    void Start()
    {
        foreach (Transform child in transform)
            _canvas.Add(child.GetComponent<Canvas>());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_canvas.Find(canva => canva.enabled == true))
                CloseAll();
            else
                Open("Setting");
        }
    }

    public void Open(string name)
    {
        foreach (var canva in _canvas)
        {
            if (canva.name == name)
                canva.enabled = true;
            else
                canva.enabled = false;
        }
        _isEnabled = true;
    }

    public void CloseAll()
    {
        foreach (var canva in _canvas)
            canva.enabled = false;
        _isEnabled = false;
    }

    public void CloseGame()
    {
        Application.Quit();
        EditorApplication.isPlaying = false;
    }
}
