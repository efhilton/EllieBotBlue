namespace EllieBot {

    internal class Defaults {

        public static class PinNums {
            internal static readonly int MOTOR_FORWARD_LEFT = 8;
            internal static readonly int MOTOR_BACKWARD_LEFT = 7;
            internal static readonly int MOTOR_FORWARD_RIGHT = 10;
            internal static readonly int MOTOR_BACKWARD_RIGHT = 9;
            internal static readonly int HEAD_LIGHTS = 11;
            internal static readonly int BRAKE_LIGHTS = 12;
            internal static readonly int OTHER_LIGHTS = 13;
        }

        public static class Mqtt {
            internal static readonly string TOPIC_FOR_COMMANDS = "com.efhilton.elliebot.commands";
            internal static readonly string TOPIC_FOR_LOGS = "com.efhilton.elliebot.logs";
            internal static readonly string HOST = "localhost";
            internal static readonly int PORT = 1883;
        }

        public static class Internal {
            internal static readonly string CONFIG_FILE_NAME = "robot_config.json";
        }

        public static class ComponentIds {
            internal static readonly string LEFT_MOTOR = "elliebot.motors.left";
            internal static readonly string RIGHT_MOTOR = "elliebot.motors.right";
            internal static readonly string HEAD_LIGHTS = "elliebot.leds.headlight";
            internal static readonly string BRAKE_LIGHTS = "elliebot.leds.brakelight";
            internal static readonly string OTHER_LIGHTS = "elliebot.leds.otherlight";
        }
    }
}
