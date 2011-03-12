using System;
using System.Text;
using System.Collections.Generic;

namespace netpad
{
	public class StreamConverter
	{
		public static char subpacketDelimiter = "|"[0];
		public static char packetDelimiter = "&"[0];
		
		// Stores and gives out the various reference IDs for streaming
		public Dictionary<string, int> streamIDs = new Dictionary<string, int>();
		
		public StreamConverter ()
		{
			streamIDs.Add("logindata", 0);
			streamIDs.Add("authenticationfailed", 2);
		}
		
		
		// Format: &ID|length|arg1|arg2|...argN|
		// ID can be found in streamIDs
		// length refers to the length of the arg section, including argument delimiters and the end-of-packet delimiter
		
		// NOTE: For String.Split reasons, the packet joints look like old|&new - that is, packetDelimiter is ONLY used at the start of the packet.
		
		public Byte[] SetStreamLoginData(string username, string password)
		{
			int packetID = streamIDs["logindata"];
			//md5hash the password
			string message = username + subpacketDelimiter + password + subpacketDelimiter;
			int dataLength = Encoding.ASCII.GetBytes(message.ToString()).Length;
			StringBuilder b = new StringBuilder();
			b.Append(packetDelimiter);
			b.Append(packetID);
			b.Append(subpacketDelimiter);
			b.Append(dataLength);
			b.Append(subpacketDelimiter);
			b.Append(message);
			return Encoding.ASCII.GetBytes(b.ToString());
		}
		
		public Byte[] SetStreamAuthFailed()
		{
			int packetID = streamIDs["authenticationfailed"];
			int dataLength = 1;
			
			StringBuilder b = new StringBuilder();
			b.Append(packetDelimiter);
			b.Append(packetID);
			b.Append(subpacketDelimiter);
			b.Append(dataLength);
			b.Append(subpacketDelimiter);
			
			return Encoding.ASCII.GetBytes(b.ToString());
		}
	}
}

