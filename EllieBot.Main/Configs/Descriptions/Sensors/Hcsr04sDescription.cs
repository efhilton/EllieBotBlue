using EllieBot.IO.Devices;

namespace EllieBot.Configs.Descriptions {

    public class Hcsr04sDescription : IIdentifiable {
        public string UniqueId { get; set; }
        public int TriggerPinNumber { get; set; }
        public int EchoPinNumber { get; set; }
    }
}
