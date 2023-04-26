using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controler : MonoBehaviour
{

    [Header("玩家")]
    [SerializeField] PlayerType playerType;

    [Header("選取框")]
    [SerializeField] GameObject selectBox;
    [SerializeField] GameObject selectedBox;
    [SerializeField] GameObject possibleMoveBox;

    [Header("棋盤")]
    [SerializeField] Board board;

    [Header("選取音效")]
    [SerializeField] AudioSource audioSource;

    [HideInInspector] public float ReverseTime = 0;

    BoardManager boardManager;
    PlayerInput playerInput;

    Vector2Int _selectBoxGrid = Vector2Int.zero;
    Vector2Int _selectedBoxGrid;
    List<GameObject> _possibleMoveBoxsTemp = new List<GameObject>();
    List<Vector2Int> _possibleMoveGrids = new List<Vector2Int>();
    bool _isSelect;
    bool isSelect
    {
        get { return _isSelect; }
        set
        {
            _isSelect = value;
            if (!value)
            {
                foreach (var pmb in _possibleMoveBoxsTemp)
                    Destroy(pmb);
                _possibleMoveBoxsTemp.Clear();
            }
            selectedBox.SetActive(value);
        }
    }

    void Start()
    {
        selectedBox.SetActive(false);
        board.DrawBoard(playerType);
        boardManager = GetComponentInParent<BoardManager>();
        playerInput = GetComponent<PlayerInput>();
        LoadBind();
        isSelect = false;
        BackKing();
    }

    void Update()
    {
        if (isSelect)
            if (IsEnemy(_selectedBoxGrid))
                isSelect = false;

        board.DrawBoard(playerType);

        if (ReverseTime > 0)
        {
            ReverseTime -= Time.deltaTime;
            if (ReverseTime <= 0)
                SetInputMap("Default");
        }
    }

    int GetChessID(Vector2Int grid) => boardManager.GetChessID(grid, playerType);

    bool IsOutSideBoard(Vector2Int grid) => grid.x < 0 || grid.y < 0 || grid.x > 7 || grid.y > 7;

    bool IsEnemy(Vector2Int grid)
    {
        if (GetChessID(grid) == 7)
            return false;
        return playerType == PlayerType.White ? GetChessID(grid) < 0 : GetChessID(grid) > 0;
    }

    bool IsAllies(Vector2Int grid)
    {
        if (GetChessID(grid) == 7)
            return false;
        return playerType == PlayerType.White ? GetChessID(grid) > 0 : GetChessID(grid) < 0;
    }

    bool IsEmpty(Vector2Int grid) => GetChessID(grid) == 0 || GetChessID(grid) == 7;

    void UpdateSelectBox() => selectBox.transform.position = board.TransformPosition(_selectBoxGrid);

    void Move(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                _selectBoxGrid += Vector2Int.up;
                break;

            case Direction.Down:
                _selectBoxGrid += Vector2Int.down;
                break;

            case Direction.Left:
                _selectBoxGrid += Vector2Int.left;
                break;

            case Direction.Right:
                _selectBoxGrid += Vector2Int.right;
                break;
        }

        if (_selectBoxGrid.x < 0)
            _selectBoxGrid.x = 0;
        if (_selectBoxGrid.y < 0)
            _selectBoxGrid.y = 0;
        if (_selectBoxGrid.x > 7)
            _selectBoxGrid.x = 7;
        if (_selectBoxGrid.y > 7)
            _selectBoxGrid.y = 7;

        UpdateSelectBox();
    }

    public void MoveUp(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
            return;

        Move(Direction.Up);
    }

    public void MoveDown(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
            return;

        Move(Direction.Down);
    }

    public void MoveLeft(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
            return;

        Move(Direction.Left);
    }

    public void MoveRight(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
            return;

        Move(Direction.Right);
    }

    public void Confirm(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        //選擇
        if (!isSelect && IsAllies(_selectBoxGrid))
        {
            _selectedBoxGrid = _selectBoxGrid;
            _possibleMoveGrids = GetPossibleMoveGrids();
            foreach (var grid in _possibleMoveGrids)
            {
                var box = Instantiate(possibleMoveBox, board.TransformPosition(grid), Quaternion.identity);
                _possibleMoveBoxsTemp.Add(box);
            }
            isSelect = true;
            selectedBox.transform.position = board.TransformPosition(_selectedBoxGrid);
            audioSource?.Play();
        }
        //確認
        else if (isSelect)
        {
            if (_possibleMoveGrids.Exists(grid => grid == _selectBoxGrid))
            {
                boardManager.MoveChess(_selectedBoxGrid, _selectBoxGrid, playerType);
                isSelect = false;
            }
            audioSource?.Play();
        }
    }

    public void Cancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        isSelect = false;
    }

    public void BackKing()
    {
        _selectBoxGrid = boardManager.GetKingGrid(playerType) ?? _selectBoxGrid;
        UpdateSelectBox();
    }

    public void BackKing(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        _selectBoxGrid = boardManager.GetKingGrid(playerType) ?? _selectBoxGrid;
        UpdateSelectBox();
    }

    List<Vector2Int> GetPossibleMoveGrids()
    {
        /* 
         * 空:0
         * 白:+
         * 黑:-
         * 國王:1
         * 皇后:2
         * 主教:3
         * 騎士:4
         * 城堡:5
         * 士兵:6
         */
        var possibleMoveGrids = new List<Vector2Int>();
        Vector2Int grid;

        bool AddGrid()
        {
            if (IsOutSideBoard(grid) || IsAllies(grid))
                return false;
            possibleMoveGrids.Add(grid);
            return true;
        }

        void CheckCastling(int way)
        {
            if (playerType == PlayerType.White)
            {
                if (!boardManager.CanCastling[0, way == -1 ? 0 : 1])
                    return;
            }
            else if (playerType == PlayerType.Black)
            {
                if (!boardManager.CanCastling[1, way == -1 ? 0 : 1])
                    return;
            }

            bool canCastling = true;
            for (int x = _selectedBoxGrid.x + way; x != (way == 1 ? 7 : 0); x += way)
                if (!IsEmpty(Vector2Int.right * x))
                {
                    canCastling = false;
                    break;
                }
            if (canCastling)
            {
                grid = _selectedBoxGrid + Vector2Int.right * way * 2;
                AddGrid();
            }
        }

        void WalkLine(int x, int y)
        {
            for (var i = 1; i <= 7; i++)
            {
                grid = _selectedBoxGrid + new Vector2Int(i * x, i * y);
                if (AddGrid())
                {
                    if (IsEnemy(grid))
                        return;
                }
                else
                    return;
            }
        }

        switch (GetChessID(_selectBoxGrid))
        {
            //國王
            case 1:
            case -1:
                //正常移動
                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j <= 1; j++)
                    {
                        grid = _selectedBoxGrid + new Vector2Int(i, j);
                        AddGrid();
                    }

                //王車易位
                CheckCastling(-1); //左邊
                CheckCastling(1);  //右邊
                break;

            //皇后 
            case 2:
            case -2:
                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j <= 1; j++)
                        WalkLine(i, j);
                break;

            //主教
            case 3:
            case -3:
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                        WalkLine(i, j);

                break;

            //騎士
            case 4:
            case -4:
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                    {
                        grid = _selectedBoxGrid + new Vector2Int(1 * i, 2 * j);
                        AddGrid();
                    }
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                    {
                        grid = _selectedBoxGrid + new Vector2Int(2 * i, 1 * j);
                        AddGrid();
                    }
                break;

            //城堡
            case 5:
            case -5:
                WalkLine(0, 1);
                WalkLine(0, -1);
                WalkLine(1, 0);
                WalkLine(-1, 0);
                break;

            //士兵
            case 6:
            case -6:
                //左前敵人
                grid = _selectedBoxGrid + new Vector2Int(-1, 1);
                if (!IsOutSideBoard(grid) && IsEnemy(grid))
                    AddGrid();

                //右前敵人
                grid = _selectedBoxGrid + new Vector2Int(1, 1);
                if (!IsOutSideBoard(grid) && IsEnemy(grid))
                    AddGrid();

                //正常移動
                grid = _selectedBoxGrid + Vector2Int.up;
                if (!IsOutSideBoard(grid) && !IsEmpty(grid))
                    break;
                AddGrid();

                //首次多移動一格
                grid = _selectedBoxGrid + Vector2Int.up * 2;
                if (_selectBoxGrid.y == 1 && IsEmpty(grid))
                    AddGrid();
                break;
        }

        return possibleMoveGrids;
    }

    public void SetInputMap(string mapName)
    {
        if (mapName == "Reverse")
        {
            UpdaeReverseInput();
        }

        playerInput?.SwitchCurrentActionMap(mapName);
    }

    string BindPath => Application.persistentDataPath + $"/{playerType.ToString()}Binding.json";

    public void RestBind()
    {
        File.WriteAllText(BindPath, string.Empty);
        try
        {
            GameObject.Find("UI").GetComponentInChildren<UpdateBinding>().UpdateBindings();
        }
        catch (NullReferenceException) { }
    }

    public void SaveBind()
    {
        var json = playerInput.actions.SaveBindingOverridesAsJson();
        File.WriteAllText(BindPath, json);
    }

    public void LoadBind()
    {
        try
        {
            var json = File.ReadAllText(BindPath);
            playerInput.actions.LoadBindingOverridesFromJson(json);
        }
        catch (FileNotFoundException)
        {
            RestBind();
        }
    }

    public void UpdaeReverseInput()
    {
        void SetBinding(InputAction a, InputAction b)
        {
            a.ApplyBindingOverride(b.bindings[0].effectivePath);
        }
        var dUp = playerInput.actions["Default/Up"];
        var dDown = playerInput.actions["Default/Down"];
        var dLeft = playerInput.actions["Default/Left"];
        var dRight = playerInput.actions["Default/Right"];
        var dConfirm = playerInput.actions["Default/Confirm"];
        var dCancel = playerInput.actions["Default/Cancel"];
        var dBackking = playerInput.actions["Default/BackKing"];

        var rUp = playerInput.actions["Reverse/Up"];
        var rDown = playerInput.actions["Reverse/Down"];
        var rLeft = playerInput.actions["Reverse/Left"];
        var rRight = playerInput.actions["Reverse/Right"];
        var rConfirm = playerInput.actions["Reverse/Confirm"];
        var rCancel = playerInput.actions["Reverse/Cancel"];
        var rBackking = playerInput.actions["Reverse/BackKing"];

        SetBinding(rUp, dDown);
        SetBinding(rDown, dUp);
        SetBinding(rLeft, dRight);
        SetBinding(rRight, dLeft);
        SetBinding(rConfirm, dBackking);
        SetBinding(rCancel, dConfirm);
        SetBinding(rBackking, dCancel);
    }
}

public enum PlayerType { White = 1, Black = -1 }

public enum Direction { Up, Down, Left, Right }
