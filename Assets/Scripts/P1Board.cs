using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class P1Board : MonoBehaviour
{
    [HideInInspector]
    public Vector2 boardOffset;
    [HideInInspector]
    public Vector2 gridSize;

    public Dictionary<int, string> chessDic;
    void Start()
    {
        chessDic = new Dictionary<int, string>()
        {
            {1,"KingW"},{2,"QueenW"},{3,"BishopW"},{4,"KnightW"},{5,"RookW"},{6,"PawnW"},
            {-1,"KingB"},{-2,"QueenB"},{-3,"BishopB"},{-4,"KnightB"},{-5,"RookB"},{-6,"PawnB"}
        };
        DrawBoard();
    }

    public void UpdateBoard()
    {
        ClearBoard();
        DrawBoard();
    }

    void DrawBoard()
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
                    var pos = new Vector2(boardOffset.x + x * gridSize.x, boardOffset.y + y * gridSize.y);
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
