using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace netpad
{
	public class NtpdServer
	{		
		public static void Main ()
		{
			new NtpdServer();
		}
		
		NtpdPersistentData dataStorage;
		NtpdServerCommands cmdInterface;
		TcpListener tcpServer;
		StreamConverter sconv;
		
		private Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
		
		public NtpdServer()
		{	
			dataStorage = new NtpdPersistentData();
			cmdInterface = new NtpdServerCommands(this, dataStorage);
			sconv = new StreamConverter();
			tcpServer = new TcpListener(IPAddress.Any, 8888);
			Thread listenThread = new Thread(new ThreadStart(NewClient));
			listenThread.Start();
			
			// Currently this creates five fake clients to connect to the fledgeling server.
			
			for(int i = 0; i<5; i++)
			{
				TcpClient tcpC = new TcpClient();
			
				tcpC.Connect("localhost", 8888);
				
				Thread clientThread = new Thread(new ParameterizedThreadStart(ClientMain));
    			clientThread.Start(tcpC);
			}			
		}
		
		// This method runs in listenThread and spends 90% of its time halted.
		public void NewClient()
		{
			tcpServer.Start();
			
			while (true)
			{
				// This will halt execution until a client connects.
				TcpClient client = tcpServer.AcceptTcpClient();
				
				Thread clientThread = new Thread(new ParameterizedThreadStart(ServerClientListener));
    			clientThread.Start(client);
			}
		}
		
		// This method is for debugging purposes. It should not exist here - this is the server code.
		public void ClientMain(object clientIn)
		{
			TcpClient client = (TcpClient)clientIn;
			NetworkStream cstream = client.GetStream();
			
			Console.WriteLine("- Client Thread Starting");
			
			string username = "James Telfer2";
			
			Byte[] outbytes = sconv.SetStreamLoginData(username, "password");
			cstream.Write(outbytes, 0, outbytes.Length);
			
			while(true)
			{
						
				Byte[] netbuffer = new Byte[256];
				string[] packet;
				
				//Console.WriteLine("-CLIENT- Waiting for message...");
				cstream.Read(netbuffer, 0, 256);
				packet = NetbufferToPacket(netbuffer);				
				//Console.WriteLine("-CLIENT- Recieved packet");
				switch(packet[0])
				{
				case "1":
					// We should initialise some GUI stuff now. We've got our authentication confirmation.
					Console.WriteLine("-CLIENT- Authentication Confirmed.");
					break;
				case "2":
					// Authentication failed.
					Console.WriteLine("-CLIENT- Authentication failed - packet recived.");
					break;
				}
			}
		}
		
		// This method is the server-side client communication thread. A process of this type will be continuously running for each attached client.
		public void ServerClientListener(object clientIn)
		{
			Console.WriteLine("[System] >>> Client connection thread started at " + DateTime.Now.ToString());
			
			// Initialise a bunch of variables. 
			// As this thread is permanently running, we don't want to have memory leaks, and as such we'll use the same variables.
			TcpClient client = (TcpClient)clientIn;
			NetworkStream cstream = client.GetStream();
			Byte[] netbuffer = new Byte[256];
			string[] packet;
			
			// Wait until the first connection occurs
			while(!cstream.DataAvailable)
			{
			}
			
			// Bring the header of a packet into netbuffer.
			// The packet header consists of &ID|length|
			// ID and Length are one byte each.
			// Length is an unsigned 8-bit integer, with the result that maximum message length is 255 characters.
			cstream.Read(netbuffer, 0, 6);
			
			// packet stores a list of the items in a given data stream - in this case, packet[0] is ID and packet[1] is length.
			packet = NetbufferToPacket(netbuffer);
			if(packet[0] == "0")
			{
				Console.WriteLine("DEBUG: Receiving login data from client...");
				cstream.Read(netbuffer, 6, Convert.ToInt32(packet[1]));
				packet = NetbufferToPacket(netbuffer);
				
				bool auth = dataStorage.CheckUserAuth(packet[2], packet[3]);
				
				if(auth)
				{
					Object clientArrayLockObject = new Object();
					Monitor.Enter(clientArrayLockObject);
					{
						try
						{
							clients.Add(packet[2], client);
						}
						catch (Exception e)
						{
							Console.WriteLine("[ERROR] >>> " + e.Message + " [Adding client to online list]");
						}
					}
					Monitor.Exit(clientArrayLockObject);
					Console.WriteLine("[Info] >>> " + packet[2] + " authenticated.");
				}
				else
				{
					Console.WriteLine("[Info] >>> " + packet[2] + " failed authentication.");
					byte[] outbytes = sconv.SetStreamAuthFailed();
					cstream.Write(outbytes, 0, outbytes.Length);
				}
			}
			
			while (true)
			{
				//if(client.)
			}
		}
		
		public string[] NetbufferToPacket (byte[] netbuffer)
		{
			// netinput stores whatever is being read straight to string, with no splitting or changes.
			// When this is split using packetDelimiter, the 0th element will generally be null.
			string netinput = Encoding.ASCII.GetString(netbuffer);
			string[] packet = netinput.Split(StreamConverter.packetDelimiter);
			return packet[1].Split(StreamConverter.subpacketDelimiter);
		}
	}
}

