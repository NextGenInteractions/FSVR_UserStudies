using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Text;
using NextGen.VrManager.Serials.Devices;
using NextGen.VrManager.Devices;

namespace NextGen.VrManager.Serials
{
    public class Serial : MonoBehaviour
    {
        public string com;

        private SerialPort port;
        public bool IsOpen;
        public int baudrate = 9600;

        public bool handshake = false;
        public string deviceUid = "";
        public string deviceType = "";

        public bool ToggleToOpen;
        public bool ToggleToClose;

        public Action<string> OnLineReceived;

        public int BytesToRead;
        public List<string> LineLog = new List<string>();
        public List<int> ByteQueue = new List<int>();
        public string TextQueue = "";

        private SerialDevice serialDevice;

        public void Init(string _com)
        {
            com = _com;
            Open();
        }

        public void Open()
        {
            try
            {
                SerialPort sp = new SerialPort(com, baudrate, Parity.None, 8, StopBits.One);
                sp.DtrEnable = true;
                sp.RtsEnable = true;
                sp.ReadTimeout = 500;
                sp.WriteTimeout = 500;
                sp.Open();

                port = sp;

                Debug.Log(string.Format("Serial port opened on {0} with baudrate {1}.", com, baudrate));

                name = com + " (Open)";
            }

            catch
            {
                Debug.Log(string.Format("Failed to open serial port on {0} with baudrate {1}.", com, baudrate));
            }
        }

        public void Close()
        {
            if (port != null)
            {
                if (port.IsOpen)
                {
                    port.Close();
                    Debug.Log(string.Format("Closing serial port on {0}.", com));
                    name = com + " (Closed)";
                }
            }
        }

        private void OnDisable()
        {
            Close();
        }

        // Update is called once per frame
        void Update()
        {
            if(ToggleToOpen)
            {
                ToggleToOpen = false;
                Open();
            }
            if(ToggleToClose)
            {
                ToggleToClose = false;
                Close();
            }

            //-----

            if(port != null)
            {
                IsOpen = port.IsOpen;
                if (IsOpen)
                {
                    BytesToRead = port.BytesToRead;

                    if (port.BytesToRead > 0)
                    {
                        int[] intArray = new int[port.BytesToRead];
                        for (int i = 0; i < intArray.Length; i++)
                        {
                            intArray[i] = port.ReadByte();
                        }

                        byte[] byteArray = new byte[intArray.Length];
                        for (int i = 0; i < byteArray.Length; i++)
                        {
                            byteArray[i] = (byte)intArray[i];
                        }

                        string parsedText = Encoding.ASCII.GetString(byteArray);
                        TextQueue += parsedText;
                    }
                }
                else
                {

                }
            }
            else
            {
                IsOpen = false;
            }

            if(TextQueue != "")
            {
                while (TextQueue.Contains("\n"))
                {
                    char[] separator = new char[] { '\n' };
                    string[] split = TextQueue.Split(separator, 2);
                    Line(split[0]);
                    TextQueue = split[1];
                }
            }
        }

        void Line(string line)
        {
            //Debug.Log(line);

            LineLog.Add(line);

            if(line[0] == '#')
                CheckLine(line);

            if (LineLog.Count > 10)
                LineLog.RemoveAt(0);

            OnLineReceived?.Invoke(line);
        }

        void CheckLine(string line)
        {
            if (line.Contains("HandshakeEnd"))
                HandshakeEnd();
            if (line.Contains("Baudrate"))
                ReceiveBaudrate(line);
            if (line.Contains("UID"))
                ReceiveUID(line);
            if (line.Contains("Device Type"))
                ReceiveDeviceType(line);
        }

        void HandshakeEnd()
        {
            handshake = true;
            port.Write("!");
            BecomeDevice();
        }

        void ReceiveBaudrate(string line)
        {
            baudrate = int.Parse(line.Split(':')[1]);
        }

        void ReceiveUID(string line)
        {
            deviceUid = line.Split(':')[1];
        }

        void ReceiveDeviceType(string line)
        {
            deviceType = line.Split(':')[1];
        }

        void BecomeDevice()
        {
            if (deviceType.Contains("Touchpad"))
            {
                Touchpad touchpad = new Touchpad();
                touchpad.SetData(this, deviceUid, deviceType);
                DeviceManager.AddDevice(touchpad);
                serialDevice = touchpad;
            }
        }
    }
}
