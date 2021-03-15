from brain import Brain
import sys

USE_MOTORS = True
REMOTE_MODE = "Rest"

if __name__ == '__main__':
    for p in sys.argv:
        if p == '--MOTORS':
            USE_MOTORS = True
        if p == '--NO_MOTORS':
            USE_MOTORS = False
        if p == '--SERIAL':
            REMOTE_MODE = 'Serial'
        if p == '--REST':
            REMOTE_MODE = 'Rest'

    if not USE_MOTORS:
        print("Motors Disabled")
        from motors_mock import Motors
    else:
        print("Motors Enabled")
        from motors import Motors

    motors = Motors()
    brain = Brain(motors)

    if REMOTE_MODE == 'Serial':
        print("Using BLUETOOTH")
        from bluetooth_slave import BluetoothSlave
        slave = BluetoothSlave()
        slave.run_service(brain.parse_string)
    else:
        print("Using REST")
        from restful import RobotView
        from flask import Flask
        app = Flask(__name__)
        RobotView.register(app,init_argument=brain)
        app.run()