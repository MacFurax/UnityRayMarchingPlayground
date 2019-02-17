using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputOSC : IImput
{
    public event EventHandler<ImputMoveEventArgs> OnImputMove;
    public event EventHandler<ImputTrigger> OnTriggerChanged;
}
