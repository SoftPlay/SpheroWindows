'use strict';

var AWS = require('aws-sdk');

var options = require('./options');

var serverUrl = '';
var clientUrl = '';
var sqsServer = null;
var sqsClient = null;

var AlexaSkill = require('./AlexaSkill');
var EchoSphero = function () {
    AlexaSkill.call(this, options.appid);
};

EchoSphero.prototype = Object.create(AlexaSkill.prototype);
EchoSphero.prototype.constructor = EchoSphero;

EchoSphero.prototype.intentHandlers = {


    StartRollIntent: function (intent, session, response) {
        console.log("StartRollIntent received");
        sqsreq('startRoll', function (error, successMessage) {
            genericResponse(error, response, successMessage);
        });
    },

    StopRollIntent: function (intent, session, response) {
        console.log("StopRollIntent received");
        sqsreq('stopRoll', function (error, successMessage) {
            genericResponse(error, response, successMessage);
        });
    },

    SetColourIntent: function (intent, session, response) {
        console.log("SetColourIntent received");
        sqsreq('setColour/' + intent.slots.Colour.value, function (error, successMessage) {
            genericResponse(error, response, successMessage);
        });
    }
};

function sqsreq(command, responseCallback) {
    sqsServer.purgeQueue({ QueueUrl: serverUrl }, function (err, data) {
        console.log("sending SQS " + command);
        sqsClient.sendMessage({
            MessageBody: command,
            QueueUrl: clientUrl
        },
            function (err, data) {
                if (err) {
                    console.log('ERR', err);
                } else {
                    console.log(data);
                    sqsServer.receiveMessage({
                        QueueUrl: serverUrl,
                        MaxNumberOfMessages: 1, // how many messages do we wanna retrieve?
                        VisibilityTimeout: 60, // seconds - how long we want a lock on this job
                        WaitTimeSeconds: 20 // seconds - how long should we wait for a message?
                    },
                        function (err, data) {
                            var message = data.Messages[0];
                            var response = message.Body;
                            if (!err) {
                                sqsServer.deleteMessage({
                                    QueueUrl: serverUrl,
                                    ReceiptHandle: message.ReceiptHandle
                                }, function (err, data) {
                                    responseCallback(undefined, response);
                                    if (err) {
                                        console.log(err);
                                    }
                                });
                            } else {
                                console.log(err);
                                responseCallback(err);
                            }
                        }
                    );
                }
            }
        );
    });
}

function genericResponse(error, response, successMessage) {
    if (!error) {
        if (!successMessage) {
            response.tell("OK");
        }
        else {
            response.tell(successMessage);
        }
    }
    else {
        response.tell("The Lambda service encountered an error: " + error.message);
    }
}

// Create the handler that responds to the Alexa Request.
exports.handler = function (event, context) {
    // Create an instance of the EchoSonos skill.
    var echoSphero = new EchoSphero();
    
    var region = process.env.AWS_REGION;
    var arn = context.invokedFunctionArn;
    var actLoc = arn.indexOf(region) + region.length + 1;
    var accountId = arn.substring(actLoc, arn.indexOf(':', actLoc));
    var baseSqsUrl = "https://sqs." + region + ".amazonaws.com/" + accountId;
    serverUrl = baseSqsUrl + "/SQS-Sphero-Server";
    clientUrl = baseSqsUrl + "/SQS-Sphero-Client";
    sqsServer = new AWS.SQS({ region: region });
    sqsClient = new AWS.SQS({ region: region });
    
    echoSphero.execute(event, context);
};