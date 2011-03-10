// Netpad [ntpd]
// Netpad is released under the Artistic Licence 2.0. The full licence can be found in the root directory.
// Contributing authors: James Telfer [jt]

// ntpdClientConnection
// The networking interface initilisation class.
// Handles display of servers available, joining servers, starting a new one and disconnecting.

// NOTES
// System.Collections.Generic is a class with useful statically typed lists and dictionaries.
// Window IDs 0 through 10 are reserved for use by ClientConnection

// TODO
// (Line 13 in Global) This is an example of a todo, with the author who contributed it [jt]

// Window IDs
// 1 : Password Protected Server Dialog

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ntpdClientConnection : MonoBehaviour {

	public string masterServerIP = "127.0.0.1"; // This IP will also be used for the Facilitator.
	public int masterServerPort = 0; // Change this to the default masterserver port.
	public int facilitatorPort = 0; // Change this to the default masterserver port.
	
	float lastRequest = -100.0f;
	float requestRefreshTime = 10.0f; // How often do we check the server list?
	HostData[] servers;
	
	bool showPasswordWindow = false;
	string serverPassword = ""; // Stores the password for connecting to servers and the password for setting up servers. Remember to reset.
	HostData currentServer = null; // Used to store the server to connect to while the password dialog is shown.
	
	void Start () 
	{
		// [jt]
		// Currently, the Unity Default Master Server is being used.
		// Uncomment these lines when and if a masterserver is reliably hosted.
		
		// MasterServer.ipAddress = masterServerIP;
		// MasterServer.port = masterServerPort;
		// Network.natFacilitatorIP = masterServerIP;
		// Network.natFacilitatorPort = facilitatorPort
		
		
		// [jt]
		// CURRENTLY USING TO DEBUG THE HOST LIST
		Network.InitializeServer(32, 25000, true);
		//Network.incomingPassword = "obsidian";
		MasterServer.RegisterHost("netpad", "James Telfer", "Testing Netpad.");
		
	}
	
	void Update ()
	{
		//if (Network.peerType == NetworkPeerType.Disconnected)
		{
			if (Time.time >= lastRequest + requestRefreshTime)
			{
				MasterServer.RequestHostList("netpad");
				Debug.Log("Requesting host list from Master Server.");
				lastRequest = Time.time;
			}
			servers = MasterServer.PollHostList();
		}
	}
	
	void OnGUI ()
	{
		//if (Network.peerType == NetworkPeerType.Disconnected)
		{
			if(showPasswordWindow == true)
			{
				//new Rect((Screen.width/2) - 50, (Screen.width/2) - 25, 100, 50)
				GUILayout.Window(1, new Rect(100, 100, 300, 50) , GUIWindowPasswordProtectedServer, "Enter password to connect");
			}
			
			GUILayout.BeginArea(new Rect(20, 20, 100, Screen.height));
			GUILayout.BeginVertical();
			if (servers != null && servers.Length > 0)
			{
				foreach (HostData server in servers)
				{
					GUILayout.BeginVertical("box");
					GUILayout.Label(server.gameName, "box");
					GUILayout.Space(2);
					GUILayout.Label(server.comment);
					if (server.passwordProtected)
					{
						GUILayout.Label("Protected");
						if(GUILayout.Button("Join"))
						{
							showPasswordWindow = true;
							currentServer = server;
						}
					}
					else if(GUILayout.Button("Join"))
					{
						Network.Connect(server);
					}
					GUILayout.EndVertical();
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
	
	void GUIWindowPasswordProtectedServer (int id)
	{
		GUILayout.BeginVertical();
		serverPassword = GUILayout.PasswordField(serverPassword, "*"[0]);
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Connect"))
		{
			Network.Connect(currentServer, serverPassword);
			showPasswordWindow = false;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	void OnFailedToConnect (NetworkConnectionError e)
	{
		if (e == NetworkConnectionError.InvalidPassword)
		{
			Debug.Log("That was not the correct password.");
		}
		else if (e == NetworkConnectionError.TooManyConnectedPlayers)
		{
			Debug.Log("This server is already full.");
		}
		else
		{
			Debug.Log("Failed to connect. The server may be down, or you may have incompatible network types.");
		}
	}
	
}
