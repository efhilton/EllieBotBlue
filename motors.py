#!/usr/bin/env python
import RPi.GPIO as GPIO

class Motors:
    __doc__ = """
        A simple motor controller which allows the motors
        to be driven forward, backward, turn left, turn right, stop.

        No extra intelligence is supplied here.
        """
    
    all_ports = [7,8,9,10]
        
    def set_bit_to_all(self,value):
        for p in self.all_ports:
            GPIO.output(p,value)

    def stop(self):
        self.set_bit_to_all(0)

    def right_reverse(self):
        GPIO.output(9,1)
        GPIO.output(10,0)

    def right_forward(self):
        GPIO.output(9,0)
        GPIO.output(10,1)

    def left_forward(self):
        GPIO.output(7,0)
        GPIO.output(8,1)
        
    def left_reverse(self):
        GPIO.output(7,1)
        GPIO.output(8,0)

    def forward(self):
        self.left_forward()
        self.right_forward()

    def reverse(self):
        self.left_reverse()
        self.right_reverse()

    def turn_left(self):
        self.left_reverse()
        self.right_forward()

    def turn_right(self):
        self.left_forward()
        self.right_reverse()

    def initialize(self):
        GPIO.setmode(GPIO.BCM)
        GPIO.setwarnings(False)

        for p in self.all_ports:
            GPIO.setup(p, GPIO.OUT)
            
    
    def __init__(self):
        self.initialize()
        self.stop()
