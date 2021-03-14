import bluetooth
import motors as m
import brain as b


class BluetoothSlave:
    BLUETOOTH_SLAVE_NAME = "BlueCamJam"
    UUID = "4760dd17-365a-4f28-9035-d0029e6f46a8"

    def __init__(self, service_name=BLUETOOTH_SLAVE_NAME, uuid=UUID):
        self.service_name = service_name
        self.uuid = uuid

    def initialize(self):
        print("Initializing...")
        self.server_socket = bluetooth.BluetoothSocket(bluetooth.RFCOMM)
        self.server_socket.bind(("", bluetooth.PORT_ANY))
        self.server_socket.listen(1)

        bluetooth.advertise_service(
            self.server_socket,
            self.service_name,
            self.uuid
        )

    def run_service(self, callback=None):
        self.initialize()
        print("Awaiting connections")

        client_sock, client_info = self.server_socket.accept()
        print("Accepted connection from ", client_info)

        while True:
            data = client_sock.recv(1024).decode()
            if not data:
                # if data is not received break
                break
            print("received [%s]" % data)
            if callback != None:
                callback(data)

            # As of now there's no need to send any data back.
            # client_sock.send("ACK".encode(encoding="utf-8"))

        client_sock.close()
        self.server_socket.close()


if __name__ == '__main__':

    motors = m.Motors()
    brain = b.Brain(motors)
    slave = BluetoothSlave()

    while True:
        try:
            slave.run_service(brain.parse_string)
        except bluetooth.BluetoothError as e:
            # ignore
            print("Disconnected")
