using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controler : MonoBehaviour
{

    [Header("玩家")]
    [SerializeField] PlayerType _playerType;

    [Header("選取框")]
    [SerializeField] GameObject _selectBox;
    [SerializeField] GameObject _selectedBox;
    [SerializeField] GameObject _possibleMoveBox;

    [Header("棋盤")]
    [SerializeField] Board _board;

    BoardManager _boardManager;
    PlayerInput _playerInput;
    AudioSource _audioSource;

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
            _selectedBox.SetActive(value);
        }
    }

    void Start()
    {
        _selectedBox.SetActive(false);
        _board.DrawChesses(_playerType);
        _boardManager = GetComponentInParent<BoardManager>();
        _playerInput = GetComponent<PlayerInput>();
        LoadBind();
        _audioSource = GameObject.Find("SE").GetComponent<AudioSource>();
        isSelect = false;
        BackKing();
    }

    void Update()
    {
        if (isSelect)
            if (IsEnemy(_selectedBoxGrid))
                isSelect = false;

        _board.DrawChesses(_playerType);
    }

    int GetChessID(Vector2Int grid) => _boardManager.GetChessID(_playerType, grid);

    bool IsOutSideBoard(Vector2Int grid) => grid.x < 0 || grid.y < 0 || grid.x > 7 || grid.y > 7;

    bool IsEnemy(Vector2Int grid) => _playerType == PlayerType.White ? GetChessID(grid) < 0 : GetChessID(grid) > 0;

    bool IsAllies(Vector2Int grid) => _playerType == PlayerType.White ? GetChessID(grid) > 0 : GetChessID(grid) < 0;

    bool IsEmpty(Vector2Int grid) => GetChessID(grid) == 0;

    void UpdateSelectBox() => _selectBox.transform.position = _board.TransformPosition(_selectBoxGrid);

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
                var box = Instantiate(_possibleMoveBox, _board.TransformPosition(grid), Quaternion.identity);
                _possibleMoveBoxsTemp.Add(box);
            }
            isSelect = true;
            _selectedBox.transform.position = _board.TransformPosition(_selectedBoxGrid);
            _audioSource.Play();
        }
        //確認
        if (isSelect)
        {
            if (_possibleMoveGrids.Exists(grid => grid == _selectBoxGrid))
            {
                _boardManager.MoveChess(_playerType, _selectedBoxGrid, _selectBoxGrid);
                isSelect = false;
            }
            _audioSource.Play();
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
        _selectBoxGrid = _boardManager.GetKingGrid(_playerType);
        UpdateSelectBox();
    }

    public void BackKing(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;
        _selectBoxGrid = _boardManager.GetKingGrid(_playerType);
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
            if (_playerType == PlayerType.White)
            {
                if (!_boardManager.canCastling[0, way == -1 ? 0 : 1])
                    return;
            }
            else if (_playerType == PlayerType.Black)
            {
                if (!_boardManager.canCastling[1, way == -1 ? 0 : 1])
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
                grid = _selectedBoxGrid + Vector2Int.up + Vector2Int.left;
                if (!IsOutSideBoard(grid) && IsEnemy(grid))
                    AddGrid();

                //右前敵人
                grid = _selectedBoxGrid + Vector2Int.up + Vector2Int.right;
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

    public void SwitchInput(string mapName)
    {
        _playerInput?.SwitchCurrentActionMap(mapName);
    }

    string BindPath => Application.persistentDataPath + $"/{_playerType.ToString()}Binding.json";

    public void RestBind()
    {
        File.WriteAllText(BindPath, string.Empty);
        GameObject.Find("UI").GetComponentInChildren<UpdateBinding>().UpdateBindings();
    }

    public void SaveBind()
    {
        var json = _playerInput.actions.SaveBindingOverridesAsJson();
        File.WriteAllText(BindPath, json);
    }

    public void LoadBind()
    {
        try
        {
            var json = File.ReadAllText(BindPath);
            _playerInput.actions.LoadBindingOverridesFromJson(json);
        }
        catch (FileNotFoundException)
        {
            RestBind();
        }
    }
}

public enum PlayerType { White = 1, Black = -1 }

public enum Direction { Up, Down, Left, Right }
