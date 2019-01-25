using extOSC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particulesManager : MonoBehaviour
{
    Matrix4x4 particules = new Matrix4x4();

    public Matrix4x4 Particules { get => particules; set => particules = value; }

    // Start is called before the first frame update
    void Start()
    {
        OSCReceiver _receiver;
        // Creating a receiver.
        _receiver = gameObject.AddComponent<OSCReceiver>();
        // Set local port.
        _receiver.LocalPort = 8000;
        _receiver.Bind("/*", MessageReceived);


        // init particules pos
        particules.SetRow(0, new Vector4(-0.5f, 0.5f, 0.0f, 0.2f));
        particules.SetRow(1, new Vector4(0.5f, 0.5f, 0.0f, 0.2f));
        particules.SetRow(2, new Vector4(0.5f, -0.5f, 0.0f, 0.2f));
        particules.SetRow(3, new Vector4(-0.5f, -0.5f, 0.0f, 0.2f));
    }

    private void MessageReceived(OSCMessage msg)
    {
        OSCValueType[] types = { OSCValueType.Float };

        if (msg.Address.Equals("/3/xyM_l"))
        {
           
            OSCValue[] vals = msg.GetValues(types);

            //Debug.Log("Vals received ["+vals.Length+"] - ["+ vals[0].FloatValue + ","+ vals[1].FloatValue + "]");

            particules[0, 0] = -0.5f + (vals[1].FloatValue - 0.5f );
            particules[0, 1] = 0.5f + ( vals[0].FloatValue - 0.5f );
        }
        else if (msg.Address.Equals("/3/xyM_r"))
        {
            OSCValue[] vals = msg.GetValues(types);
            particules[1, 0] = 0.5f + (vals[1].FloatValue - 0.5f);
            particules[1, 1] = 0.5f + (vals[0].FloatValue - 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
