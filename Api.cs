using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;

namespace dotnet_websockets {
	[Route("")]
	public class Api: Controller {
		[HttpGet]
		[Route("soc")]
		public async Task Sockets() {
			Console.WriteLine(HttpContext.WebSockets);
			if (HttpContext.WebSockets.IsWebSocketRequest) {
				using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
				await Echo(webSocket);
			} else {
				HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
			}
		}

		private async Task Echo(WebSocket webSocket) {
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			while (!result.CloseStatus.HasValue) {
				await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}
			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}
	}
}