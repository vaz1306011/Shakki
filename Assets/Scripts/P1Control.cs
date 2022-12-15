using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class P1Control : MonoBehaviour
{
    [Header("�����")]

    [SerializeField]
    GameObject selectBox;

    [SerializeField]
    GameObject selectedBox;

    [SerializeField]
    GameObject canMoveBox;

    [Header("�ѽL")]

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

    //�ѽL�y����@�ɦ�m
    Vector2 TransformBoardPosition(int x, int y) => new Vector2(
        board.boardOffset.x + x * board.gridSize.x,
        board.boardOffset.y + y * board.gridSize.y
    );

    //����ش�ID
    int selectChessID => boardManager.Board[selectBoxPosition.y, selectBoxPosition.x];

    //�w����ش�ID
    int selectedChessID => boardManager.Board[selectedBoxPosition.y, selectedBoxPosition.x];

    int chessID(Vector2Int pos) => boardManager.Board[pos.y, pos.x];

    bool isOutSideBoard(Vector2Int pos) => pos.x < 0 || pos.y < 0 || pos.x > 7 || pos.y > 7;

    bool isEnemy(int chess) => chess < 0;

    bool isAllies(int chess) => chess > 0;

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
            //�������
            if (!isSelect && isAllies(selectChessID))
            {
                selectedBoxPosition = selectBoxPosition;
                canMovePositions = GetCanMovePositions();
                foreach (var pos in canMovePositions)
                {
                    var box = Instantiate(canMoveBox, TransformBoardPosition(pos.x, pos.y), Quaternion.identity);
                    canMoveBoxsTemp.Add(box);
                }
                isSelect = true;
            }
            //�w�����
            if (isSelect)
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
        var Positions = new List<Vector2Int>();
        Vector2Int pos;
        void LineWalk(int x, int y)
        {
            for (var i = 1; i <= 7; i++)
            {
                pos = selectedBoxPosition + new Vector2Int(i * x, i * y);
                if (isOutSideBoard(pos) || isAllies(chessID(pos)))
                    return;
                Positions.Add(pos);
                if (isEnemy(chessID(pos)))
                    return;
            }
        }
        switch (selectedChessID)
        {
            //���
            case 1:
                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j <= 1; j++)
                        if (!(i == 0 && j == 0))
                        {
                            pos = selectedBoxPosition + new Vector2Int(i, j);
                            Positions.Add(pos);
                        }
                break;
            //�ӦZ 
            case 2:
                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j <= 1; j++)
                        LineWalk(i, j);
                break;
            //�D��
            case 3:
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                        LineWalk(i, j);

                break;
            //�M�h
            case 4:
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                    {
                        pos = selectedBoxPosition + new Vector2Int(1 * i, 2 * j);
                        if (isOutSideBoard(pos) || isAllies(chessID(pos)))
                            continue;
                        Positions.Add(pos);
                    }
                for (var i = -1; i <= 1; i += 2)
                    for (var j = -1; j <= 1; j += 2)
                    {
                        pos = selectedBoxPosition + new Vector2Int(2 * i, 1 * j);
                        if (isOutSideBoard(pos) || isAllies(chessID(pos)))
                            break;
                        Positions.Add(pos);
                        if (isEnemy(chessID(pos)))
                            break;
                    }
                break;
            //����
            case 5:
                LineWalk(0, 1);
                LineWalk(0, -1);
                LineWalk(1, 0);
                LineWalk(-1, 0);
                break;
            //�h�L
            case 6:
                if (selectBoxPosition.y == 1)
                {
                    pos = selectedBoxPosition + Vector2Int.up * 2;
                    Positions.Add(pos);
                }
                pos = selectedBoxPosition + Vector2Int.up;
                if (isOutSideBoard(pos) || isAllies(chessID(pos)))
                    break;
                Positions.Add(pos);
                break;
        }
        Positions.RemoveAll(pos => isOutSideBoard(pos) || isAllies(chessID(pos)));
        return Positions;
    }


    void SelectBoxUpdate()
    {
        selectBox.transform.position = TransformBoardPosition(selectBoxPosition.x, selectBoxPosition.y);
        selectedBox.transform.position = TransformBoardPosition(selectedBoxPosition.x, selectedBoxPosition.y);
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
