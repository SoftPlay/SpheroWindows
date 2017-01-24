# SpheroWindows
Playground for driving Sphero from a Windows Universal App

I hope to add lots of different control mechanisms for driving Sphero:
* WPF
* XBox controller
* Voice

# Quick Start - Development

## Pre-requisites
- Visual Studio 2015 with the following options installed from the "Windows and Web Development" section:
  - Universal Windows App Development Tools:
    - Tools (1.4.1) and Windows 10 SDK (10.0.14393)
    - Windows 10.0.10240 SDK

## Building and running the project
Visual Studio may automatically have the project targetted for ARM platforms. Change this to x86. The debug device should already be "Local Machine", but change it to this if it is not.

You do **not** need to install Windows Phone 8.1 SDK, Visual Studio will prompt you to install it if you attempt to build and run this project if the target is set to ARM instead of x86.

# Echo Configuration

## Usage
* Go: "Alexa, ask the ball to go"
* Stop: "Alexa, ask the ball to stop"
* Colour: "Alexa, ask the ball to change colour to COLOUR"

## How it works

When you say the command to Alexa, it triggers the Alexa skill with invocation name "the ball".
The Alexa skill calls a web service running on AWS Lambda, passing it the intent which it identified.
Lambda then posts a message to an amazon message queue (SQS).
The Sphero UWP app retreives the command from the message queue, inerprets the command, and controls any Spheros which it is connected to.
The Sphero UWP posts responses back to SQS, which is read by lambda, and passed back to the Alexa skill

Included here are the Alexa API definitions, the Lambda  AWS service that catches the Alexa requests, and the Sphero UWP app.
The AWS Lambda is currently available in both NodeJs and C#.

To set it up, you need to do the following:

# Create the Alexa skill which will send events to AWS lambda
1. Create a new Skill in the Alexa Skills control panel on Amazon. You need a developer account to do this. The account must be the same as bound to your Echo, and make sure you are logged into that account on amazon.com. You will see an error indicating access denied if the two accounts are different.
2. Name can be whatever you want. "Invocation" is what you say, and it must be all lowercase (I used "the ball" because Alexa didn't understand "Sphero").
3. Check Custom Interaction Model if it is not already checked. Click Next
4. Click Next, taking you to Interaction Model. Create a Custom Slot Type ("Add Slot Type"). Add a new type for COLOURS. Copy/paste the contents of echo/COLOURS.slot.txt.
5. Still in Interaction Model, copy this repo's echo/intents.json into the "Intent Schema" field, and echo/utterances.txt into "Sample Utterances".
6. Don't test yet, just save. Click back to "Skill Information" and copy the "Application ID". You'll need this for Lambda

#Configure the AWS Lambda service that will trigger your sphero UWP app
1. Create an AWS Lambda account if you don't have one already. It's free!
2. In the Lambda console, look to the upper right. Make sure "Ireland" or one of the Lambda supported regions is selected, because not every zone supports Alexa yet.
3. Create a new Lambda function. Skip the blueprint.
4. Pick any name you want, and choose runtime C#.
5. Open the .NET solution in Visual Studio
6. Make sure you have the AWS toolkit extension installed
7. Right-click the AlexaToSphero project and select "Publish to AWS Lambda..."
8. Set function name "AlexaToSphero", assembly name "AlexToSphero" [sic], type name "AlexToSphero.Function", function handler "FunctionHandler"
9. Click "Next"
10. Choose a role which has access to SQS, set the memory to the minimum available, and the timeout to 30 seconds.
11. Press upload.
12. Back on aws.amazon.com make sure your lambda has the trigger "Alexa Skills Kit"

#Connect Alexa Skill to AWS Lambda

In the Lambda console, copy the long "ARN" string in the upper right.
Go back into the Alexa Skill console, open your skill, click "Skill Information", choose Lambda ARN and paste that ARN string in.
Now you're ready to put it all together. Try "Alexa, ask the ball to go"

#TODO - Need to update documentation to detail changing of SQS queue names for different accounts