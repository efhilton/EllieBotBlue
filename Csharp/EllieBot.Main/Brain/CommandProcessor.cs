using EllieBot.Ambulator;
using System;
using System.Threading.Tasks;

namespace EllieBot.Brain
{
    public class CommandProcessor: ICommandProcessor
    {
        private readonly IMotors Motors;

        public CommandProcessor(IMotors motors)
        {
            this.Motors = motors;
        }

        public Task Execute(RobotCommand cmd)
        {
            switch (cmd.Command.ToUpper())
            {
                case "GO":
                    double left = Math.Clamp(float.Parse(cmd.Arguments[0]),-1.0,1.0);
                    double right = Math.Clamp(float.Parse(cmd.Arguments[1]), -1.0, 1.0);
                    return Motors.Go(left,right);
                case "PAUSE":
                    int durationMs = int.Parse(cmd.Arguments[0]);
                    return Task.Delay(durationMs);
                default:
                    return Task.FromResult(0);
            }
        }
    }
}
