using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using SpheroController.Common.Interfaces;
using Windows.UI;

namespace SpheroController.Common.Controllers
{
	public class SqsController : ISqsController, IDisposable
	{
		private IAmazonSQS sqsClient;

		private IMainPageViewModel spheroModel;

		private bool disposedValue = false;

		private CancellationTokenSource cancellationToken = new CancellationTokenSource();

		Dictionary<string, Color> ColorNames;

		public SqsController(IAmazonSQS sqsClient, IMainPageViewModel spheroModel)
		{
			this.sqsClient = sqsClient;
			this.spheroModel = spheroModel;

			ColorNames = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);
			foreach (var color in typeof(Colors).GetRuntimeProperties())
			{
				ColorNames[color.Name] = (Color)color.GetValue(null);
			}
		}

		public async Task StartAsync()
		{
			while (!this.cancellationToken.IsCancellationRequested)
			{
				await ReceiveMessages();
			}
		}

		public async Task ReceiveMessages()
		{
			ReceiveMessageRequest receiveRequest = new ReceiveMessageRequest("https://sqs.eu-west-1.amazonaws.com/172566467397/SQS-Sphero-Client");
			receiveRequest.MaxNumberOfMessages = 1;
			receiveRequest.WaitTimeSeconds = 20;
			var result = await this.sqsClient.ReceiveMessageAsync(receiveRequest, this.cancellationToken.Token);

			if (!this.cancellationToken.IsCancellationRequested)
			{
				foreach (var message in result.Messages)
				{
					System.Diagnostics.Debug.WriteLine("Message received: " + message.Body);

					var response = this.ProcessMessage(message.Body);

					await this.sqsClient.SendMessageAsync("https://sqs.eu-west-1.amazonaws.com/172566467397/SQS-Sphero-Server", response);

					await this.sqsClient.DeleteMessageAsync("https://sqs.eu-west-1.amazonaws.com/172566467397/SQS-Sphero-Client", message.ReceiptHandle);
				}
			}
		}

		private string ProcessMessage(string body)
		{
			if (string.IsNullOrEmpty(body))
			{
				return "Something went wrong. An empty message was received";
			}

			if (this.spheroModel.Count == 0)
			{
				return "I have no balls";
			}

			if (body.Equals("startroll", StringComparison.OrdinalIgnoreCase))
			{
				this.spheroModel.RollDistance = 100;

				return "Started rolling sphero";
			}
			else if (body.Equals("stoproll", StringComparison.OrdinalIgnoreCase))
			{
				this.spheroModel.RollDistance = 0;

				return "Stopped rolling sphero";
			}
			else if (body.StartsWith("setcolour", StringComparison.OrdinalIgnoreCase))
			{
				var arr = body.Split('/');

				if (arr.Length == 2)
				{
					// remove spaces in name (e.g. "alice blue" becomes "aliceblue")
					var colourName = arr[1].Replace(" ", string.Empty);

					Color colour;

					if (ColorNames.TryGetValue(colourName, out colour))
					{
						this.spheroModel.Colour = colour;

						return string.Format("Set the colour of sphero to {0}", arr[1]);
					}
					else
					{
						return string.Format("I don't know what colour {0} is", arr[1]);
					}
				}
				else
				{
					return "You must specify the colour. Say something like \"ask sphero to set the colour to blue\"";
				}
			}
			else
			{
				return "Something went wrong. The commnd received at the Sphero service was unexpected.";
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.cancellationToken.Cancel();
					this.cancellationToken.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}
