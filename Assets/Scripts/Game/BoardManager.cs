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
     * 效果格:7
     */
    [SerializeField] GameObject effectBox;
    [SerializeField] StringEvent GameOver;

    public bool[,] CanCastling;
    public static bool KillKingWin = false;

    Controler p1Controler, p2Controler;
    int[,] _board;

    static float _nextEffectTime = 1;
    public static bool IsEffectGridEnable
    {
        get
        {
            return _nextEffectTime > 0;
        }
        set
        {
            if (value)
                _nextEffectTime = 1;
            else
                _nextEffectTime = 0;
        }
    }

    void Awake()
    {
        ResetBoard();
    }

    void Start()
    {
        p1Controler = transform.Find("Player1/Controler").GetComponent<Controler>();
        p2Controler = transform.Find("Player2/Controler").GetComponent<Controler>();
    }

    void Update()
    {
        if (_nextEffectTime > 0)
        {
            _nextEffectTime -= Time.deltaTime;
            if (_nextEffectTime <= 0)
            {
                bool hasEffectGrid = false;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        if (GetChessID(new Vector2Int(i, j)) == 7)
                        {
                            hasEffectGrid = true;
                            break;
                        }

                while (!hasEffectGrid)
                {
                    var x = UnityEngine.Random.Range(0, 8);
                    var y = UnityEngine.Random.Range(0, 8);
                    if (SetEffecGrid(new Vector2Int(x, y)))
                        break;
                }
                _nextEffectTime = UnityEngine.Random.Range(2f, 5f);
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            for (int i = 0; i < 8; i++)
                print(_board[i, 0] + " " + _board[i, 1] + " " + _board[i, 2] + " " + _board[i, 3] + " " + _board[i, 4] + " " + _board[i, 5] + " " + _board[i, 6] + " " + _board[i, 7]);

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
                        if (_board[i, j] > 0 && _board[i, j] != 7)
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

    void UnCastling(PlayerType playerType, int way)
    {
        if (playerType == PlayerType.White)
            CanCastling[0, way] = false;
        else if (playerType == PlayerType.Black)
            CanCastling[1, way] = false;
    }

    void UnCastlingAll(PlayerType playerType)
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
        //轉成白方方向
        if (playerType == PlayerType.Black)
        {
            start = Vector2Int.one * 7 - start;
            target = Vector2Int.one * 7 - target;
        }

        //特殊格
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

        //移動
        SetChess(GetChessID(start), target);
        SetChess(0, start);

        var winner = GetWinner();
        if (winner != null)
        {
            GameOver.Invoke(winner.ToString());
            return;
        }

        //兵升變
        if (GetChessID(target) == (int)playerType * 6 &&
            (target.y == 7 || target.y == 0)
            )
            SetChess((int)playerType * 2, target);

        #region 王車易位
        //國王移動判斷
        if (GetChessID(target) == (int)playerType * 1)
            UnCastlingAll(playerType);
        //城堡移動判斷
        if (GetChessID(target) == (int)playerType * 5)
        {
            if (start == new Vector2Int(0, 0)) //白左
                UnCastling(PlayerType.White, 0);
            else if (start == new Vector2Int(7, 0)) //白右
                UnCastling(PlayerType.White, 1);
            else if (start == new Vector2Int(7, 7)) //黑左
                UnCastling(PlayerType.Black, 0);
            else if (start == new Vector2Int(0, 7)) //黑右
                UnCastling(PlayerType.Black, 1);
        }
        //易位執行
        if (GetChessID(target) == (int)playerType * 1 && Mathf.Abs(target.x - start.x) == 2)
        {
            if (target.x == 2) //左
            {
                if (GetChessID(new Vector2Int(0, target.y)) != (int)playerType * 5)
                    return;
                SetChess(0, 0, target.y);
                SetChess((int)playerType * 5, target.x + 1, target.y);
            }
            if (target.x == 6) //右
            {
                if (GetChessID(new Vector2Int(7, target.y)) != (int)playerType * 5)
                    return;
                SetChess(0, 7, target.y);
                SetChess((int)playerType * 5, target.x - 1, target.y);
            }
            UnCastlingAll(playerType);
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

    void SetChess(int chessID, Vector2Int grid)
    {
        if (chessID < -6 || 7 < chessID)
            return;

        if (grid.x < 0 || 7 < grid.x || grid.y < 0 || 7 < grid.y)
            return;

        _board[grid.y, grid.x] = chessID;
    }

    void SetChess(int chessID, int x, int y)
    {
        if (chessID < -6 || 7 < chessID)
            return;

        if (x < 0 || 7 < x || y < 0 || 7 < y)
            return;

        _board[y, x] = chessID;
    }

    public Vector2Int? GetKingGrid(PlayerType playerType)
    {
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
            {
                var chessGrid = new Vector2Int(y, x);
                if (GetChessID(chessGrid, playerType) == (int)playerType * 1)
                    return chessGrid;
            }
        return null;
    }

    public bool SetEffecGrid(Vector2Int grid)
    {
        if (GetChessID(grid) != 0)
            return false;

        SetChess(7, grid);
        return true;
    }

    public void SwitchEffectGrid()
    {
        IsEffectGridEnable = !IsEffectGridEnable;
        if (!IsEffectGridEnable)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    var grid = new Vector2Int(i, j);
                    if (GetChessID(grid) == 7)
                        SetChess(0, grid);
                }
        }
    }

}
