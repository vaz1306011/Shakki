using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerType { white = 1, black = -1 }

public class Controler : MonoBehaviour
{
    [Header("玩家")]
    [SerializeField] PlayerType player;

    [Header("選取框")]
    [SerializeField] GameObject selectBox;
    [SerializeField] GameObject selectedBox;
    [SerializeField] GameObject possibleMoveBox;

    [Header("棋盤")]
    [SerializeField] Board board;

    [Header("快捷鍵")]
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode confirm;
    public KeyCode cancel;

    BoardManager boardManager;
    Vector2Int selectBoxGrid;
    Vector2Int selectedBoxGrid;
    List<GameObject> possibleMoveBoxsTemp = new List<GameObject>();
    List<Vector2Int> possibleMoveGrids = new List<Vector2Int>();
    bool _isSelect;
    bool isSelect
    {
        get { return _isSelect; }
        set
        {
            _isSelect = value;
            if (!value)
            {
                foreach (var pmb in possibleMoveBoxsTemp)
                    Destroy(pmb);
                possibleMoveBoxsTemp.Clear();
            }
            selectedBox.SetActive(value);
        }
    }

    void Start()
    {
        boardManager = GetComponentInParent<BoardManager>();
        board.DrawChesses(player);
        selectBoxGrid = Vector2Int.zero;
        selectedBox.SetActive(false);
        isSelect = false;
    }

    void Update()
    {
        PlayerInput();
    }

    int GetChessID(Vector2Int grid) => boardManager.GetChessID(player, grid);

    bool IsOutSideBoard(Vector2Int grid) => grid.x < 0 || grid.y < 0 || grid.x > 7 || grid.y > 7;

    bool IsEnemy(Vector2Int grid) => player == PlayerType.white ? GetChessID(grid) < 0 : GetChessID(grid) > 0;

    bool IsAllies(Vector2Int grid) => player == PlayerType.white ? GetChessID(grid) > 0 : GetChessID(grid) < 0;

    bool IsEmpty(Vector2Int grid) => GetChessID(grid) == 0;

    void Move(Vector2Int direction)
    {
        selectBoxGrid += direction;

        if (selectBoxGrid.x < 0)
            selectBoxGrid.x = 0;
        if (selectBoxGrid.y < 0)
            selectBoxGrid.y = 0;
        if (selectBoxGrid.x > 7)
            selectBoxGrid.x = 7;
        if (selectBoxGrid.y > 7)
            selectBoxGrid.y = 7;

        selectBox.transform.position = board.TransformPosition(selectBoxGrid);
    }

    void PlayerInput()
    {
        //TODO json存取快捷鍵
        if (Input.GetKeyDown(right))
            Move(Vector2Int.right);
        if (Input.GetKeyDown(left))
            Move(Vector2Int.left);
        if (Input.GetKeyDown(up))
            Move(Vector2Int.up);
        if (Input.GetKeyDown(down))
            Move(Vector2Int.down);

        if (Input.GetKeyDown(confirm))
        {
            //選擇
            if (!isSelect && IsAllies(selectBoxGrid))
            {
                selectedBoxGrid = selectBoxGrid;
                possibleMoveGrids = GetPossibleMoveGrids();
                foreach (var grid in possibleMoveGrids)
                {
                    var box = Instantiate(possibleMoveBox, board.TransformPosition(grid), Quaternion.identity);
                    possibleMoveBoxsTemp.Add(box);
                }
                isSelect = true;
                selectedBox.transform.position = board.TransformPosition(selectedBoxGrid);
            }
            //確認
            if (isSelect)
            {
                if (possibleMoveGrids.Exists(grid => grid == selectBoxGrid))
                {
                    boardManager.MoveChess(player, selectedBoxGrid, selectBoxGrid);
                    isSelect = false;
                }
            }
        }
        if (Input.GetKeyDown(cancel))
            isSelect = false;

        if (isSelect)
            if (IsEnemy(selectedBoxGrid))
                isSelect = false;

        board.DrawChesses(player);
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
            if (player == PlayerType.white)
            {
                if (!boardManager.canCastling[0, way == -1 ? 0 : 1])
                    return;
            }
            else if (player == PlayerType.black)
            {
                if (!boardManager.canCastling[1, way == -1 ? 0 : 1])
                    return;
            }

            bool canCastling = true;
            for (int x = selectedBoxGrid.x + way; x != (way == 1 ? 7 : 0); x += way)
                if (!IsEmpty(Vector2Int.right * x))
                {
                    canCastling = false;
                    break;
                }
            if (canCastling)
            {
                grid = selectedBoxGrid + Vector2Int.right * way * 2;
                AddGrid();
            }
        }

        void WalkLine(int x, int y)
        {
            for (var i = 1; i <= 7; i++)
            {
                grid = selectedBoxGrid + new Vector2Int(i * x, i * y);
                if (AddGrid())
                {
                    if (IsEnemy(grid))
                        return;
                }
                else
                    return;
            }
        }

        switch (GetChessID(selectBoxGrid))
        {
            //國王
            case 1:
            case -1:
                //正常移動
                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j <= 1; j++)
                    {
                        grid = selectedBoxGrid + new Vector2Int(i, j);
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
                        grid = selectedBoxGrid + new Vector2Int(1 * i, 2 * j);
                        AddGrid();
                    }
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                    {
                        grid = selectedBoxGrid + new Vector2Int(2 * i, 1 * j);
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
                bool lrIsEnemy = false;
                grid = selectedBoxGrid + Vector2Int.up + Vector2Int.left;
                if (!IsOutSideBoard(grid) && IsEnemy(grid))
                {
                    lrIsEnemy = true;
                    AddGrid();
                }

                //右前敵人
                grid = selectedBoxGrid + Vector2Int.up + Vector2Int.right;
                if (!IsOutSideBoard(grid) && IsEnemy(grid))
                {
                    lrIsEnemy = true;
                    AddGrid();
                }
                if (lrIsEnemy)
                    break;

                //正常移動
                if (IsEmpty(selectBoxGrid + Vector2Int.up))
                {
                    grid = selectedBoxGrid + Vector2Int.up;
                    AddGrid();
                }

                //首次多移動一格
                if (selectBoxGrid.y == 1 && IsEmpty(selectBoxGrid + Vector2Int.up) && IsEmpty(selectBoxGrid + Vector2Int.up * 2))
                {
                    grid = selectedBoxGrid + Vector2Int.up * 2;
                    AddGrid();
                }
                break;
        }

        return possibleMoveGrids;


    }
}
