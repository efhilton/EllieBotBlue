import time

MAX_DURATION = 10.0


class Brain:
    __doc__ = """ A simple brain to help manage the motors.
        The centerpiece for this class is the 'parse_string()'
        which takes a raw string of the format:

        [command]:[seconds], [command]:[seconds]

        and executes each in turn.

        Here, [seconds] is any positive float, denoting the
        amount of seconds to hold the command.

        Commands are one of:

        F - Go Forward
        B - Go Backward
        R - Turn Right
        L - Turn Left
        S - Stop

        """

    def __init__(self, motors):
        self.motors = motors
        self.initialize()

    def initialize(self):
        self.motors.stop()

    def wait(self, seconds: float = None):
        if seconds and seconds > 0.0:
            if seconds > MAX_DURATION:
                time.sleep(MAX_DURATION)
            else:
                time.sleep(seconds)

    def parse_string(self, raw_string: str):
        # commands are of syntax: "[cmd1]:[seconds1],[cmd2]:[seconds2]"
        if not raw_string:
            return

        for command in raw_string.split(","):
            cmd_arg = command.split(":")
            self.execute_command(cmd_arg[0], float(cmd_arg[1]))

        # Just in case somebody forgets to stop.
        self.motors.stop()

    def turn_right(self):
        self.motors.turn_right()

    def turn_left(self):
        self.motors.turn_left()

    def stop(self):
        self.motors.stop()

    def forward(self):
        self.motors.forward()

    def reverse(self):
        self.motors.reverse()

    def execute_command(self, command: str, seconds: float = None):
        cmd = command.upper()
        if cmd == 'RIGHT' or cmd == 'R':
            self.turn_right()
            self.wait(seconds)
        elif cmd == 'LEFT' or cmd == 'L':
            self.turn_left()
            self.wait(seconds)
        elif cmd == 'FORWARD' or cmd == 'F':
            self.forward()
            self.wait(seconds)
        elif cmd == 'BACK' or cmd == 'B':
            self.reverse()
            self.wait(seconds)
        elif cmd == 'STOP' or cmd == 'S':
            self.stop()
        elif cmd == 'WAIT' or cmd == 'W':
            self.wait(seconds)
        else:
            print("Ignoring " + str(cmd) + " with seconds " + str(seconds))

    parse_string.__doc__ = """Parses a comma separated raw string of commands.
                                Commands are of syntax 'cmd:seconds' """
