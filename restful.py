from flask_classful import FlaskView,route

MAX_DURATION = 10.0

class RobotView(FlaskView):
    brain = None

    def __init__(self, brain):
        self.counter=0
        self.brain = brain

    def index(self):
        return self.ping()

    @route("/go/<string:cmd>/<float:duration>", methods=["GET"])
    def command_float(self, cmd: str, duration: float):
        self.counter = self.counter+1
        if duration > MAX_DURATION:
            return "That's Too long Boss!!! I only know how to count to " + str(MAX_DURATION) + " seconds!"
        else: self.brain.execute_command(cmd.upper(),duration)
        return "Done, Boss!"

    @route("/go/<string:cmd>/<int:duration>", methods=["GET"])
    def command_int(self, cmd: str, duration: int):
        return self.command_float(cmd,float(duration))

    @route("/do/<string:sequence>")
    def sequence(self, sequence: str):
        self.counter = self.counter+1
        self.brain.parse_string(sequence)
        return "I Executed " + sequence.upper()

    def ping(self):
        self.counter = self.counter+1
        return "I'm Awake! I've handled " + str(self.counter) + " requests!"

    # def run_service(self, callback=None):
    #     self.app.run()