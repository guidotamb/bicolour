using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartScene : MonoBehaviour {

	public GameObject image;

	// Use this for initialization
	void Start () {
//		Vector2 size = new Vector2 (Screen.width, Screen.height);
//		Debug.Log (size);
//		image.GetComponent<Image> ().rectTransform.sizeDelta = size;
		GetComponent<UnityAdsExample>().ShowAd();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
	}

	public void play(){
		SceneManager.LoadScene ("ColorTap");
	}
}
