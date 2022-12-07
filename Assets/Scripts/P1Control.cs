using System.Collections;
using System.Collections.Generic;
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

    [Header("�ѽL")]
    [SerializeField]
    P1Board board;
    [SerializeField]
    Vector2 boardOffset;
    [SerializeField]
    Vector2 gridSize;

    Vector2Int selectBoxPos;
    Vector2Int selectedBoxPos;
    bool isSelect;
    void Awake()
    {
        board.boardOffset = boardOffset;
        board.gridSize = gridSize;
    }

    void Start()
    {
        selectBoxPos = Vector2Int.zero;
        selectedBox.SetActive(false);
        isSelect = false;
    }

    void Update()
    {
        PlayerInput();
    }

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
            var boardManager = GetComponentInParent<BoardManager>(); //�{�b���ѽL���A
            var chessID = boardManager.Board[selectBoxPos.y, selectBoxPos.x]; //����إثe��ܪ���ID
            //var selectSpriteRenderer = selectBox.GetComponent<SpriteRenderer>(); //����ضK��
            if (!isSelect && isWhite(chessID))
            {
                selectedBoxPos = selectBoxPos;
                isSelect = true;

            }
            if (isSelect && !isWhite(chessID))
            {
                boardManager.MoveChess(selectedBoxPos, selectBoxPos);
                isSelect = false;
            }
            selectedBox.SetActive(isSelect);
        }
        SelectBoxUpdate();
    }
    Vector2 getBoardPos(int x, int y) => new Vector2(
        board.boardOffset.x + x * board.gridSize.x,
        board.boardOffset.y + y * board.gridSize.y
    );
    void SelectBoxUpdate()
    {
        selectBox.transform.position = getBoardPos(selectBoxPos.x, selectBoxPos.y);
        selectedBox.transform.position = getBoardPos(selectedBoxPos.x, selectedBoxPos.y);
    }

    void MoveUp()
    {
        if (selectBoxPos.y < 7)
        {
            selectBoxPos.y += 1;
        }
    }

    void MoveDown()
    {
        if (selectBoxPos.y > 0)
        {
            selectBoxPos.y -= 1;
        }
    }

    void MoveLeft()
    {
        if (selectBoxPos.x > 0)
        {
            selectBoxPos.x -= 1;
        }
    }

    void MoveRight()
    {
        if (selectBoxPos.x < 7)
        {
            selectBoxPos.x += 1;
        }
    }
}
