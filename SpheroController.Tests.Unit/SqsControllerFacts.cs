using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using HyperMock;
using Amazon.SQS;
using SpheroController.Common.Interfaces;
using SpheroController.Common.Controllers;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using System.Collections.Generic;
using Windows.UI;

namespace SpheroController.Tests.Unit
{
	[TestClass]
	public class SqsControllerFacts
	{
		private Mock<IAmazonSQS> mockAmazonSqs = Mock.Create<IAmazonSQS>();
		private Mock<IMainPageViewModel> mockSpheroModel = Mock.Create<IMainPageViewModel>();

		[TestMethod]
		public void Ctor_Always_CreatesInstance()
		{
			var sut = this.CreateSut();

			Assert.IsNotNull(sut);
		}

		[TestMethod]
		public async Task StartRollMessageReceived_WhenBallConnected_CallsRoll()
		{
			this.mockSpheroModel.SetupGet(x => x.Count).Returns(1);
			var messages = new List<Message> { new Amazon.SQS.Model.Message { Body = "startRoll" } };
			this.mockAmazonSqs.Setup(x => x.ReceiveMessageAsync(Param.IsAny<ReceiveMessageRequest>(), Param.IsAny<CancellationToken>())).Returns(() => Task<ReceiveMessageResponse>.Run(() => new ReceiveMessageResponse { Messages = messages }));
			this.mockAmazonSqs.Setup(x => x.SendMessageAsync(Param.IsAny<string>(), Param.IsAny<string>(), Param.IsAny<CancellationToken>())).Returns(() => Task<SendMessageResponse>.Run(() => new SendMessageResponse()));
			this.mockAmazonSqs.Setup(x => x.DeleteMessageAsync(Param.IsAny<string>(), Param.IsAny<string>(), Param.IsAny<CancellationToken>())).Returns(() => Task<DeleteMessageResponse>.Run(() => new DeleteMessageResponse() ));

			var sut = this.CreateSut();
			await sut.ReceiveMessages();

			this.mockSpheroModel.VerifySet(x => x.RollDistance, 100, Occurred.Once());
		}

		[TestMethod]
		public async Task StopRollMessageReceived_WhenBallConnected_CallsRoll()
		{
			this.mockSpheroModel.SetupGet(x => x.Count).Returns(1);
			var messages = new List<Message> { new Amazon.SQS.Model.Message { Body = "stopRoll" } };
			this.mockAmazonSqs.Setup(x => x.ReceiveMessageAsync(Param.IsAny<ReceiveMessageRequest>(), Param.IsAny<CancellationToken>())).Returns(() => Task<ReceiveMessageResponse>.Run(() => new ReceiveMessageResponse { Messages = messages }));
			this.mockAmazonSqs.Setup(x => x.SendMessageAsync(Param.IsAny<string>(), Param.IsAny<string>(), Param.IsAny<CancellationToken>())).Returns(() => Task<SendMessageResponse>.Run(() => new SendMessageResponse()));
			this.mockAmazonSqs.Setup(x => x.DeleteMessageAsync(Param.IsAny<string>(), Param.IsAny<string>(), Param.IsAny<CancellationToken>())).Returns(() => Task<DeleteMessageResponse>.Run(() => new DeleteMessageResponse()));

			var sut = this.CreateSut();
			await sut.ReceiveMessages();

			this.mockSpheroModel.VerifySet(x => x.RollDistance, 0, Occurred.Once());
		}

		[TestMethod]
		public async Task SetColourRed_WhenBallConnected_SetsColour()
		{
			this.mockSpheroModel.SetupGet(x => x.Count).Returns(1);
			var messages = new List<Message> { new Amazon.SQS.Model.Message { Body = "setColour/red" } };
			this.mockAmazonSqs.Setup(x => x.ReceiveMessageAsync(Param.IsAny<ReceiveMessageRequest>(), Param.IsAny<CancellationToken>())).Returns(() => Task<ReceiveMessageResponse>.Run(() => new ReceiveMessageResponse { Messages = messages }));
			this.mockAmazonSqs.Setup(x => x.SendMessageAsync(Param.IsAny<string>(), Param.IsAny<string>(), Param.IsAny<CancellationToken>())).Returns(() => Task<SendMessageResponse>.Run(() => new SendMessageResponse()));
			this.mockAmazonSqs.Setup(x => x.DeleteMessageAsync(Param.IsAny<string>(), Param.IsAny<string>(), Param.IsAny<CancellationToken>())).Returns(() => Task<DeleteMessageResponse>.Run(() => new DeleteMessageResponse()));

			var sut = this.CreateSut();
			await sut.ReceiveMessages();

			this.mockSpheroModel.VerifySet(x => x.Colour, Colors.Red, Occurred.Once());
		}

		[TestMethod]
		public async Task SetColourAliceBlue_WhenBallConnected_SetsColour()
		{
			this.mockSpheroModel.SetupGet(x => x.Count).Returns(1);
			var messages = new List<Message> { new Amazon.SQS.Model.Message { Body = "setColour/alice blue" } };
			this.mockAmazonSqs.Setup(x => x.ReceiveMessageAsync(Param.IsAny<ReceiveMessageRequest>(), Param.IsAny<CancellationToken>())).Returns(() => Task<ReceiveMessageResponse>.Run(() => new ReceiveMessageResponse { Messages = messages }));
			this.mockAmazonSqs.Setup(x => x.SendMessageAsync(Param.IsAny<string>(), Param.IsAny<string>(), Param.IsAny<CancellationToken>())).Returns(() => Task<SendMessageResponse>.Run(() => new SendMessageResponse()));
			this.mockAmazonSqs.Setup(x => x.DeleteMessageAsync(Param.IsAny<string>(), Param.IsAny<string>(), Param.IsAny<CancellationToken>())).Returns(() => Task<DeleteMessageResponse>.Run(() => new DeleteMessageResponse()));

			var sut = this.CreateSut();
			await sut.ReceiveMessages();

			this.mockSpheroModel.VerifySet(x => x.Colour, Colors.AliceBlue, Occurred.Once());
		}

		private SqsController CreateSut()
		{
			return new SqsController(this.mockAmazonSqs.Object, this.mockSpheroModel.Object);
		}
	}
}
