# EllieBotBlue
This project was built on a [CamJam EduKit 3](https://thepihut.com/collections/camjam-edukit/products/camjam-edukit-3-robotics) though it should work on most any Raspberry Pi robots.  

This code gives a robotics engineer the ability to communicate with a remote application via an [MQTT](https://mqtt.org/) broker.  

## Introduction
This project allows the control of any Raspberry Pi robot (for example one built from a [CamJam EduKti 3](https://thepihut.com/collections/camjam-edukit/products/camjam-edukit-3-robotics), no affiliation) through an [MQTT](https://mqtt.org/) message broker. It has been written in C# for performance reasons, though from the design architecture, it should be able to be extended by use of any programming language that understands [MQTT](https://mqtt.org/).

This project was created so that I can teach kids how to program.  The idea is to have them use either [MIT App Inventor](https://appinventor.mit.edu/) or [Thunkable](https://thunkable.com/#/) from their tablet to:

- Control all actuators in their robot
- Receive logging information from their robot
- Receive sensor signals from the robot

And although this project originally was conceived for either [MIT App Inventor](https://appinventor.mit.edu/) and [Thunkable](https://thunkable.com/#/), by its architecture, any other programming language with an appropriate [MQTT](https://mqtt.org/) library can be used.

## Setting Up Your Robot

To get this code up and running, you will need:

- **An Assembled Robot**: I used the CamJam EduKit 3, which uses double H-Bridges to control the motors. Any robot should work, as long as it is configured correctly. See Configuration section below.
- **A Raspberry Pi**: I've been using a Raspberry Pi 3B+ with lots of success
- **An [MQTT](https://mqtt.org/) Message Broker**: It can be running either locally in the robot (such as mosquitto) or in a remote server, accessible to all devices. 
- **Dotnet Core 3.1 installed in the Raspberry Pi**: This is needed to both compile and publish the executables.

### Installing [MQTT](https://mqtt.org/)

To install the mosquitto server and client in your Raspberry Pi, just install both the server and client:

```BASH
sudo apt-get install mosquitto mosquitto-clients
```

### Installing DotNet

To install `dotnet` in your Raspberry Pi, follow the instructions found [here](https://docs.microsoft.com/en-us/dotnet/iot/deployment).

### Compiling the Code

To build the code, simply do the following:

```BASH
cd /your/project/directory
dotnet build
dotnet publish -o /your/target/directory
```
For example, assuming that the source code is located in the `~/BlueCamJam` folder, and that you want to place the excutable in a folder named `~/Elliebot` do:

```BASH
cd ~/BlueCamJam
dotnet build
dotnet publish -o ~/EllieBot
```

This will create a folder in your home directory named `EllieBot`. In it, you will find the executable. 

### Configuring Your Robot

To configure your robot, run the `EllieBot` executable for the first time to create a default configuration file.

To do so, simply type:

```BASH
~/EllieBot/EllieBot
```

If this is your first time running EllieBot, then you will see it print a message, create a configuration file, and exit.  You can find the configuration file in `~/EllieBot/elliebot_config.json`.

The `elliebot_config.json` file is used to configure your robot. 


Your configuration file will probably look something like this:

```JSON
{
  "DebuggingLevel": "INFO",
  "MqttConnectionDescription": {
    "Port": 1883,
    "Host": "localhost",
    "TopicForCommands": "com.efhilton.elliebot.topic.commands",
    "TopicForLogging": "com.efhilton.elliebot.topic.logs",
    "TopicForSensorData": "com.efhilton.elliebot.topic.sensors"
  },
  "DriveTrainDescription": {
    "LeftMotorUniqueId": "com.efhilton.elliebot.id.motors.left",
    "RightMotorUniqueId": "com.efhilton.elliebot.id.motors.right"
  },
  "Actuators": {
    "HBridgeMotorDescriptions": [
      {
        "UniqueId": "com.efhilton.elliebot.id.motors.left",
        "ForwardPinNumber": 8,
        "BackwardPinNumber": 7
      },
      {
        "UniqueId": "com.efhilton.elliebot.id.motors.right",
        "ForwardPinNumber": 10,
        "BackwardPinNumber": 9
      }
    ],
    "OutputPinDescriptions": [
      {
        "UniqueId": "com.efhilton.elliebot.id.leds.lightyellow",
        "OutputPinNumber": 18
      }
    ]
  },
  "Sensors": {
    "Hcsr04sDescriptions": [
      {
        "UniqueId": "com.efhilton.elliebot.id.sensors.front.range",
        "TriggerPinNumber": 4,
        "EchoPinNumber": 17
      }
    ],
    "InputPinTriggerDescriptions": [
      {
        "UniqueId": "com.efhilton.elliebot.id.sensors.front.right.collision",
        "InputPinNumber": 27
      },
      {
        "UniqueId": "com.efhilton.elliebot.id.sensors.front.left.collision",
        "InputPinNumber": 22
      }
    ]
  }
}

```
Here, the settings are described in the following sections.

#### General

The following influence general behavior of the robot.

| Setting | Example | Description | 
| --- | ---| --- | 
| `DebuggingLevel` | `INFO` | Sets the debugging level to one of FINEST, FINE, DEBUG, INFO, WARN, ERROR. Messages are reported via the Logging MQTT topic. |

#### MQTT Settings

The following influence general settings for the MQTT connection.

| Setting | Example | Description | 
| --- | ---| --- | 
| `MqttConnectionDescription.Port` | `1883` | The port via which to connect to the [MQTT](https://mqtt.org/) broker | 
| `MqttConnectionDescription.Host` | `localhost` | The server hosting the [MQTT](https://mqtt.org/) broker (it could be local or remote) |
| `MqttConnectionDescription.TopicForCommands` | `com.efhilton.elliebot.topic.commands` | The [MQTT](https://mqtt.org/) topic via which commands will be received. Any device/module can add/listen to commands in this topic. |
| `MqttConnectionDescription.TopicForLogging` | `com.efhilton.elliebot.topic.logs` | The [MQTT](https://mqtt.org/) topic via which commands will be received. Any device and/or module can add/listen to events in this topic. |
| `MqttConnectionDescription.TopicForSensorData` | `com.efhilton.elliebot.topic.sensors` | The [MQTT](https://mqtt.org/) topic via which sensor data will be transmitted. Any device and/or module can add/listen to data in this topic. |

#### Drive Train

The following are used to configure the general drive train characteristics of this robot to provide context needed by the Forward, Backward, CCW, CW, Stop, and Tank commands.  Specifically, a left motor and a right motor must be associated with a unique ID, which shall be defined later in the Actuators section.

| Setting | Example | Description | 
| --- | ---| --- | 
|`DriveTrainDescription.LeftMotorUniqueId` | `com.efhilton.elliebot.id.motors.left` | Declares the left motor id. This id should be defined in the Actuator section. |
|`DriveTrainDescription.RightMotorUniqueId` | `com.efhilton.elliebot.id.motors.right` | Declares the right motor id. This id should be defined in the Actuator section. |

#### Actuators

The following describes actuators configurations. Actuators are any lights, motors, etc. Any number of them can be defined.  Actuators are generally accessible via the Commands MQTT topic.

| Setting | Example | Description | 
| --- | ---| --- | 
| `Actuators.HBridgeMotorDescriptions.UniqueId` | `com.efhilton.elliebot.id.motors.left` | HBridge motor unique identifier for a given motor. This motor is controlled via a simple HBridge, which uses a forward pin, and a backward pin. In this configuration, no Enable pin is used, yet the program will simulate a PWM signal to control the pins as necessary. |
| `Actuators.HBridgeMotorDescriptions.ForwardPinNumber` | 8 | The GPIO pin which will drive this motor forward. |
| `Actuators.HBridgeMotorDescriptions.BackwardPinNumber` |  7| The GPIO pin which will drive the motor Backward. | 
| `OutputPinDescriptions.UniqueId` | `com.efhilton.elliebot.id.leds.lightyellow` | The unique identifier for an output pin |
| `OutputPinDescriptions.OutputPinNumber` | 18 | The GPIO output pin. | 

#### Sensors 

Registered sensors will be monitored and their value reported via the Sensor Data MQTT topic.

| Setting | Example | Description |
| --- | --- | --- |
| `Sensors.Hcsr04sDescriptions.UniqueId` |` com.efhilton.elliebot.id.sensors.front.range` | The unique identifier for a HCSR04s Ultrasonic Range sensor |
| `Sensors.Hcsr04sDescriptions.TriggerPinNumber` |  4 | The GPIO pin which is used to trigger the range finding in the HCSR04s sensor. |
| `Sensors.Hcsr04sDescriptions.EchoPinNumber` | 17 | The GPIO pin that receives the return pulse from the HCSR04s sensor. |
| `Sensors.InputPinTriggerDescriptions.UniqueId` | `com.efhilton.elliebot.id.sensors.front.right.collision` | The Unique Identifier for an input pin trigger. |
| `Sensors.InputPinTriggerDescriptions.InputPinNumber` | 27 | The GPIO pin which receives the change in value (high or low) |


> :warning: **WARNING**: For simplicity, and with the assumption that this code is running inside a controlled network, all security mechanisms have been disabled. You have been warned!

> :warning: **WARNING** Please make sure that the settings match your hardware. Failure to do so could damage your robot!

## Running The Code

Once you've configured your robot as described above, you are ready to run the code.

To do so, simply type:

```BASH
~/EllieBot/EllieBot
```

Your robot is now ready to receive commands.

## Commanding Your Robot

This section describes the syntax needed to command your robot.

### General Command Syntax

Commands are sent as json packets over the [MQTT](https://mqtt.org/) Commands topic. They have the syntax as follows:

```JSON
{
    "Command":"<some command>",
    "Arguments": [
        "<arg1>", ... , "<argN>"
    ]
}
```

For example, to drive your robot like a tank, simply set the forward and ccw effort by using the `com.efhilton.elliebot.cmd.go.tank` command as follows:

```JSON
{
    "Command":"com.efhilton.elliebot.cmd.go.tank",
    "Arguments": [
        "0.1","-0.5"
    ]
}
```
Here, the end effect is that the tank will advance forward at a very slow speed while turning clockwise (roughly speaking, the left motor will operate at 60% duty cycle, going forward, while the right motor operates at 40% duty cycle, going backward).

### Commands

The following commands are recognized by the robot.

| Command | Arguments | Example | Description |
| --- | --- | --- | --- | 
| `com.efhilton.elliebot.cmd.go.tank` | `<forward_effort>,<ccw_effort>`| `{"Command":"com.efhilton.elliebot.cmd.go.tank", "Arguments": ["0.5","-0.5"]}`| Sets the forward speed of the robot as well as the rate of rotation.  For example, setting the `<forward_effort>` to zero, and the `<ccw_effort>` to one will cause the robot to spin counterclockwise about its vertical axis only.  Acceptable values range from `[-1, 1]` for both arguments.|
| `com.efhilton.elliebot.cmd.go.back` | `<abs_dutycycle>`| `{"Command":"com.efhilton.elliebot.cmd.go.back", "Arguments": ["0.5"]}`| Sets the backward speed to the specified absolute duty cycle. The `<abs_dutycycle>` argument will dictate how much effort to use on this command.  Valid arguments are in the range of `[0,1]`.|
| `com.efhilton.elliebot.cmd.go.forward` | `<abs_dutycycle>`| `{"Command":"com.efhilton.elliebot.cmd.go.forward", "Arguments": ["0.5"]}`| Sets the forward speed to the specified absolute duty cycle. The `<abs_dutycycle>` argument will dictate how much effort to use on this command.  Valid arguments are in the range of `[0,1]`.|
| `com.efhilton.elliebot.cmd.go.ccw` | `<abs_dutycycle>`| `{"Command":"com.efhilton.elliebot.cmd.go.ccw", "Arguments": ["0.5"]}`| Sets the counterclockwise speed to the specified absolute duty cycle. The `<abs_dutycycle>` argument will dictate how much effort to use on this command.  Valid arguments are in the range of `[0,1]`.|
| `com.efhilton.elliebot.cmd.go.cw` | `<abs_dutycycle>`| `{"Command":"com.efhilton.elliebot.cmd.go.cw", "Arguments": ["0.5"]}`| Sets the clockwise speed to the specified absolute duty cycle. The `<abs_dutycycle>` argument will dictate how much effort to use on this command.  Valid arguments are in the range of `[0,1]`.|
| `com.efhilton.elliebot.cmd.go.stop` | _none_ | `{"Command":"com.efhilton.elliebot.cmd.go.stop", "Arguments": []}`| Stops all motion of the robot.|
| `com.efhilton.elliebot.cmd.led.on` | `{"Command":"com.efhilton.elliebot.cmd.led.on", "Arguments": ["com.efhilton.elliebot.id.leds.lightyellow" ]}` | Turns on the pin identified by the unique identifier.  In the case of this example, this turns on the yellow light identified by `com.efhilton.elliebot.id.leds.lightyellow`.|
| `com.efhilton.elliebot.cmd.led.off` | `{"Command":"com.efhilton.elliebot.cmd.led.off", "Arguments": ["com.efhilton.elliebot.id.leds.lightyellow" ]}` | Turns off the pin identified by the unique identifier.  In the case of this example, this turns off the yellow light identified by `com.efhilton.elliebot.id.leds.lightyellow`.|
| `com.efhilton.elliebot.cmd.pwm.set` | `{"Command":"com.efhilton.elliebot.cmd.pwm.set", "Arguments": ["com.efhilton.elliebot.id.motors.left", "0.4" ]}` | Sets the PWM duty cycle for the identified HBridge Motor.  In the case of this example, this sets the duty cycle for the left motor identified by `com.efhilton.elliebot.id.motors.left`.|

## Receiving Sensor Data 

Sensor data is transmitted as a JSON packet over the Sensor Data MQTT topic.  The packet looks as follows:

```JSON
{
     "UniqueId":"<unique_id>",
     "Data":["<datum_1>",...,"<datum_N>"]
}
```

Example:

```JSON
{
     "UniqueId":"my.temperature.sensor",
     "Data":["23.0"]
}
```

Here:

- `UniqueId` is a unique identifier for the sensor, in this case `my.temperature.sensor`
- `Data` is an array of data received from that sensor.  For example, a temperature sensor would return one datum, whereas an array of temperature sensors would return an appropriately sized array. In this case, this sensor is reporting a 23 degree celsius teperature.

It is important to note that any device/module can generate and/or consume sensor data.

## Receiving Logging Information

Logging messages are transmitted as JSON packets over the Logs MQTT topic.  The packet looks as follows:

```JSON
{
     "Level":"<log_level_as_int>",
     "LevelStr": "<log_level_as_human_readable_str>",
     "Message": "<log_message>"
}
```
For example:

```JSON
{
     "Level":"2",
     "LevelStr": "DEBUG",
     "Message": "Battery level is now at 50%"
}
```
Here:

- `Level`: A number ranging from 0-5, representing a FINEST, FINE, DEBUG, INFO, WARN, ERROR, respectively.
- `LevelStr`: A human readable level string, for example "FINEST", "FINE", "DEBUG", "INFO", "WARN", and "ERROR".
- `Message`: The event message. For example "Battery level is low".

It is important to note that any device/module can generate and/or consume logging information.

## Client Code, [MIT App Inventor](https://appinventor.mit.edu/)

A quick way to test out your robot is to create a simple application in [MIT App Inventor](https://appinventor.mit.edu/).  

Here, I created a simple joystick application which makes use of the `com.efhilton.elliebot.cmd.go.tank` command. I had a lot of fun driving this robot all around the house, using my phone. I drove it around all forms of obstacles, and timed myself as a personal challenge.

The [MIT App Inventor](https://appinventor.mit.edu/) code can be found [here](./images/EllieBotBlue_Tester.aia).
The Design screen implements a simple joystick control as follows: ![Design Screen](images/screens.png).

I used:
- the built in "Ball" widget, used that as the joystick's head. You drag the ball across the screen, and its relative position from center is converted into control signals for the robot.  
- a text box to enter the IP address of the robot
- a checkbox to connect and disconnect from the robot
- two sliders, which I use to control the horizontal and vertical sensitivity of the joystick
- the `UrsPahoClient`, which is a fantastic little [MQTT](https://mqtt.org/) client plugin that you can download from the [Ullis Roboter Seite](https://ullisroboterseite.de/android-AI2-PahoMQTT-en.html).
- a log window which listens to the logs topic.
- a sensor display widget
- a switch to turn a light on and off.

The blocks for this project are as follows ![Blocks](images/blocks.png).  

## License
This project is governed by the MIT License. All rights reserved.

## Author
The main author for this project is Edgar Hilton, edgar.hilton@gmail.com.

## Questions?
Please don't hesitate to reach out to me, edgar.hilton@gmail.com if you have any questions, comments, or ideas on how to enhance this project.

## Buy Me Coffee
If you find this code useful, and if you either want to feed my growing love of Raspberry Pi hardware, or if you want to buy me a cup of coffee, then consider making a Paypal donation to [my Paypal](http://paypal.com.me/mighty2020).

## Copyright
Copyright 2021 Edgar Hilton

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

In other words, use this software at your own risk. 

