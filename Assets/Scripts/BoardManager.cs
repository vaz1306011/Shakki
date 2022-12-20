using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
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

    int[,] board;
    void Awake()
    {
        ResetBoard();
    }

    void ResetBoard()
    {
        board = new int[,]{
            { 5, 4, 3, 2, 1, 3, 4, 5 },
            { 6, 6 ,6, 6, 6, 6, 6, 6 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { -6, -6 ,-6, -6, -6, -6, -6, -6 },
            { -5, -4, -3, -2, -1, -3, -4, -5 }
        };
    }

    public void MoveChess(PlayerType player, Vector2Int start, Vector2Int target)
    {
        switch (player)
        {
            case PlayerType.white:
                board[target.y, target.x] = board[start.y, start.x];
                if (GetChessID(player, target) == 6 && target.y == 7)
                    board[target.y, target.x] = 2;
                board[start.y, start.x] = 0;
                break;

            case PlayerType.black:
                board[7 - target.y, 7 - target.x] = board[7 - start.y, 7 - start.x];
                board[7 - start.y, 7 - start.x] = 0;
                if (GetChessID(player, target) == -6 && target.y == 7)
                    board[7 - target.y, 7 - target.x] = -2;
                break;
        }
    }

    public int[,] GetBoard(PlayerType player)
    {
        if (player == PlayerType.white)
            return board;

        var p2Board = new int[8, 8];

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                p2Board[7 - i, 7 - j] = board[i, j];

        return p2Board;
    }

    public int GetChessID(PlayerType player, Vector2Int frame) => GetBoard(player)[frame.y, frame.x];

}
