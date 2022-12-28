using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Vector2 boardOffset;
    public Vector2 gridSize;

    BoardManager boardManager;
    Dictionary<int, string> chessDic =
        new Dictionary<int, string>()
        {
            {1,"KingW"},{2,"QueenW"},{3,"BishopW"},{4,"KnightW"},{5,"RookW"},{6,"PawnW"},
            {-1,"KingB"},{-2,"QueenB"},{-3,"BishopB"},{-4,"KnightB"},{-5,"RookB"},{-6,"PawnB"}
        };
    List<GameObject> chessTemp = new List<GameObject>();


    void Awake()
    {
        boardManager = GetComponentInParent<BoardManager>();
    }

    public Vector2 TransformPosition(Vector2Int grid) =>
        (Vector2)transform.position +
        boardOffset +
        new Vector2(grid.x * gridSize.x, grid.y * gridSize.y);

    public void DrawChesses(PlayerType playerType)
    {
        ClearBoard();
        var board = boardManager.GetBoard(playerType);
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var chessID = board[y, x];
                if (chessID != 0)
                {
                    var chessPrefab = Resources.Load<GameObject>($"Chesses/{chessDic[chessID]}");
                    var position = TransformPosition(new Vector2Int(x, y));
                    var rotation = Quaternion.identity;
                    var chess = Instantiate(chessPrefab, position, rotation);
                    chessTemp.Add(chess);
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
