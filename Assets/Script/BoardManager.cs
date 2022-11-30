using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    Vector2 boardOffset;
    [SerializeField]
    Vector2 gridSize;
    [SerializeField]
    GameObject king;
    [SerializeField]
    GameObject queen;
    [SerializeField]
    GameObject bishop;
    [SerializeField]
    GameObject knight;
    [SerializeField]
    GameObject rook;
    [SerializeField]
    GameObject pawn;
    /* 
     * 空:0
     * 國王:1
     * 皇后:2
     * 主教:3
     * 騎士:4
     * 城堡:5
     * 士兵:6
     */
    static int[,] board = new int[,] {
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 }
    };
    void Start()
    {
        var x = 0;
        var y = 0;
        var pos = new Vector2(boardOffset.x + x * gridSize.x, boardOffset.y + y * gridSize.y);
        Instantiate(king, pos, transform.rotation);
    }
    void Update()
    {
        
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
    void DrawBoard()
    {
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                var pos = new Vector2(boardOffset.x + x * gridSize.x, boardOffset.y + y * gridSize.y);
                switch (board[x, y])
                {
                    case 1:
                        Instantiate(king, pos, transform.rotation);
                        break;
                }
            }
        }
    }
    void MoveChess(Vector2Int start, Vector2Int target)
    {
        if (isAllie(board[start.x, start.y]) && !isAllie(board[target.x, target.y]))
        {
            board[target.x, target.y] = board[start.x, start.y];
            board[start.x, start.y] = 0;
        }
    }
    bool isAllie(int chess)
    {
        return chess > 0;
    }
}
