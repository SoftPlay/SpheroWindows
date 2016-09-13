# SpheroWindows
Playground for driving Sphero from a Windows Universal App

I hope to add lots of different control mechanisms for driving Sphero:
* WPF
* XBox controller
* Voice


## Pre-requisites
- Visual Studio 2015 with the following options installed from the "Windows and Web Development" section:
  - Universal Windows App Development Tools:
    - Tools (1.4.1) and Windows 10 SDK (10.0.14393)
    - Windows 10.0.10240 SDK

## Building and running the project
Visual Studio may automatically have the project targetted for ARM platforms. Change this to x86. The debug device should already be "Local Machine", but change it to this if it is not.

You do **not** need to install Windows Phone 8.1 SDK, Visual Studio will prompt you to install it if you attempt to build and run this project if the target is set to ARM instead of x86.