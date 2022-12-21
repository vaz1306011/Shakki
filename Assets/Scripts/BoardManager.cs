using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class BoardManager : MonoBehaviour
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
    public bool[,] canCastling;

    int[,] board;
    void Awake()
    {
        ResetBoard();
    }

    public void ResetBoard()
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
        canCastling = new bool[,] { { true, true }, { true, true } };
    }

    void unCastling(PlayerType player, int way)
    {
        if (player == PlayerType.white)
            canCastling[0, way] = false;
        else if (player == PlayerType.black)
            canCastling[1, way] = false;
    }

    void unCastling(PlayerType player)
    {
        if (player == PlayerType.white)
        {
            canCastling[0, 0] = false;
            canCastling[0, 1] = false;
        }
        else if (player == PlayerType.black)
        {
            canCastling[1, 0] = false;
            canCastling[1, 1] = false;
        }
    }

    public void MoveChess(PlayerType player, Vector2Int start, Vector2Int target)
    {
        //�ন�դ��V
        if (player == PlayerType.black)
        {
            start = Vector2Int.one * 7 - start;
            target = Vector2Int.one * 7 - target;
        }

        //����
        board[target.y, target.x] = board[start.y, start.x];
        board[start.y, start.x] = 0;

        //�L����
        if (GetChessID(target) == (int)player * 6 && target.y == 7)
            board[target.y, target.x] = (int)player * 2;


        #region ��������
        //������ʧP�_
        if (GetChessID(target) == (int)player * 1)
            unCastling(player);
        //�������ʧP�_
        if (GetChessID(target) == (int)player * 5)
        {
            if (start == new Vector2Int(0, 0)) //�ե�
                unCastling(PlayerType.white, 0);
            else if (start == new Vector2Int(7, 0)) //�եk
                unCastling(PlayerType.white, 1);
            else if (start == new Vector2Int(7, 7)) //�¥�
                unCastling(PlayerType.black, 0);
            else if (start == new Vector2Int(0, 7)) //�¥k
                unCastling(PlayerType.black, 1);
        }
        //�������
        if (GetChessID(target) == (int)player * 1 && Math.Abs(target.x - start.x) == 2)
        {
            if (target.x == 2) //��
            {
                if (GetChessID(new Vector2Int(0, target.y)) != (int)player * 5)
                    return;
                board[target.y, 0] = 0;
                board[target.y, target.x + 1] = (int)player * 5;
            }
            if (target.x == 6) //�k
            {
                if (GetChessID(new Vector2Int(7, target.y)) != (int)player * 5)
                    return;
                board[target.y, 7] = 0;
                board[target.y, target.x - 1] = (int)player * 5;
            }
            unCastling(player);
        }
        #endregion
    }

    public int[,] GetBoard(PlayerType player)
    {
        if (player == PlayerType.white)
            return this.board;

        var board = new int[8, 8];

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                board[7 - i, 7 - j] = this.board[i, j];

        return board;
    }

    public int GetChessID(PlayerType player, Vector2Int grid) => GetBoard(player)[grid.y, grid.x];
    public int GetChessID(Vector2Int grid) => GetBoard(PlayerType.white)[grid.y, grid.x];

}
