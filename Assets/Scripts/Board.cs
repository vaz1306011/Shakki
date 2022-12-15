using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Board : MonoBehaviour
{
    [HideInInspector] public Vector2 boardOffset;
    [HideInInspector] public Vector2 gridSize;

    Dictionary<int, string> chessDic;
    void Start()
    {
        chessDic = new Dictionary<int, string>()
        {
            {1,"KingW"},{2,"QueenW"},{3,"BishopW"},{4,"KnightW"},{5,"RookW"},{6,"PawnW"},
            {-1,"KingB"},{-2,"QueenB"},{-3,"BishopB"},{-4,"KnightB"},{-5,"RookB"},{-6,"PawnB"}
        };
    }

    public void UpdateP1Board()
    {
        ClearBoard();
        P1DrawBoard();
    }
    public void UpdateP2Board()
    {
        ClearBoard();
        P2DrawBoard();
    }

    void P1DrawBoard()
    {
        var board = GetComponentInParent<BoardManager>().Board;
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var chessID = board[y, x];
                if (chessID != 0)
                {
                    var chess = Resources.Load<GameObject>($"Chesses/{chessDic[chessID]}");
                    var pos = (Vector2)transform.position +
                        new Vector2(boardOffset.x, boardOffset.y) +
                        new Vector2(x * gridSize.x, y * gridSize.y);
                    var rot = Quaternion.identity;
                    Instantiate(chess, pos, rot);
                }
            }
        }
    }
    void P2DrawBoard()
    {
        var board = GetComponentInParent<BoardManager>().Board;
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var chessID = board[y, x];
                if (chessID != 0)
                {
                    var chess = Resources.Load<GameObject>($"Chesses/{chessDic[chessID]}");
                    var pos = (Vector2)transform.position +
                        new Vector2(boardOffset.x, boardOffset.y) +
                        new Vector2((7 - x) * gridSize.x, (7 - y) * gridSize.y);
                    var rot = Quaternion.identity;
                    Instantiate(chess, pos, rot);
                }
            }
        }
    }

    void ClearBoard()
    {
        foreach (var c in GameObject.FindGameObjectsWithTag("Chess"))
        {
            Destroy(c);
        }
    }
}
