using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Control : MonoBehaviour
{
    [SerializeField]
    GameObject selectBox;
    [SerializeField]
    Vector2 boardOffset;
    [SerializeField]
    Vector2 gridSize;

    static int x = 0;
    static int y = 0;
    void Update()
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
        SelectBoxUpdate();
    }
    void SelectBoxUpdate()
    {
        selectBox.transform.position = new Vector2(
            boardOffset.x + x * gridSize.x,
            boardOffset.y + y * gridSize.y
        );
    }
    void MoveUp()
    {
        if (y < 7)
        {
            y += 1;
        }
        SelectBoxUpdate();
    }
    void MoveDown()
    {
        if (y > 0)
        {
            y -= 1;
        }
        SelectBoxUpdate();
    }
    void MoveLeft()
    {
        if (x > 0)
        {
            x -= 1;
        }
        SelectBoxUpdate();
    }
    void MoveRight()
    {
        if (x < 7)
        {
            x += 1;
        }
        SelectBoxUpdate();
    }
}
