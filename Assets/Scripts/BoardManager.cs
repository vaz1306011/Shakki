using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int[,] Board;
    P1Board P1Board;
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
    void Awake()
    {
        ResetBoard();
    }

    private void Start()
    {
        P1Board = GetComponentInChildren<P1Board>();
    }

    void ResetBoard()
    {
        Board = new int[,]{
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

    public void MoveChess(Vector2Int start, Vector2Int target)
    {
        Board[target.y, target.x] = Board[start.y, start.x];
        Board[start.y, start.x] = 0;
        P1Board.UpdateBoard();
    }
}
