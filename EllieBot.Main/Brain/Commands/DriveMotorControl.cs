using EllieBot.IO;
using System;

namespace EllieBot.Brain.Commands {

    internal class DriveMotorControl : ICommandExecutor {
        private const string LEFT = "go.left";
        private const string RIGHT = "go.right";
        private const string FORWARD = "go.forward";
        private const string BACKWARD = "go.back";
        private const string STOP = "go.stop";
        private const string TANK = "go.tank";

        public string[] Commands => new string[] { LEFT, RIGHT, FORWARD, BACKWARD, STOP };

        public IPWMController Motors { get; }

        public DriveMotorControl(IPWMController motorsController) => this.Motors = motorsController;

        public void Execute(RobotCommand command) {
            if (string.IsNullOrWhiteSpace(command.Command)) {
                return;
            }
            if (command.Arguments.Length != 1) {
                return;
            }

            double leftDuty = 0;
            double rightDuty = 0;
            double effort;
            switch (command.Command.Trim().ToLower()) {
                case FORWARD:
                    if (command.Arguments.Length != 1) {
                        return;
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = Math.Abs(effort);
                    rightDuty = Math.Abs(effort);
                    break;

                case BACKWARD:
                    if (command.Arguments.Length != 1) {
                        return;
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = -Math.Abs(effort);
                    rightDuty = -Math.Abs(effort);
                    break;

                case LEFT:
                    if (command.Arguments.Length != 1) {
                        return;
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = -Math.Abs(effort);
                    rightDuty = Math.Abs(effort);
                    break;

                case RIGHT:
                    if (command.Arguments.Length != 1) {
                        return;
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = Math.Abs(effort);
                    rightDuty = -Math.Abs(effort);
                    break;

                case TANK:
                    if (command.Arguments.Length != 2) {
                        return;
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

            this.Motors.SetDutyCycle(Constants.LEFT_MOTOR_ID, leftDuty);
            this.Motors.SetDutyCycle(Constants.RIGHT_MOTOR_ID, rightDuty);
        }
    }
}
