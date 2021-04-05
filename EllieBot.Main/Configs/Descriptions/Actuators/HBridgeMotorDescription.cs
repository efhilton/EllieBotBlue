using EllieBot.IO.Devices;

namespace EllieBot.Configs.Descriptions {

    public class HBridgeMotorDescription : IIdentifiable {
        public string UniqueId { get; set; }
        public int ForwardPinNumber { get; set; }
        public int BackwardPinNumber { get; set; }
    }
}
