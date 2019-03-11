using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMouse : InputBase
{
    int maxPlayer = 4;
    int currentPlayer = 1;

    public InputMouse()
    {
        inputName = "Mouse input";
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Init()
    {
        base.Init();
    }

    public override void UpdateInput()
    {
        base.UpdateInput();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if space bar is pressed 
        // move current player
        if (Input.GetKey(KeyCode.Space))
        {
            Vector2 screenResolution = new Vector2(Screen.width, Screen.height);

            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            mousePos = mousePos / screenResolution;
            InputMove("Mouse_" + currentPlayer, mousePos);
        }

        // switch user with rigth mouse buton
        if (Input.GetMouseButtonDown(1))
        {
            currentPlayer++;
            currentPlayer = currentPlayer % maxPlayer;

        }
    }
}
