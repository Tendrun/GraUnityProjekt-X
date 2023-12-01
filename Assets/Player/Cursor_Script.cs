using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor_Script : MonoBehaviour
{
    public GameObject CursorObject;
    public CanvasGroup AlphaColor;

    private void Awake()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 mouseCursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CursorObject.transform.position = mouseCursorPos;
    }

    public void DisableCursor()
    {
        Cursor.visible = false;
    }

    public void EnableCursor()
    {
        Cursor.visible = true;
    }
}
