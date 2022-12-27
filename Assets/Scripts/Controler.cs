using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controler : MonoBehaviour
{
    [Header("���a")]
    [SerializeField] PlayerType _playerType;

    [Header("�����")]
    [SerializeField] GameObject _selectBox;
    [SerializeField] GameObject _selectedBox;
    [SerializeField] GameObject _possibleMoveBox;

    [Header("�ѽL")]
    [SerializeField] Board _board;

    [Header("�ֱ���")]
    [SerializeField] KeyCode _up;
    [SerializeField] KeyCode _down;
    [SerializeField] KeyCode _left;
    [SerializeField] KeyCode _right;
    [SerializeField] KeyCode _confirm;
    [SerializeField] KeyCode _cancel;
    [SerializeField] KeyCode _backKing;

    BoardManager _boardManager;
    AudioSource _source;
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
        _source = GameObject.Find("SE").GetComponent<AudioSource>();
        _boardManager = GetComponentInParent<BoardManager>();
        _board.DrawChesses(_playerType);
        _selectedBox.SetActive(false);
        isSelect = false;
    }

    void Update()
    {
        PlayerInput();
    }

    int GetChessID(Vector2Int grid) => _boardManager.GetChessID(_playerType, grid);

    bool IsOutSideBoard(Vector2Int grid) => grid.x < 0 || grid.y < 0 || grid.x > 7 || grid.y > 7;

    bool IsEnemy(Vector2Int grid) => _playerType == PlayerType.White ? GetChessID(grid) < 0 : GetChessID(grid) > 0;

    bool IsAllies(Vector2Int grid) => _playerType == PlayerType.White ? GetChessID(grid) > 0 : GetChessID(grid) < 0;

    bool IsEmpty(Vector2Int grid) => GetChessID(grid) == 0;

    void UpdateSelectBox() => _selectBox.transform.position = _board.TransformPosition(_selectBoxGrid);

    void Move(Vector2Int direction)
    {
        _selectBoxGrid += direction;

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

    void BackKing()
    {
        _selectBoxGrid = _boardManager.GetKingGrid(_playerType);
        UpdateSelectBox();
    }

    void PlayerInput()
    {
        if (UIControler.IsEnabled)
            return;

        if (Input.GetKeyDown(_right))
            Move(Vector2Int.right);
        if (Input.GetKeyDown(_left))
            Move(Vector2Int.left);
        if (Input.GetKeyDown(_up))
            Move(Vector2Int.up);
        if (Input.GetKeyDown(_down))
            Move(Vector2Int.down);

        if (Input.GetKeyDown(_confirm))
        {
            //���
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
                _source.Play();
            }
            //�T�{
            if (isSelect)
            {
                if (_possibleMoveGrids.Exists(grid => grid == _selectBoxGrid))
                {
                    _boardManager.MoveChess(_playerType, _selectedBoxGrid, _selectBoxGrid);
                    isSelect = false;
                }
                _source.Play();
            }
        }
        if (Input.GetKeyDown(_cancel))
            isSelect = false;

        if (Input.GetKeyDown(_backKing))
            BackKing();

        if (isSelect)
            if (IsEnemy(_selectedBoxGrid))
                isSelect = false;

        _board.DrawChesses(_playerType);
    }

    List<Vector2Int> GetPossibleMoveGrids()
    {
        /* 
         * ��:0
         * ��:+
         * ��:-
         * ���:1
         * �ӦZ:2
         * �D��:3
         * �M�h:4
         * ����:5
         * �h�L:6
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
            //���
            case 1:
            case -1:
                //���`����
                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j <= 1; j++)
                    {
                        grid = _selectedBoxGrid + new Vector2Int(i, j);
                        AddGrid();
                    }

                //��������
                CheckCastling(-1); //����
                CheckCastling(1);  //�k��
                break;

            //�ӦZ 
            case 2:
            case -2:
                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j <= 1; j++)
                        WalkLine(i, j);
                break;

            //�D��
            case 3:
            case -3:
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                        WalkLine(i, j);

                break;

            //�M�h
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

            //����
            case 5:
            case -5:
                WalkLine(0, 1);
                WalkLine(0, -1);
                WalkLine(1, 0);
                WalkLine(-1, 0);
                break;

            //�h�L
            case 6:
            case -6:
                //���e�ĤH
                grid = _selectedBoxGrid + Vector2Int.up + Vector2Int.left;
                if (!IsOutSideBoard(grid) && IsEnemy(grid))
                    AddGrid();

                //�k�e�ĤH
                grid = _selectedBoxGrid + Vector2Int.up + Vector2Int.right;
                if (!IsOutSideBoard(grid) && IsEnemy(grid))
                    AddGrid();

                //���`����
                grid = _selectedBoxGrid + Vector2Int.up;
                if (!IsOutSideBoard(grid) && !IsEmpty(grid))
                    break;
                AddGrid();

                //�����h���ʤ@��
                grid = _selectedBoxGrid + Vector2Int.up * 2;
                if (_selectBoxGrid.y == 1 && IsEmpty(grid))
                    AddGrid();
                break;
        }

        return possibleMoveGrids;
    }

    public void SetHotKeys(HotKeyControler.PlayerHotKeys playerHotKeys)
    {
        if (_playerType == PlayerType.White)
        {
            _up = playerHotKeys.white.up;
            _down = playerHotKeys.white.down;
            _left = playerHotKeys.white.left;
            _right = playerHotKeys.white.right;
            _confirm = playerHotKeys.white.confirm;
            _cancel = playerHotKeys.white.cancel;
            _backKing = playerHotKeys.white.backKing;
        }
        else if (_playerType == PlayerType.Black)
        {
            _up = playerHotKeys.black.up;
            _down = playerHotKeys.black.down;
            _left = playerHotKeys.black.left;
            _right = playerHotKeys.black.right;
            _confirm = playerHotKeys.black.confirm;
            _cancel = playerHotKeys.black.cancel;
            _backKing = playerHotKeys.black.backKing;
        }
    }
}

public enum PlayerType { White = 1, Black = -1 }
