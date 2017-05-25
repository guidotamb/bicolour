using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class OverriddenNetworkDiscovery : NetworkDiscovery
{

	void Start() {
	}

	public void StartDiscovery() {
		Initialize ();
		StartAsClient ();
	}

	public override void OnReceivedBroadcast(string fromAddress, string data)
	{
		NetworkManager.singleton.networkAddress = fromAddress;
		Debug.Log ("Trying to connect to: " + fromAddress);
		NetworkManager.singleton.StartClient();
	}
}