using System;
using UnityEngine;

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
    bool triggerState;

    public ImputTrigger(bool triggerState)
    {
        this.triggerState = triggerState;
    }
}

public interface IImput
{
    event EventHandler<ImputMoveEventArgs> OnImputMove;
    event EventHandler<ImputTrigger> OnTriggerChanged;
}