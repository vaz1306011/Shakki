using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Board : MonoBehaviour
{
    public Vector2 boardOffset;
    public Vector2 gridSize;

    Dictionary<int, string> chessDic =
        new Dictionary<int, string>()
        {
            {1,"KingW"},{2,"QueenW"},{3,"BishopW"},{4,"KnightW"},{5,"RookW"},{6,"PawnW"},
            {-1,"KingB"},{-2,"QueenB"},{-3,"BishopB"},{-4,"KnightB"},{-5,"RookB"},{-6,"PawnB"}
        };
    List<GameObject> chessTemp = new List<GameObject>();

    public Vector2 TransformPosition(Vector2Int frame) =>
        (Vector2)transform.position +
        boardOffset +
        new Vector2(frame.x * gridSize.x, frame.y * gridSize.y);

    public void DrawChesses(PlayerType player)
    {
        ClearBoard();
        var board = GetComponentInParent<BoardManager>().GetBoard(player);
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var chessID = board[y, x];
                if (chessID != 0)
                {
                    var chess = Resources.Load<GameObject>($"Chesses/{chessDic[chessID]}");
                    var pos = TransformPosition(new Vector2Int(x, y));
                    var rot = Quaternion.identity;
                    var obj = Instantiate(chess, pos, rot);
                    chessTemp.Add(obj);
                }
            }
        }
    }

    public void ClearBoard()
    {
        foreach (var ct in chessTemp)
            Destroy(ct);
        chessTemp.Clear();
    }
}
