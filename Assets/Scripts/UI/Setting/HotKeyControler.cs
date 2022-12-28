using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class HotKeyControler : MonoBehaviour
{
    [System.Serializable]
    public class HotKeys
    {
        public KeyCode up;
        public KeyCode down;
        public KeyCode left;
        public KeyCode right;
        public KeyCode confirm;
        public KeyCode cancel;
        public KeyCode backKing;
    }

    [System.Serializable]
    public class PlayerHotKeys
    {
        public HotKeys white;
        public HotKeys black;
    }

    [SerializeField] TextAsset _hotKeyJson;
    [SerializeField] Controler _player1Controler;
    [SerializeField] Controler _player2Controler;

    PlayerHotKeys _playerHotKeys;

    void Start()
    {
        _playerHotKeys = JsonUtility.FromJson<PlayerHotKeys>(_hotKeyJson.text); //±qHotKey.jsonÅª¨ú§Ö±¶Áä
        SetControlers();
    }

    void SetControlers()
    {
        _player1Controler.SetHotKeys(_playerHotKeys);
        _player2Controler.SetHotKeys(_playerHotKeys);
    }

    public void SetHotKey(PlayerType playerType, Operate operate, KeyCode key)
    {
        switch (operate)
        {
            case Operate.Up:
                if (playerType == PlayerType.White)
                    _playerHotKeys.white.up = key;
                else if (playerType == PlayerType.Black)
                    _playerHotKeys.black.up = key;
                break;
            case Operate.Down:
                if (playerType == PlayerType.White)
                    _playerHotKeys.white.down = key;
                else if (playerType == PlayerType.Black)
                    _playerHotKeys.black.down = key;
                break;
            case Operate.Left:
                if (playerType == PlayerType.White)
                    _playerHotKeys.white.left = key;
                else if (playerType == PlayerType.Black)
                    _playerHotKeys.black.left = key;
                break;
            case Operate.Right:
                if (playerType == PlayerType.White)
                    _playerHotKeys.white.right = key;
                else if (playerType == PlayerType.Black)
                    _playerHotKeys.black.right = key;
                break;
            case Operate.Confirm:
                if (playerType == PlayerType.White)
                    _playerHotKeys.white.confirm = key;
                else if (playerType == PlayerType.Black)
                    _playerHotKeys.black.confirm = key;
                break;
            case Operate.Cancel:
                if (playerType == PlayerType.White)
                    _playerHotKeys.white.cancel = key;
                else if (playerType == PlayerType.Black)
                    _playerHotKeys.black.cancel = key;
                break;
            case Operate.BackKing:
                if (playerType == PlayerType.White)
                    _playerHotKeys.white.backKing = key;
                else if (playerType == PlayerType.Black)
                    _playerHotKeys.black.backKing = key;
                break;
        }

        var json = JsonUtility.ToJson(_playerHotKeys);
        File.WriteAllText(Application.dataPath + "/Scripts/HotKey.json", json);
        SetControlers();
    }
}


public enum Operate
{
    Up,
    Down,
    Left,
    Right,
    Confirm,
    Cancel,
    BackKing
}