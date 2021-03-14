import motors as m
import brain as b
import bluetooth_slave as z

motors = m.Motors()
brain = b.Brain(motors)
bluez = z.BluetoothSlave(brain)

