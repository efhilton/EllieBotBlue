using EllieBot.Ambulator;
using System;

namespace EllieBot.Brain
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IMotorsController Motors;

        public CommandProcessor(IMotorsController motorController)
        {
            this.Motors = motorController;
        }

        public void Initialize()
        {
        }

        public void QueueExecute(RobotCommand cmd)
        {
            switch (cmd.Command.ToUpper())
            {
                case "GO":
                    double left = float.Parse(cmd.Arguments[0]);
                    double right = float.Parse(cmd.Arguments[1]);
                    Motors.SetDutyCycles(left, right);
                    break;

                default:
                    // ignore.
                    break;
            }
        }
    }
}