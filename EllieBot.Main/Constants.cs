namespace EllieBot {

    public class Constants {

        public static class PinNums {
            internal const int MOTOR_FORWARD_LEFT = 8;
            internal const int MOTOR_BACKWARD_LEFT = 7;
            internal const int MOTOR_FORWARD_RIGHT = 10;
            internal const int MOTOR_BACKWARD_RIGHT = 9;
            internal const int YELLOW_LIGHT = 18;
            internal const int ULTRASONIC_HCSR04_TRIGGER = 4;
            internal const int ULTRASONIC_HCSR04_ECHO = 17;
        }

        public static class Mqtt {
            internal const string TOPIC_FOR_COMMANDS = "com.efhilton.elliebot.topic.commands";
            internal const string TOPIC_FOR_LOGGING = "com.efhilton.elliebot.topic.logs";
            internal const string TOPIC_FOR_SENSORS = "com.efhilton.elliebot.topic.sensors";
            internal const string HOST = "localhost";
            internal const int PORT = 1883;
        }

        public static class Internal {
            internal const string CONFIG_FILE_NAME = "elliebot_config.json";
        }

        public static class ComponentIds {
            internal const string LEFT_MOTOR = "com.efhilton.elliebot.id.motors.left";
            internal const string RIGHT_MOTOR = "com.efhilton.elliebot.id.motors.right";
            internal const string YELLOW_LIGHT = "com.efhilton.elliebot.id.leds.lightyellow";
            internal const string ULTRASONIC_HCSR04 = "com.efhilton.elliebot.id.sensors.front.distance";
        }

        public static class Commands {

            public static class Go {
                internal const string BACKWARD = "com.efhilton.elliebot.cmd.go.back";
                internal const string FORWARD = "com.efhilton.elliebot.cmd.go.forward";
                internal const string LEFT = "com.efhilton.elliebot.cmd.go.left";
                internal const string RIGHT = "com.efhilton.elliebot.cmd.go.right";
                internal const string STOP = "com.efhilton.elliebot.cmd.go.stop";
                internal const string TANK = "com.efhilton.elliebot.cmd.go.tank";
            }

            public static class Led {
                internal const string ON = "com.efhilton.elliebot.cmd.led.on";
                internal const string OFF = "com.efhilton.elliebot.cmd.led.off";
            }

            public static class Pwm {
                internal const string SET_PWM = "com.efhilton.elliebot.cmd.pwm.set";
            }
        }

        public enum LoggingLevel {
            FINEST = 0,
            FINE = 1,
            DEBUG = 2,
            INFO = 3,
            WARN = 4,
            ERROR = 5
        }
    }
}
