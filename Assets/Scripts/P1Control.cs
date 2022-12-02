using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class P1Control : MonoBehaviour
{
    [Header("¿ï¨ú®Ø")]
    [SerializeField]
    GameObject selectBox;


    [Header("´Ñ½L")]
    [SerializeField]
    P1Board board;
    [SerializeField]
    Vector2 boardOffset;
    [SerializeField]
    Vector2 gridSize;

    Vector2Int selectBoxPos;
    bool isSelect;
    Vector2Int startPos, endPos;
    void Awake()
    {
        board.boardOffset = boardOffset;
        board.gridSize = gridSize;
    }

    void Start()
    {
        selectBoxPos = Vector2Int.zero;
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
            if (!isSelect)
            {
                startPos = selectBoxPos;
                isSelect = true;
            }
            else
            {
                endPos = selectBoxPos;
                GetComponentInParent<BoardManager>().MoveChess(startPos, endPos);
                isSelect = false;
            }
        }
    }

    void SelectBoxUpdate()
    {
        selectBox.transform.position = new Vector2(
            board.boardOffset.x + selectBoxPos.x * board.gridSize.x,
            board.boardOffset.y + selectBoxPos.y * board.gridSize.y
        );
    }

    void MoveUp()
    {
        if (selectBoxPos.y < 7)
        {
            selectBoxPos.y += 1;
        }
        SelectBoxUpdate();
    }

    void MoveDown()
    {
        if (selectBoxPos.y > 0)
        {
            selectBoxPos.y -= 1;
        }
        SelectBoxUpdate();
    }

    void MoveLeft()
    {
        if (selectBoxPos.x > 0)
        {
            selectBoxPos.x -= 1;
        }
        SelectBoxUpdate();
    }

    void MoveRight()
    {
        if (selectBoxPos.x < 7)
        {
            selectBoxPos.x += 1;
        }
        SelectBoxUpdate();
    }
}
