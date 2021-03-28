namespace EllieBot {

    internal class Constants {
        internal static readonly int DEFAULT_FORWARD_PIN_LEFT = 8;
        internal static readonly int DEFAULT_BACKWARD_PIN_LEFT = 7;
        internal static readonly int DEFAULT_FORWARD_PIN_RIGHT = 10;
        internal static readonly int DEFAULT_BACKWARD_PIN_RIGHT = 9;
        internal static readonly int DEFAULT_HEADLIGHTS_PIN = 11;

        internal static readonly string DEFAULT_CONFIG_FILE_NAME = "robot_config.json";
        internal static readonly string LEFT_MOTOR_ID = "LEFT";
        internal static readonly string RIGHT_MOTOR_ID = "RIGHT";

        internal static readonly string DEFAULT_TOPIC_FOR_COMMANDS = "com.efhilton.elliebot.commands";
        internal static readonly string DEFAULT_TOPIC_FOR_LOGS = "com.efhilton.elliebot.logs";
        internal static readonly string DEFAULT_COMMUNICATIONS_ADDRESS = "localhost";
        internal static readonly int DEFAULT_COMMUNICATIONS_PORT = 1883;
    }
}
