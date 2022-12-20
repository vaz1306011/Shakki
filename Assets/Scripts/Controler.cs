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
    [SerializeField] KeyCode up;
    [SerializeField] KeyCode down;
    [SerializeField] KeyCode left;
    [SerializeField] KeyCode right;
    [SerializeField] KeyCode confirm;
    [SerializeField] KeyCode cancel;


    BoardManager boardManager;
    Vector2Int selectBoxFrame;
    Vector2Int selectedBoxFrame;
    List<GameObject> possibleMoveBoxsTemp = new List<GameObject>();
    List<Vector2Int> possibleMoveFrame = new List<Vector2Int>();
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
    //TODO 王車易位
    bool canCastling;

    void Start()
    {
        boardManager = GetComponentInParent<BoardManager>();
        board.DrawChesses(player);
        selectBoxFrame = Vector2Int.zero;
        selectedBox.SetActive(false);
        isSelect = false;
        canCastling = true;
    }

    void Update()
    {
        PlayerInput();
    }

    //選取框棋ID
    int SelectChessID => boardManager.GetChessID(player, selectBoxFrame);

    //已選取框棋ID
    int SelectedChessID => boardManager.GetChessID(player, selectedBoxFrame);

    int GetChessID(Vector2Int pos) => boardManager.GetChessID(player, pos);

    bool IsOutSideBoard(Vector2Int pos) => pos.x < 0 || pos.y < 0 || pos.x > 7 || pos.y > 7;

    bool IsEnemy(int chess) => player == PlayerType.white ? chess < 0 : chess > 0;

    bool IsAllies(int chess) => player == PlayerType.white ? chess > 0 : chess < 0;

    void Move(Vector2Int direction)
    {
        selectBoxFrame += direction;

        if (selectBoxFrame.x < 0)
            selectBoxFrame.x = 0;
        if (selectBoxFrame.y < 0)
            selectBoxFrame.y = 0;
        if (selectBoxFrame.x > 7)
            selectBoxFrame.x = 7;
        if (selectBoxFrame.y > 7)
            selectBoxFrame.y = 7;

        selectBox.transform.position = board.TransformPosition(selectBoxFrame);
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
            if (!isSelect && IsAllies(SelectChessID))
            {
                selectedBoxFrame = selectBoxFrame;
                possibleMoveFrame = GetPossibleMovesFrame();
                foreach (var pos in possibleMoveFrame)
                {
                    var box = Instantiate(possibleMoveBox, board.TransformPosition(pos), Quaternion.identity);
                    possibleMoveBoxsTemp.Add(box);
                }
                isSelect = true;
                selectedBox.transform.position = board.TransformPosition(selectedBoxFrame);
            }
            //確認
            if (isSelect)
            {
                if (possibleMoveFrame.Exists(pos => pos == selectBoxFrame))
                {
                    boardManager.MoveChess(player, selectedBoxFrame, selectBoxFrame);
                    isSelect = false;
                }
            }
        }
        if (Input.GetKeyDown(cancel))
            isSelect = false;

        if (isSelect)
            if (IsEnemy(GetChessID(selectedBoxFrame)))
                isSelect = false;

        board.DrawChesses(player);
    }

    List<Vector2Int> GetPossibleMovesFrame()
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
        var possibleMovesFrame = new List<Vector2Int>();
        Vector2Int pos;
        void LineWalk(int x, int y)
        {
            for (var i = 1; i <= 7; i++)
            {
                pos = selectedBoxFrame + new Vector2Int(i * x, i * y);
                if (IsOutSideBoard(pos) || IsAllies(GetChessID(pos)))
                    return;
                possibleMovesFrame.Add(pos);
                if (IsEnemy(GetChessID(pos)))
                    return;
            }
        }
        switch (SelectedChessID)
        {
            //國王
            case 1:
            case -1:
                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j <= 1; j++)
                        if (!(i == 0 && j == 0))
                        {
                            pos = selectedBoxFrame + new Vector2Int(i, j);
                            possibleMovesFrame.Add(pos);
                        }
                break;

            //皇后 
            case 2:
            case -2:
                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j <= 1; j++)
                        LineWalk(i, j);
                break;

            //主教
            case 3:
            case -3:
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                        LineWalk(i, j);

                break;

            //騎士
            case 4:
            case -4:
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                    {
                        pos = selectedBoxFrame + new Vector2Int(1 * i, 2 * j);
                        if (IsOutSideBoard(pos) || IsAllies(GetChessID(pos)))
                            continue;
                        possibleMovesFrame.Add(pos);
                    }
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                    {
                        pos = selectedBoxFrame + new Vector2Int(2 * i, 1 * j);
                        if (IsOutSideBoard(pos) || IsAllies(GetChessID(pos)))
                            break;
                        possibleMovesFrame.Add(pos);
                        if (IsEnemy(GetChessID(pos)))
                            break;
                    }
                break;

            //城堡
            case 5:
            case -5:
                LineWalk(0, 1);
                LineWalk(0, -1);
                LineWalk(1, 0);
                LineWalk(-1, 0);
                break;

            //士兵
            case 6:
            case -6:
                if (selectBoxFrame.y == 1)
                {
                    pos = selectedBoxFrame + Vector2Int.up * 2;
                    possibleMovesFrame.Add(pos);
                }
                pos = selectedBoxFrame + Vector2Int.up;
                if (IsOutSideBoard(pos) || IsAllies(GetChessID(pos)))
                    break;
                possibleMovesFrame.Add(pos);
                break;
        }
        possibleMovesFrame.RemoveAll(pos => IsOutSideBoard(pos) || IsAllies(GetChessID(pos)));
        return possibleMovesFrame;
    }
}
