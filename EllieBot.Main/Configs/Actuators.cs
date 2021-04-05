using EllieBot.Configs.Descriptions;
using static EllieBot.Constants;

namespace EllieBot.Configs {
    public class Actuators {

        public HBridgeMotorDescription[] HBridgeMotorDescriptions { get; set; } = new HBridgeMotorDescription[] {
            new HBridgeMotorDescription {
                UniqueId = ComponentIds.LEFT_MOTOR,
                ForwardPinNumber = PinNums.MOTOR_FORWARD_LEFT,
                BackwardPinNumber = PinNums.MOTOR_BACKWARD_LEFT
            },
            new HBridgeMotorDescription {
                UniqueId = ComponentIds.RIGHT_MOTOR,
                ForwardPinNumber = PinNums.MOTOR_FORWARD_RIGHT,
                BackwardPinNumber = PinNums.MOTOR_BACKWARD_LEFT
            }
        };

        public OutputPinDescription[] OutputPinDescriptions { get; set; } = new OutputPinDescription[] {
            new OutputPinDescription {
                UniqueId = ComponentIds.YELLOW_LIGHT,
                OutputPinNumber = PinNums.YELLOW_LIGHT
            },
        };
    }
}
