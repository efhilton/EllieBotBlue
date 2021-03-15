# BlueCamJam
This project gives the CamJam EduKit 3 the ability to communicate with a remote application via either:

- Bluetooth
- RESTful services
- WebSockets (default)

The idea is to provide a mechanism via which a remote application (for example, an application running on an Android/IPhone/PC) can use to communicate and control the robot.

## Introduction
This project adds motor control via:

- Bluetooth (as a bluetooth slave using Bluez)
- RESTful
- and Websockets (default)

To a Cam Jam EduKit 3 Raspberry Pi robot kit.  

- Turn Right
- Turn Left
- Go Forward
- Go Backward
- Stop
- Execute a sequence of pre-programmed commands: In this case a long sequence of commands is sent to the robot, and the robot will interpret them one after the next. 

This project was created so that I can teach my 10 yo daughter how to program on her tablet using either Thunkable or MIT App Inventor.  The idea is to have her use Scratch from her tablet to control this robot. 

Note, as of now, this project only adds the ability to provide open loop control assertions to the robot.  I will be adding the ability to send back sensor signals via the websocket shortly, once I figure out how MIT App Inventor or Thunkable handle these.  I will be providing a link to a samplel MIT App Inventor project as soon as it is completed.

## License
This project is governed by the MIT License. All rights reserved.

## Author
The main author for this project is Edgar Hilton, edgar.hilton@gmail.com.

## Questions?
Please don't hesitate to reach out to me, edgar.hilton@gmail.com if you have any questions, comments, or ideas.

## Buy Me Coffee?
If you find this code useful, please think of buying me a cup of coffee (I like Starbucks!) by making a Paypal donation to [my Paypal](http://paypal.com.me/mighty2020).
