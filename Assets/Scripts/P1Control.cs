using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class P1Control : MonoBehaviour
{
    [Header("選取框")]

    [SerializeField]
    GameObject selectBox;

    [SerializeField]
    GameObject selectedBox;

    [SerializeField]
    GameObject canMoveBox;

    [Header("棋盤")]

    [SerializeField]
    P1Board board;

    [SerializeField]
    Vector2 boardOffset;

    [SerializeField]
    Vector2 gridSize;

    BoardManager boardManager;
    Vector2Int selectBoxPosition;
    Vector2Int selectedBoxPosition;
    List<GameObject> canMoveBoxsTemp;
    List<Vector2Int> canMovePositions;
    bool _isSelect;
    bool isSelect
    {
        get { return _isSelect; }
        set
        {
            _isSelect = value;
            if (!value)
            {
                while (canMoveBoxsTemp.Count > 0)
                {
                    Destroy(canMoveBoxsTemp[0]);
                    canMoveBoxsTemp.RemoveAt(0);
                }
            }
            selectedBox.SetActive(value);
        }
    }
    void Awake()
    {
        board.boardOffset = boardOffset;
        board.gridSize = gridSize;
    }

    void Start()
    {
        boardManager = GetComponentInParent<BoardManager>();
        selectBoxPosition = Vector2Int.zero;
        selectedBox.SetActive(false);
        canMoveBoxsTemp = new List<GameObject>();
        canMovePositions = new List<Vector2Int>();
        isSelect = false;
    }

    void Update()
    {
        PlayerInput();
    }

    Vector2 GetBoardPosition(int x, int y) => new Vector2(
        board.boardOffset.x + x * board.gridSize.x,
        board.boardOffset.y + y * board.gridSize.y
    );
    int selectChessID => boardManager.Board[selectBoxPosition.y, selectBoxPosition.x]; //選取框棋ID
    int selectedChessID => boardManager.Board[selectedBoxPosition.y, selectedBoxPosition.x]; //已選取框棋ID
    bool isBlack(int chess) => chess < 0;
    bool isWhite(int chess) => chess > 0;
    void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveUp();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveDown();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            //未選取時
            if (!isSelect && isWhite(selectChessID))
            {
                selectedBoxPosition = selectBoxPosition;
                canMovePositions = GetCanMovePositions();
                foreach (var pos in canMovePositions)
                {
                    var box = Instantiate(canMoveBox, GetBoardPosition(pos.x, pos.y), Quaternion.identity);
                    canMoveBoxsTemp.Add(box);
                }
                isSelect = true;
            }
            //已選取時
            if (isSelect && !isWhite(selectChessID))
            {
                if (canMovePositions.Exists(pos => pos == selectBoxPosition))
                {
                    boardManager.MoveChess(selectedBoxPosition, selectBoxPosition);
                    isSelect = false;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            isSelect = false;
        }
        SelectBoxUpdate();
    }

    List<Vector2Int> GetCanMovePositions()
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
        var Positions = new List<Vector2Int>();


        //FIXME 移動路徑有人後面就不能走了、友軍判斷
        switch (selectedChessID)
        {
            //國王
            case 1:
                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j <= 1; j++)
                        if (!(i == 0 && j == 0))
                        {
                            var pos = selectedBoxPosition + new Vector2Int(i, j);
                            Positions.Add(pos);
                        }
                break;
            //皇后 
            case 2:
                for (var i = 1; i <= 7; i++)
                {
                    var pos = selectedBoxPosition + new Vector2Int(i, i);
                    Positions.Add(pos);
                    pos = selectedBoxPosition + new Vector2Int(-i, i);
                    Positions.Add(pos);
                    pos = selectedBoxPosition + new Vector2Int(i, -i);
                    Positions.Add(pos);
                    pos = selectedBoxPosition + new Vector2Int(-i, -i);
                    Positions.Add(pos);
                }
                break;
            //主教
            case 3:

                break;
            //騎士
            case 4:

                break;
            //城堡
            case 5:

                break;
            //士兵
            case 6:

                break;

        }
        Positions.RemoveAll(p => p.x < 0 || p.y < 0 || p.x > 7 || p.y > 7);

        return Positions;
    }


    void SelectBoxUpdate()
    {
        selectBox.transform.position = GetBoardPosition(selectBoxPosition.x, selectBoxPosition.y);
        selectedBox.transform.position = GetBoardPosition(selectedBoxPosition.x, selectedBoxPosition.y);
    }

    void MoveUp()
    {
        if (selectBoxPosition.y < 7)
        {
            selectBoxPosition.y += 1;
        }
    }

    void MoveDown()
    {
        if (selectBoxPosition.y > 0)
        {
            selectBoxPosition.y -= 1;
        }
    }

    void MoveLeft()
    {
        if (selectBoxPosition.x > 0)
        {
            selectBoxPosition.x -= 1;
        }
    }

    void MoveRight()
    {
        if (selectBoxPosition.x < 7)
        {
            selectBoxPosition.x += 1;
        }
    }
}
