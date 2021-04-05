using EllieBot.IO.Devices;

namespace EllieBot.Configs.Descriptions {

    public class InputPinTriggerDescription : IIdentifiable {
        public string UniqueId { get; set; }
        public int InputPinNumber { get; set; }
    }
}
