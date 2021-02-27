using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO.Pipes;
using System.Text;
using System.Security.Principal;
using System.IO.MemoryMappedFiles;

[RequireComponent(typeof(SliMeshMaker))]

public class NamedPipes : MonoBehaviour
{
    GameObject tbx;

    Text debug_text;
    string debug_string = "hi";

    SliMeshMaker smm;

#if UNITY_EDITOR
    private string debug_sli = @"C:\Users\chris\source\repos\whistlehead Roadways V2 GUI Unity\whistlehead Roadways V2 GUI\bin\Debug\dat\debug.sli";
    private string debug_dat = @"C:\Users\chris\source\repos\whistlehead Roadways V2 GUI Unity\whistlehead Roadways V2 GUI\bin\Debug\dat";
#endif

#if UNITY_STANDALONE && !UNITY_EDITOR
    private volatile bool connected = false;
#endif

    private volatile bool regenerate = false;
    private volatile bool exit = false;
    private volatile string sli;
    private volatile string dat;

    //private int timeSinceAlive = 0;

    // Start is called before the first frame update
    void Start()
    {
        tbx = GameObject.Find("debugText");
        debug_text = tbx.GetComponent<Text>();
        debug_text.text = "Debug textbox test";
        smm = GetComponent<SliMeshMaker>();

#if UNITY_EDITOR
        sli = debug_sli;
        dat = debug_dat;
        smm.Regenerate(File.ReadAllText(sli), dat);
#endif

#if UNITY_STANDALONE && !UNITY_EDITOR
        StreamString clientDataStream;
        string server_msg = "lol RIP";
        Thread _DataThread = new Thread(() =>
        {
            NamedPipeClientStream client = new NamedPipeClientStream(".", "whistleheadRoadwaysDataPipe", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None);
            Debug.Log("Client started, connecting");
            client.Connect();
            connected = true;
            Debug.Log("Connected");
            clientDataStream = new StreamString(client);
            while (true)
            {
                try
                {
                    server_msg = clientDataStream.ReadString();
                    //Debug.Log(server_msg);
                    switch (server_msg)
                    {
                        case "Regenerate":
                            clientDataStream.WriteString("Ready to regenerate");
                            sli = clientDataStream.ReadString();
                            //Debug.Log(sli);
                            regenerate = true;
                            //debug_string = "Got regeneration message";
                            clientDataStream.WriteString("Regenerating next frame");
                            break;
                        case "Update Dat Path":
                            clientDataStream.WriteString("Ready to update");
                            dat = clientDataStream.ReadString();
                            //Debug.Log(dat);
                            debug_string = "Got update dat path message";
                            clientDataStream.WriteString("Dat path updated");
                            break;
                        default:
                            clientDataStream.WriteString("Did not understand message");
                            Debug.Log("Did not understand message");
                            break;
                    }
                } catch {
                    exit = true;
                }
            }
        });
        _DataThread.Start();

        StreamString clientAliveStream;
        Thread _AliveThread = new Thread(() =>
        {
            NamedPipeClientStream client = new NamedPipeClientStream(".", "whistleheadRoadwaysAlivePipe", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None);
            //Debug.Log("Client started, connecting");
            client.Connect();
            //Debug.Log("Connected");
            clientAliveStream = new StreamString(client);
            while (true)
            {
            try {
                    clientAliveStream.WriteString("Client alive and well and sending you a very long message, some might say excessively long, in order to test whether this causes more memory to be used by the program");
                    //timeSinceAlive = 0;
                    // don't need to bother every loop iteration
                    Thread.Sleep(500);
                } catch (Exception e) {
                    Debug.Log(e);
                    exit = true;
                }
            }
        });
        _AliveThread.Start();

        //int delay = 500;
        //int limit = 2000;
        //Thread _KillThread = new Thread(() =>
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(delay);
        //        timeSinceAlive += delay;
        //        if (timeSinceAlive > limit) exit = true;
        //    }
        //});
        //_KillThread.Start();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (exit) Application.Quit();
        debug_text.text = debug_string;
        if (regenerate)
        {
            smm.Regenerate(sli, dat);
            regenerate = false;
        }
    }

    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            // max length to read is 2^16 - 1 = 65535 (0xFFFF) characters
            int len;
            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            var inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            // max length to write is 2^16 - 1 = 65535 (0xFFFF) characters
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}
