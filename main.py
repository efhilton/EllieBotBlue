from brain import Brain
from bluetooth_slave import BluetoothSlave
import bluetooth
import sys

IS_DEBUGGING = False

if __name__ == '__main__':
    for p in sys.argv:
        if p == '--DEBUG':
            IS_DEBUGGING = True
            break

    if IS_DEBUGGING:
        print("Running in Debug Mode")
        print("Motors Disabled")
        from motors_mock import Motors
    else:
        print("Motors Enabled")
        from motors import Motors

    motors = Motors()
    brain = Brain(motors)
    slave = BluetoothSlave()

    while True:
        try:
            slave.run_service(brain.parse_string)
        except bluetooth.BluetoothError as e:
            # ignore
            print("Disconnected")
