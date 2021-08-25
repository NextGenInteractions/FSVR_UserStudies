using NextGen.VrManager.Devices;

namespace NextGen.VrManager.Serials
{
    public class SerialDevice : Device
    {
        protected Serial serial;

        public void SetData(Serial _serial, string _uid, string _deviceType)
        {
            serial = _serial;

            Uid = _uid;
            Name = _deviceType;

            MapCharacteristics();
            MapFeatures();

            serial.OnLineReceived += OnLineReceived;
        }

        public virtual void MapCharacteristics()
        {
            Characteristics = DeviceCharacteristics.None;
        }

        public virtual void MapFeatures()
        {
        }

        public virtual void OnLineReceived(string line)
        {

        }
    }
}