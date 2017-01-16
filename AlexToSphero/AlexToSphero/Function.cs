using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Amazon.SQS;
using Amazon.SQS.Model;
using Slight.Alexa.Framework.Models.Requests;
using Slight.Alexa.Framework.Models.Requests.RequestTypes;
using Slight.Alexa.Framework.Models.Responses;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AlexToSphero
{
	public class Function
	{
		private const string SqsServerUri = "https://sqs.eu-west-1.amazonaws.com/172566467397/SQS-Sphero-Server";

		private const string SqsClientUri = "https://sqs.eu-west-1.amazonaws.com/172566467397/SQS-Sphero-Client";

		/// <summary>
		/// A simple function that takes a string and does a ToUpper
		/// </summary>
		/// <param name="input"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
		{
			var log = context.Logger;

			if (input.GetRequestType() == typeof(IIntentRequest))
			{
				Response response;

				if (input.Request.Intent.Name == "StartRollIntent")
				{
					log.LogLine("StartRollIntent received");

					response = await StartRollIntent(input.Request);
				}
				else if (input.Request.Intent.Name == "StopRollIntent")
				{
					log.LogLine("StopRollIntent received");
					response = await StopRollIntent(input.Request);
				}
				else if (input.Request.Intent.Name == "SetColourIntent")
				{
					log.LogLine("SetColourIntent received");

					response = await SetColourIntent(input.Request);
				}
				else
				{
					throw new ArgumentException($"Unexpected intent {input.Request.Intent.Name}");
				}

				SkillResponse skillResponse = new SkillResponse();
				skillResponse.Response = response;
				skillResponse.Version = "1.0";

				return skillResponse;
			}
			else
			{
				throw new ArgumentException($"Unexpected request type {input.GetRequestType().Name}");
			}
		}

		private async Task<Response> SetColourIntent(RequestBundle request)
		{
			if (request.Intent.Slots.ContainsKey("Colour"))
			{
				var colour = request.Intent.Slots["Colour"].Value;
				var responseText = await this.SqsRequest($"setColour/{colour}");

				return GetFinalResponse(responseText);
			}
			else
			{
				return GetFinalResponse("I didn't hear the colour that you specified");
			}
		}

		private async Task<Response> StopRollIntent(RequestBundle request)
		{
			var responseText = await this.SqsRequest("stopRoll");

			return GetFinalResponse(responseText);
		}

		private async Task<Response> StartRollIntent(RequestBundle request)
		{
			var responseText = await this.SqsRequest("startRoll");

			return GetFinalResponse(responseText);
		}

		Response GetFinalResponse(string message)
		{
			var response = new Response();
			response.ShouldEndSession = true;
			response.OutputSpeech = new PlainTextOutputSpeech { Text = message };
			return response;
		}

		private async Task<string> SqsRequest(string request)
		{
			// Purge the queue which we will later listen to for responses
			var sqsClient = new AmazonSQSClient(RegionEndpoint.EUWest1);

			try
			{
				await sqsClient.PurgeQueueAsync(SqsServerUri);
			}
			catch (PurgeQueueInProgressException)
			{
				// This happens if we try to purge the queue twice in 60 seconds. Safe to ignore since this is a "just in case" purge
			}

			// Send the request to the client
			await sqsClient.SendMessageAsync(SqsClientUri, request);

			// Wait for a response
			var receiveRequest = new ReceiveMessageRequest(SqsServerUri);
			receiveRequest.MaxNumberOfMessages = 1;
			receiveRequest.VisibilityTimeout = 60;
			receiveRequest.WaitTimeSeconds = 20;
			var responses = await sqsClient.ReceiveMessageAsync(receiveRequest);

			if (responses.Messages.Count == 1)
			{
				var message = responses.Messages[0];

				// delete the message from the queue
				await sqsClient.DeleteMessageAsync(SqsServerUri, message.ReceiptHandle);

				return message.Body;
			}
			else
			{
				return "The sphero service did not respond";
			}
		}
	}
}
