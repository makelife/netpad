using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace netpad
{
	public class NtpdServer
	{
		public static void Main ()
		{
			TcpListener tcpL;
			NetworkStream cstream;
			NetworkStream sstream;
			
			Console.WriteLine("Initialising");
			TcpClient client = new TcpClient();
			
			tcpL = new TcpListener(8888);
			tcpL.Start();
			
			
			// The next section creates a fake client (tcpC) to simulate a person connecting to the server.
			
			Console.WriteLine("NtpdDEBUG: Creating a fake client");
			TcpClient tcpC = new TcpClient();
			
			NetworkStream stream;
			tcpC.Connect("localhost", 8888);
			// Debugged out for now
			//if(tcpC.Connected)
			{
				cstream = tcpC.GetStream();
			}
			// End fake client creation and connection
			
			client = tcpL.AcceptTcpClient();
			if(client.Connected)
			{
				sstream = client.GetStream();
				Console.WriteLine("Client connected");
				// The client has connected, let's try to send some data
				Byte[] sendBytes = Encoding.ASCII.GetBytes("Testing Network Messages");
				cstream.Write(sendBytes, 0, sendBytes.Length);
				Console.WriteLine("message sent");
				Byte[] buffer = new Byte[50];
				sstream.Read(buffer, 0, 50);
				Console.WriteLine(Encoding.ASCII.GetString(buffer));
			}
		}
	}
}

