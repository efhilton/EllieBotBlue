using EllieBot.IO;
using System;

namespace EllieBot.Brain.Commands {

    internal class DriveMotorControl : ICommandExecutor {
        private const string BACKWARD = "go.back";
        private const string FORWARD = "go.forward";
        private const string LEFT = "go.left";
        private const string RIGHT = "go.right";
        private const string STOP = "go.stop";
        private const string TANK = "go.tank";

        private readonly Action<string> Logger;
        private readonly string leftMotorId;
        private readonly string rightMotorId;

        public DriveMotorControl(string leftMotorUniqueId, string rightMotorUniqueId, IPWMController motorsController, Action<string> logger = null) {
            this.Logger = logger;
            this.leftMotorId = leftMotorUniqueId;
            this.rightMotorId = rightMotorUniqueId;
            this.Motors = motorsController;
        }

        public string[] Commands => new string[] { LEFT, RIGHT, FORWARD, BACKWARD, STOP, TANK };
        public IPWMController Motors { get; }

        public void Execute(CommandPacket command) {
            if (command == null
                || string.IsNullOrWhiteSpace(command.Command)
                || command.Arguments == null) {
                return;
            }

            double leftDuty = 0;
            double rightDuty = 0;
            double effort;
            switch (command.Command.Trim().ToLower()) {
                case FORWARD:
                    if (command.Arguments.Length != 1) {
                        throw new ArgumentException("Expected 1 argument");
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = Math.Abs(effort);
                    rightDuty = Math.Abs(effort);
                    break;

                case BACKWARD:
                    if (command.Arguments.Length != 1) {
                        throw new ArgumentException("Expected 1 argument");
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = -Math.Abs(effort);
                    rightDuty = -Math.Abs(effort);
                    break;

                case LEFT:
                    if (command.Arguments.Length != 1) {
                        throw new ArgumentException("Expected 1 argument");
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = -Math.Abs(effort);
                    rightDuty = Math.Abs(effort);
                    break;

                case RIGHT:
                    if (command.Arguments.Length != 1) {
                        throw new ArgumentException("Expected 1 argument");
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = Math.Abs(effort);
                    rightDuty = -Math.Abs(effort);
                    break;

                case TANK:
                    if (command.Arguments.Length != 2) {
                        throw new ArgumentException("Expected 2 arguments");
                    }
                    double forwardSpeed = double.Parse(command.Arguments[0] ?? "0");
                    double antiClockwiseSpin = double.Parse(command.Arguments[1] ?? "0");
                    leftDuty = forwardSpeed - antiClockwiseSpin;
                    rightDuty = forwardSpeed + antiClockwiseSpin;
                    break;

                case STOP:
                default:
                    leftDuty = 0;
                    rightDuty = 0;
                    break;
            }

            this.Motors.SetDutyCycle(this.leftMotorId, leftDuty);
            this.Motors.SetDutyCycle(this.rightMotorId, rightDuty);
        }
    }
}
