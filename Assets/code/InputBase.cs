using System;
using UnityEngine;

public abstract class InputBase : MonoBehaviour
{ 
    public class ImputMoveEventArgs
    {
        string playerId;
        Vector2 pos;

        public ImputMoveEventArgs(string playerId, Vector2 pos)
        {
            this.playerId = playerId;
            this.pos = pos;
        }
    }


    public class ImputTrigger
    {
 
        string playerId;
        bool triggerState;

        public ImputTrigger(string playerId, bool triggerState)
        {
            this.playerId = playerId;
            this.triggerState = triggerState;
        }
    }

    public event EventHandler<ImputMoveEventArgs> OnImputMove;
    public event EventHandler<ImputTrigger> OnTriggerChanged;

    protected string inputName = "to_fill";

    public string InputName { get => inputName; }

    public virtual void Init()
    {

    }

    public virtual void UpdateInput()
    {

    }

    public virtual void Close()
    {
    }
}