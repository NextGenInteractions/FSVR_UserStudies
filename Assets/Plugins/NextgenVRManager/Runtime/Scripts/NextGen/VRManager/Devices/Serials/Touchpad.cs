using NextGen.VrManager.Devices;
using NextGen.VrManager.Devices.Serials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NextGen.VrManager.Serials.Devices
{
    public class Touchpad : SerialDevice
    {
        public int x;
        public int y;

        public bool touching;

        public override void MapFeatures()
        {
            featureValues[(DeviceFeatureUsage)SerialDeviceFeatures.touchpad] = (out object obj) => { obj = new Vector2(x, y); return true; };
            featureValues[(DeviceFeatureUsage)SerialDeviceFeatures.touchpadTouch] = (out object obj) => { obj = touching; return true; };
            
            featureValues[(DeviceFeatureUsage)CommonDeviceFeatures.primary2DAxis] = (out object obj) => { obj = Vector2.zero; return true; };
            featureValues[(DeviceFeatureUsage)CommonDeviceFeatures.primary2DAxisTouch] = (out object obj) => {obj = touching; return true; };
        }

        public override void OnLineReceived(string line)
        {
            if(!line.Contains("#"))
            {
                string[] split = line.Split(',');

                int.TryParse(split[0], out x);
                int.TryParse(split[1], out y);

                touching = x != -1 && y != -1;
            }
        }
    }
}
