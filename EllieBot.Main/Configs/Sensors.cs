using EllieBot.Configs.Descriptions;
using static EllieBot.Constants;

namespace EllieBot.Configs {

    public class Sensors {

        public Hcsr04sDescription[] Hcsr04sDescriptions { get; set; } = new Hcsr04sDescription[] {
            new Hcsr04sDescription {
                UniqueId = ComponentIds.ULTRASONIC_HCSR04,
                TriggerPinNumber = PinNums.ULTRASONIC_HCSR04_TRIGGER,
                EchoPinNumber = PinNums.ULTRASONIC_HCSR04_ECHO
            }
        };

        public InputPinTriggerDescription[] InputPinTriggerDescriptions { get; set; } = new InputPinTriggerDescription[] {
            new InputPinTriggerDescription {
                UniqueId = ComponentIds.BUTTON_RIGHT_COLLISION,
                InputPinNumber = PinNums.BUTTON_RIGHT_COLLISION
            },
             new InputPinTriggerDescription {
                UniqueId = ComponentIds.BUTTON_LEFT_COLLISION,
                InputPinNumber = PinNums.BUTTON_LEFT_COLLISION
            }
        };
    }
}
