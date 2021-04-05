namespace EllieBot.NervousSystem {

    public class SensorDataPdu<T> {
        public string UniqueId { get; set; }
        public T[] Data { get; set; }
    }
}
