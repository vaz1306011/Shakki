using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class StringEvent : UnityEvent<string> { }

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
     * �ĪG��:7
     */
    public bool[,] CanCastling;
    public static bool KillKingWin = false;
    [SerializeField] GameObject effectBox;
    [SerializeField] StringEvent GameOver;


    Controler p1Controler, p2Controler;
    int[,] _board;

    Vector2Int _effectGrid;
    float _nextEffectTime;


    void Awake()
    {
        ResetBoard();
    }

    void Start()
    {
        p1Controler = transform.Find("Player1/Controler").GetComponent<Controler>();
        p2Controler = transform.Find("Player2/Controler").GetComponent<Controler>();
        _nextEffectTime = 1;
    }

    void Update()
    {
        if (_nextEffectTime > 0)
        {
            _nextEffectTime -= Time.deltaTime;
            if (_nextEffectTime <= 0)
            {
                while (true)
                {
                    var x = UnityEngine.Random.Range(0, 8);
                    var y = UnityEngine.Random.Range(0, 8);
                    if (SetEffecGrid(new Vector2Int(x, y)))
                        break;
                }
                _nextEffectTime = UnityEngine.Random.Range(2f, 5f);
            }
        }
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
        CanCastling = new bool[,] { { true, true }, { true, true } };
    }

    PlayerType? GetWinner()
    {
        bool IsWhiteWin()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (KillKingWin)
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
                    if (KillKingWin)
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
            CanCastling[0, way] = false;
        else if (playerType == PlayerType.Black)
            CanCastling[1, way] = false;
    }

    void unCastlingAll(PlayerType playerType)
    {
        if (playerType == PlayerType.White)
        {
            CanCastling[0, 0] = false;
            CanCastling[0, 1] = false;
        }
        else if (playerType == PlayerType.Black)
        {
            CanCastling[1, 0] = false;
            CanCastling[1, 1] = false;
        }
    }

    public void MoveChess(Vector2Int start, Vector2Int target, PlayerType playerType)
    {
        //�ন�դ��V
        if (playerType == PlayerType.Black)
        {
            start = Vector2Int.one * 7 - start;
            target = Vector2Int.one * 7 - target;
        }

        //�S���
        if (GetChessID(target) == 7)
        {
            if (playerType == PlayerType.White)
            {
                p2Controler.SetInputMap("Reverse");
                p2Controler.ReverseTime = UnityEngine.Random.Range(5f, 10f);
            }
            else if (playerType == PlayerType.Black)
            {
                p1Controler.SetInputMap("Reverse");
                p1Controler.ReverseTime = UnityEngine.Random.Range(5f, 10f);
            };
        }

        //����
        _board[target.y, target.x] = _board[start.y, start.x];
        _board[start.y, start.x] = 0;

        var winner = GetWinner();
        if (winner != null)
        {
            GameOver.Invoke(winner.ToString());
            return;
        }

        //�L����
        if (GetChessID(target) == (int)playerType * 6 &&
            (target.y == 7 || target.y == 0)
            )
            _board[target.y, target.x] = (int)playerType * 2;

        #region ��������
        //������ʧP�_
        if (GetChessID(target) == (int)playerType * 1)
            unCastlingAll(playerType);
        //�������ʧP�_
        if (GetChessID(target) == (int)playerType * 5)
        {
            if (start == new Vector2Int(0, 0)) //�ե�
                unCastling(PlayerType.White, 0);
            else if (start == new Vector2Int(7, 0)) //�եk
                unCastling(PlayerType.White, 1);
            else if (start == new Vector2Int(7, 7)) //�¥�
                unCastling(PlayerType.Black, 0);
            else if (start == new Vector2Int(0, 7)) //�¥k
                unCastling(PlayerType.Black, 1);
        }
        //�������
        if (GetChessID(target) == (int)playerType * 1 && Mathf.Abs(target.x - start.x) == 2)
        {
            if (target.x == 2) //��
            {
                if (GetChessID(new Vector2Int(0, target.y)) != (int)playerType * 5)
                    return;
                _board[target.y, 0] = 0;
                _board[target.y, target.x + 1] = (int)playerType * 5;
            }
            if (target.x == 6) //�k
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

    public int GetChessID(Vector2Int grid, PlayerType playerTpye = PlayerType.White) => GetBoard(playerTpye)[grid.y, grid.x];

    public Vector2Int TrasformOtherSideGrid(Vector2Int grid) => new Vector2Int(7 - grid.x, 7 - grid.y);

    public Vector2Int GetKingGrid(PlayerType playerType)
    {
        var board = GetBoard(playerType);
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
            {
                var chessGrid = new Vector2Int(y, x);
                if (GetChessID(chessGrid, playerType) == (int)playerType * 1)
                    return chessGrid;
            }
        return new Vector2Int(-1, -1);
    }

    public bool SetEffecGrid(Vector2Int grid)
    {
        if (GetChessID(grid) != 0)
            return false;

        _board[grid.y, grid.x] = 7;
        return true;
    }
}
