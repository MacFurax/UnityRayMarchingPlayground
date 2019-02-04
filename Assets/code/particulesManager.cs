using extOSC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class particulesManager : MonoBehaviour
{
    Matrix4x4 particules = new Matrix4x4();
    AppConfig cfg;
    string cfgFilename = "config.xml";

    OSCTransmitter _oscSender;
    OSCReceiver _receiver;

    bool probPing = true;

    public Matrix4x4 Particules { get => particules; set => particules = value; }

    bool paramsChanged = false;
    DateTime lastParamChanges;

    // Start is called before the first frame update
    void Start()
    {
        // Creating a receiver.
        _receiver = gameObject.AddComponent<OSCReceiver>();
        // Set local port.
        _receiver.LocalPort = 8000;
        _receiver.Bind("/*", MessageReceived);

        _oscSender = gameObject.AddComponent<OSCTransmitter>();

        LoadConfig();

        // init particules pos
        //particules.SetRow(0, new Vector4(-0.5f, 0.5f, 0.0f, 0.2f));
        //particules.SetRow(1, new Vector4(0.5f, 0.5f, 0.0f, 0.2f));
        //particules.SetRow(2, new Vector4(0.5f, -0.5f, 0.0f, 0.2f));
        //particules.SetRow(3, new Vector4(-0.5f, -0.5f, 0.0f, 0.2f));

        particules.SetRow(0, cfg.ballsData[0]);
        particules[0, 0] = -0.5f + (cfg.ballsData[0].x - 0.5f) * 2.0f;
        particules[0, 1] = 0.5f + (cfg.ballsData[0].y - 0.5f) * 2.0f;

        particules.SetRow(1, cfg.ballsData[1]);
        particules[1, 0] = 0.5f + (cfg.ballsData[1].x - 0.5f) * 2.0f;
        particules[1, 1] = 0.5f + (cfg.ballsData[1].y - 0.5f) * 2.0f;

        particules.SetRow(2, cfg.ballsData[2]);
        particules.SetRow(3, cfg.ballsData[3]);

    }

    private void MessageReceived(OSCMessage msg)
    {
        OSCValueType[] types = { OSCValueType.Float };

        if (msg.Address.Equals("/prob"))
        {
            SendConfig(msg);
        }
        else if (msg.Address.Equals("/1/bgRed"))
        {
            OSCValue[] vals = msg.GetValues(types);
            cfg.bgColor.x = vals[0].FloatValue;
        }
        else if (msg.Address.Equals("/1/bgGreen"))
        {
            OSCValue[] vals = msg.GetValues(types);
            cfg.bgColor.y = vals[0].FloatValue;
        }
        else if (msg.Address.Equals("/1/bgBlue"))
        {
            OSCValue[] vals = msg.GetValues(types);
            cfg.bgColor.z = vals[0].FloatValue;
        }
        else if (msg.Address.Equals("/3/xyM_l") || msg.Address.Equals("/3/xy1"))
        {

            OSCValue[] vals = msg.GetValues(types);

            //Debug.Log("Vals received ["+vals.Length+"] - ["+ vals[0].FloatValue + ","+ vals[1].FloatValue + "]");

            particules[0, 0] = -0.5f + (vals[1].FloatValue - 0.5f) * 2.0f;
            particules[0, 1] = 0.5f + (vals[0].FloatValue - 0.5f) * 2.0f;
            cfg.ballsData[0].x = vals[1].FloatValue;
            cfg.ballsData[0].y = vals[0].FloatValue;
        }
        else if (msg.Address.Equals("/3/xyM_r") || msg.Address.Equals("/3/xy2"))
        {
            OSCValue[] vals = msg.GetValues(types);
            particules[1, 0] = 0.5f + (vals[1].FloatValue - 0.5f) * 2.0f;
            particules[1, 1] = 0.5f + (vals[0].FloatValue - 0.5f) * 2.0f;
            cfg.ballsData[1].x = vals[1].FloatValue;
            cfg.ballsData[1].y = vals[0].FloatValue;
        }
        else if (msg.Address.Equals("/faderM"))
        {
            OSCValue[] vals = msg.GetValues(types);
            particules[0, 3] = 0.2f * vals[0].FloatValue * 2.0f;
            particules[1, 3] = 0.2f * vals[0].FloatValue * 2.0f;
            particules[2, 3] = 0.2f * vals[0].FloatValue * 2.0f;
            particules[3, 3] = 0.2f * vals[0].FloatValue * 2.0f;
        }

        lastParamChanges = DateTime.Now;
        paramsChanged = true;
    }

    // Update is called once per frame
    void Update()
    {
        DateTime saveTime = lastParamChanges + TimeSpan.FromSeconds(2.0f);
        if (paramsChanged && DateTime.Now >= saveTime)
        {
            //Debug.Log("Save params...");
            WriteConfig();
            paramsChanged = false; 
        }
    }

    private void LoadConfig()
    {
        XmlSerializer serializer =
        new XmlSerializer(typeof(AppConfig));

        if (File.Exists(cfgFilename))
        {
            using (Stream reader = new FileStream(cfgFilename, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                cfg = (AppConfig)serializer.Deserialize(reader);
            }
        }
        else
        {
            cfg = new AppConfig();
            WriteConfig();
        }
    }

    private void WriteConfig()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));

        Stream fs = new FileStream(cfgFilename, FileMode.Create);
        XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
        // Serialize using the XmlTextWriter.
        serializer.Serialize(writer, cfg);
        writer.Close();
    }

    private void SendConfig(OSCMessage msg)
    {
        probPing = !probPing;

        //Debug.Log("SendConfig [" + msg.Ip +":"+msg.Port+"]");
        _oscSender.RemoteHost = msg.Ip.ToString();
        _oscSender.RemotePort = 9000;

        //Debug.Log("SendConfig to [" + _oscSender.RemoteHost + ":" + _oscSender.RemotePort + "]");

        var bundle = new OSCBundle();

        var message = new OSCMessage("/1/probLed");
        message.AddValue(OSCValue.Float(probPing ?1.0f:0.0f));
        bundle.AddPacket(message);

        message = new OSCMessage("/1/bgRed");
        message.AddValue(OSCValue.Float(cfg.bgColor.x));
        bundle.AddPacket(message);

        message = new OSCMessage("/1/bgGreen");
        message.AddValue(OSCValue.Float(cfg.bgColor.y));
        bundle.AddPacket(message);

        message = new OSCMessage("/1/bgBlue");
        message.AddValue(OSCValue.Float(cfg.bgColor.z));

        message = new OSCMessage("/3/xy1");
        message.AddValue(OSCValue.Float(cfg.ballsData[0].x));
        message.AddValue(OSCValue.Float(cfg.ballsData[0].y));

        message = new OSCMessage("/3/xy2");
        message.AddValue(OSCValue.Float(cfg.ballsData[1].x));
        message.AddValue(OSCValue.Float(cfg.ballsData[1].y));

        bundle.AddPacket(message);

        // Send message
        _oscSender.Send(bundle);
    }
}
