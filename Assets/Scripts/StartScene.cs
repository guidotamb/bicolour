using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartScene : MonoBehaviour
{
	private NetworkManager networkManager;

	void Start ()
	{
		networkManager = NetworkManager.singleton;
		//GetComponent<UnityAdsExample>().ShowAd();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}

	public void play ()
	{
		SceneManager.LoadScene ("ColorTap");
	}

	public void host ()
	{
		NetworkManager.singleton.StartHost ();
		NetworkDiscovery nd = new NetworkDiscovery();
		nd.Initialize();
		nd.StartAsServer();
	}

	public void joinGame ()
	{
		SceneManager.LoadScene ("SearchHost");
	}
		
}
