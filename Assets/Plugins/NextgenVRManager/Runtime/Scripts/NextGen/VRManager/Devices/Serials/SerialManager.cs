using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using NextGen.VrManager.Devices;

namespace NextGen.VrManager.Serials
{
    public class SerialManager : MonoBehaviour
    {
        public float updateInterval = 1;

        Coroutine pollSerialsAtInterval;

        public List<Serial> Serials { get { return new List<Serial>(GetComponentsInChildren<Serial>()); } } 

        // Start is called before the first frame update
        void Start()
        {
            PollSerials();
            pollSerialsAtInterval = StartCoroutine(PollSerialsAtInterval());
        }

        void PollSerials()
        {
            List<string> polledSerials = new List<string>(SerialPort.GetPortNames());

            List<Serial> flaggedToRemove = new List<Serial>(Serials);

            foreach (string name in polledSerials)
            {
                if(GetSerialByName(name) == null)
                {
                    Serial serial = new GameObject(name + " (Closed)").AddComponent<Serial>();
                    serial.transform.SetParent(transform);
                    serial.Init(name);
                }
                else
                {
                    flaggedToRemove.Remove(GetSerialByName(name));
                }
            }

            foreach(Serial serial in flaggedToRemove)
            {
                Destroy(serial.gameObject);
            }
        }

        IEnumerator PollSerialsAtInterval()
        {
            var wait = new WaitForSeconds(updateInterval);

            while (true)
            {
                yield return wait;
                PollSerials();
            }
        }

        Serial GetSerialByName(string name)
        {
            foreach(Serial serial in GetComponentsInChildren<Serial>())
            {
                if (serial.name.Contains(name))
                    return serial;
            }

            return null;
        }
    }
}
