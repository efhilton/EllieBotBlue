class Motors:
    __doc__ = """
        A simple motor mock used for testing
        """

    def __init__(self):
        self.stop()

    def stop(self):
        print("stopping")

    def right_reverse(self):
        pass

    def right_forward(self):
        pass

    def left_forward(self):
        pass

    def left_reverse(self):
        pass

    def forward(self):
        print("Going forward")

    def reverse(self):
        print("Going backward")

    def turn_left(self):
        print("Turning Left")

    def turn_right(self):
        print("Turning Right")
