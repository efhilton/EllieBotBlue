namespace EllieBot {

    internal class Defaults {

        public static class PinNums {
            internal const int MOTOR_FORWARD_LEFT = 8;
            internal const int MOTOR_BACKWARD_LEFT = 7;
            internal const int MOTOR_FORWARD_RIGHT = 10;
            internal const int MOTOR_BACKWARD_RIGHT = 9;
            internal const int HEAD_LIGHTS = 11;
            internal const int BRAKE_LIGHTS = 12;
            internal const int OTHER_LIGHTS = 13;
        }

        public static class Mqtt {
            internal const string TOPIC_FOR_COMMANDS = "com.efhilton.elliebot.commands";
            internal const string TOPIC_FOR_LOGGING = "com.efhilton.elliebot.logs";
            internal const string HOST = "localhost";
            internal const int PORT = 1883;
        }

        public static class Internal {
            internal const string CONFIG_FILE_NAME = "elliebot_config.json";
        }

        public static class ComponentIds {
            internal const string LEFT_MOTOR = "elliebot.motors.left";
            internal const string RIGHT_MOTOR = "elliebot.motors.right";
            internal const string HEAD_LIGHTS = "elliebot.leds.headlight";
            internal const string BRAKE_LIGHTS = "elliebot.leds.brakelight";
            internal const string OTHER_LIGHTS = "elliebot.leds.otherlight";
        }

        public static class Commands {

            public static class Go {
                internal const string BACKWARD = "go.back";
                internal const string FORWARD = "go.forward";
                internal const string LEFT = "go.left";
                internal const string RIGHT = "go.right";
                internal const string STOP = "go.stop";
                internal const string TANK = "go.tank";
            }

            public static class Led {
                internal const string ON = "led.on";
                internal const string OFF = "led.off";
            }

            public static class Pwm {
                internal const string SET_PWM = "set.pwm";
            }
        }
    }
}
