﻿using extOSC;
using extOSC.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OSCSource : MonoBehaviour
{
    //public string bindIP = "127.0.0.1";
    public int bindPort = 8000;
    public int replyPort = 9000;
    public bool useGivenIP = false;
    public string localIP = "";

    OSCReceiver _receiver;
    OSCTransmitter _transmiter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        _receiver = gameObject.GetComponent<OSCReceiver>();
        _receiver.LocalPort = bindPort;
        if (useGivenIP)
        {
            _receiver.LocalIP = localIP;
        }
        else
        {
            _receiver.LocalIP = ""; // local ip will be selected by system
        }
        

        _transmiter = gameObject.GetComponent<OSCTransmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public OSCBind BindAddress(string address, UnityAction<OSCMessage> callback)
    {
        return _receiver.Bind(address, callback);
    }

    public void Send(string ip, OSCPacket packet)
    {
        _transmiter.RemoteHost = ip;
        _transmiter.RemotePort = replyPort;

        _transmiter.Send(packet);
    }
}
