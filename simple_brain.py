import time

class SimpleBrain:
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
        print("Initializing")
        self.motors.stop()

    def pause(self, seconds:float):
        time.sleep(seconds)


    def parse_string(self, raw_string:str):
        # commands are of syntax: "[cmd1]:[seconds1],[cmd2]:[seconds2]"
        for command in raw_string.split(","):
            cmd_arg = command.split(":")
            self.execute_command(cmd_arg[0],cmd_arg[1])

        # Just in case somebody forgets to stop.
        self.motors.stop()

    parse_string.__doc__ = """Parses a comma separated raw string of commands.
                                Commands are of syntax 'cmd:seconds' """
        
    def turn_right(self,seconds:float):
        print("turning right for " + seconds + " s")
        self.motors.turn_right()
        time.sleep(float(seconds))

    def turn_left(self,seconds:float):
        print("turning left for "+ seconds + " s")
        self.motors.turn_left()
        time.sleep(float(seconds))

    def stop(self,seconds:float):
        print("stopping for "+ seconds + " s")
        self.motors.stop()
        time.sleep(float(seconds))

    def forward(self,seconds:float):
        print("forward for "+ seconds + " s")
        self.motors.forward()
        time.sleep(float(seconds))

    def reverse(self,seconds:float):
        print("reversing for "+ seconds + " s")
        self.motors.reverse()
        time.sleep(float(seconds))

    def execute_command(self, cmd:str, seconds:float):   
        if (cmd == 'R' or cmd == 'r'):
            self.turn_right(seconds)
        elif (cmd == 'L' or cmd == 'l'):
            self.turn_left(seconds)
        elif (cmd == 'F' or cmd == 'f'):
            self.forward(seconds)
        elif (cmd == 'B' or cmd == 'b'):
            self.reverse(seconds)
        elif (cmd == 'S' or cmd == 's'):
            self.stop(seconds)
        else:
            print("Ignoring " + str(cmd) + " with seconds " + str(seconds))

