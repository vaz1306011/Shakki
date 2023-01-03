using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class StringEvent : UnityEvent<string> { }

public class BoardManager : MonoBehaviour
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
    public bool[,] canCastling;
    public static bool KillKingWin = false;
    [SerializeField] StringEvent GameOver;

    int[,] _board;
    void Awake()
    {
        ResetBoard();
    }

    public void ResetBoard()
    {
        _board = new int[,]{
            { 5, 4, 3, 2, 1, 3, 4, 5 },
            { 6, 6 ,6, 6, 6, 6, 6, 6 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { -6, -6 ,-6, -6, -6, -6, -6, -6 },
            { -5, -4, -3, -2, -1, -3, -4, -5 }
        };
        canCastling = new bool[,] { { true, true }, { true, true } };
    }

    PlayerType? GetWinner(bool killKingWin)
    {
        bool IsWhiteWin()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (killKingWin)
                    {
                        if (_board[i, j] == -1)
                            return false;
                    }
                    else
                    {
                        if (_board[i, j] < 0)
                            return false;
                    }
                }

            return true;
        }

        bool IsBlackWin()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (killKingWin)
                    {
                        if (_board[i, j] == 1)
                            return false;
                    }
                    else
                    {
                        if (_board[i, j] > 0)
                            return false;
                    }

            return true;
        }

        if (IsWhiteWin())
            return PlayerType.White;
        else if (IsBlackWin())
            return PlayerType.Black;

        return null;
    }

    void unCastling(PlayerType playerType, int way)
    {
        if (playerType == PlayerType.White)
            canCastling[0, way] = false;
        else if (playerType == PlayerType.Black)
            canCastling[1, way] = false;
    }

    void unCastlingAll(PlayerType playerType)
    {
        if (playerType == PlayerType.White)
        {
            canCastling[0, 0] = false;
            canCastling[0, 1] = false;
        }
        else if (playerType == PlayerType.Black)
        {
            canCastling[1, 0] = false;
            canCastling[1, 1] = false;
        }
    }

    public void MoveChess(PlayerType playerType, Vector2Int start, Vector2Int target)
    {
        //轉成白方方向
        if (playerType == PlayerType.Black)
        {
            start = Vector2Int.one * 7 - start;
            target = Vector2Int.one * 7 - target;
        }

        //移動
        _board[target.y, target.x] = _board[start.y, start.x];
        _board[start.y, start.x] = 0;

        var winner = GetWinner(KillKingWin);
        if (winner != null)
        {
            GameOver.Invoke(winner.ToString());
            return;
        }

        //兵升變
        if (GetChessID(target) == (int)playerType * 6 &&
            (target.y == 7 || target.y == 0)
            )
            _board[target.y, target.x] = (int)playerType * 2;

        #region 王車易位
        //國王移動判斷
        if (GetChessID(target) == (int)playerType * 1)
            unCastlingAll(playerType);
        //城堡移動判斷
        if (GetChessID(target) == (int)playerType * 5)
        {
            if (start == new Vector2Int(0, 0)) //白左
                unCastling(PlayerType.White, 0);
            else if (start == new Vector2Int(7, 0)) //白右
                unCastling(PlayerType.White, 1);
            else if (start == new Vector2Int(7, 7)) //黑左
                unCastling(PlayerType.Black, 0);
            else if (start == new Vector2Int(0, 7)) //黑右
                unCastling(PlayerType.Black, 1);
        }
        //易位執行
        if (GetChessID(target) == (int)playerType * 1 && Mathf.Abs(target.x - start.x) == 2)
        {
            if (target.x == 2) //左
            {
                if (GetChessID(new Vector2Int(0, target.y)) != (int)playerType * 5)
                    return;
                _board[target.y, 0] = 0;
                _board[target.y, target.x + 1] = (int)playerType * 5;
            }
            if (target.x == 6) //右
            {
                if (GetChessID(new Vector2Int(7, target.y)) != (int)playerType * 5)
                    return;
                _board[target.y, 7] = 0;
                _board[target.y, target.x - 1] = (int)playerType * 5;
            }
            unCastlingAll(playerType);
        }
        #endregion
    }

    public int[,] GetBoard(PlayerType playerType)
    {
        if (playerType == PlayerType.White)
            return this._board;

        var board = new int[8, 8];

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                board[7 - i, 7 - j] = this._board[i, j];

        return board;
    }

    public int GetChessID(PlayerType playerTpye, Vector2Int grid) => GetBoard(playerTpye)[grid.y, grid.x];

    public int GetChessID(Vector2Int grid) => GetBoard(PlayerType.White)[grid.y, grid.x];

    public Vector2Int GetKingGrid(PlayerType playerType)
    {
        var board = GetBoard(playerType);
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
            {
                var chessGrid = new Vector2Int(y, x);
                if (GetChessID(playerType, chessGrid) == (int)playerType * 1)
                    return chessGrid;
            }
        return new Vector2Int(-1, -1);
    }
}
