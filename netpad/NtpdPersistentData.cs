using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace netpad
{
	public class NtpdPersistentData
	{
		private static string dataPath = "/Users/James/netpad/data";
		
		public Dictionary<string, NtpdUserData> users = new Dictionary<string, NtpdUserData>();
		
		public NtpdPersistentData ()
		{
			LoadUsersFile();
		}
		
		public NtpdUserData GetUserData(string username)
		{
			return users[username];
		}
		
		public void LoadUsersFile()
		{
			// Open the users file. If it does not exist (new server or debugging?) it is created.
			using (FileStream stream = File.Open(dataPath + "/usr.txt", FileMode.OpenOrCreate))
			{
				byte[] b = new byte[stream.Length];
				stream.Read(b, 0, (int)stream.Length);
				
				// For each user in the file, create the NtpdUserData structure and populate it.
				// Add it to the users dictionary.
				
				string[] userStrings = Encoding.ASCII.GetString(b).Split(Environment.NewLine[0]);
				
				for (int i = 0; i<userStrings.Length; i++)
				{
					string[] userData = userStrings[i].Split("|"[0]);
					NtpdUserData newUser = new NtpdUserData(userData[0], userData[1]);
					
					// Implement later - maybe - if we go for server-based friends list
					/*for (int i2 = 2; i2<userData.Length; i2++)
					{
						newUser.friends.Add(userData[i2]);
					}*/
					
					users.Add(userData[0], newUser);
				}
				
			}
		}
		
		public void RemoveUserData()
		{
			
		}
		
		public bool CheckUserAuth(string username, string password)
		{
			if(!users.ContainsKey(username))
			{
				return false;
			}
			
			if(password == users[username].password)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
	}
	
	public class NtpdUserData
	{
		public string username = "";
		public string password = "";
		
		//public List<String> friends = new List<String>();
		
		public string dataPath = "";
		
		public NtpdUserData()
		{
		}
		
		// Defaults to storing user data in netpad/data/usr/data/username
		public NtpdUserData(string usernameIn, string passwordIn)
		{
			username = usernameIn;
			password = passwordIn;
		}
		
		// This constructor sets an explicit data path
		public NtpdUserData(string username, string password, string dataPath)
		{
			
		}
	}
	
}

