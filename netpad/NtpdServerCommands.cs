using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace netpad
{
	public class NtpdServerCommands
	{
		NtpdServer main;
		NtpdPersistentData data;
		
		public NtpdServerCommands (NtpdServer parent, NtpdPersistentData dataIn)
		{
			main = parent;
			dataIn = dataIn;
			
			Thread commandsThread = new Thread(CommandListener);
			commandsThread.Start();
		}
				
		// This is the commands interface.
		// Case statements should contain the first word of the command only.
		// Commands should be organised hierarchically, user -> dc -> [username]
		
		// Commands List
		/// Commands should be entered exactly as typed into console.
		/// Use [arg] to denote arguments
		/// 
		/// stop
		/// - Ends the program, cancelling all connections (probably causing buttloads of errors)
		/// - Add: A '-g' method that sends shutdown signals to all clients before closing. 
		/// 
		/// user list
		/// - Lists the usernames of all connected clients.
		/// 
		/// user dc [username]
		/// - Disconnects the chosen user.
		/// 
		/// user sm [username]
		/// - Sends the user a message from the console
		/// 
		/// data users -r
		/// - Reloads all user data from hard-files. 
		/// - Used in event of corruption or changed data outside system.
		/// 
		/// data users -d [username]
		/// - Permanently deletes a user from the data files.
		/// 
		// Commands List End
		
		public void CommandListener()
		{
			while(true)
			{
				string[] input = Console.ReadLine().Split(" "[0]);
				int args = input.Length;
				switch(input[0])
				{
				case "":
					break;
				case "stop":
					Console.WriteLine(">>> Shutting down");
					Environment.Exit(Environment.ExitCode);
					break;
				case "user":
					if(args > 1)
					{
						if (input[1] == "dc" && args > 2)
						{
							DisconnectUser(input[2]);
						}
						else if (input[1] == "sm" && args > 3)
						{
							SendLogMessage(input[2], input[3]);
						}
					}
					break;
				case "data":
					if(args > 2)
					{
						if (input[1] == "users" && input[2] == "-r")
						{
							data.LoadUsersFile();
						}
						else if (input[1] == "users" && input[2] == "-d" && args > 3)
						{
							//data.RemoveUserData(input[3]);
						}
					}
					break;
				}
			}
		}
		
		public void DisconnectUser(string username)
		{
			Console.WriteLine(">>> Disconnecting user: " + username);
		}
		
		public void SendLogMessage(string username, string message)
		{
			Console.WriteLine(">>> Sending message: " + message + " to: " + username);
		}
	}
}

