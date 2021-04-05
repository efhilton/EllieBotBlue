namespace EllieBot.IO.Devices {

    public interface ISensor<T> : IDevice {

        delegate void OnDataReceived(string id, T data);

        public event OnDataReceived OnData;
    }
}