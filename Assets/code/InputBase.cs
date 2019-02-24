using System;
using UnityEngine;

public class InputBase : MonoBehaviour
{
    public class ImputMoveEventArgs
    {
        public string playerId;
        public Vector2 pos;

        public ImputMoveEventArgs(string playerId, Vector2 pos)
        {
            this.playerId = playerId;
            this.pos = pos;
        }
    }


    public class ImputTrigger
    {

        public string playerId;
        public bool triggerState;

        public ImputTrigger(string playerId, bool triggerState)
        {
            this.playerId = playerId;
            this.triggerState = triggerState;
        }
    }

    public event EventHandler<ImputMoveEventArgs> OnInputMove;
    public event EventHandler<ImputTrigger> OnTriggerChanged;

    protected string inputName = "to_fill";

    public string InputName { get => inputName; }

    public void InputMove( string playerId, Vector2 pos)
    {
        OnInputMove?.Invoke(this, new ImputMoveEventArgs(playerId, pos));
    }

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