using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameOver : MonoBehaviour {

	const string gameRecord = "Game Record";

	public Text text;
	public Text recordText;
	public GameObject time;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void setPoints(int points){
		int record = PlayerPrefs.GetInt (gameRecord, 0);
		if (points > record) {
			PlayerPrefs.SetInt (gameRecord, points);
			recordText.text = "BEST: " + points;
			recordText.color = rgba (76, 175, 80, 1);
			text.text = "";
		} else {
			text.text = "SCORE: " + points;
			text.color = rgba (158, 158, 158, 1);
			recordText.text = "BEST: " + record;
			recordText.color = rgba(158, 158, 158,1);
		}

		time.SetActive (false);
	}

	public void restart(){
		time.SetActive (true);
		gameObject.SetActive (false);
		FindObjectOfType<GameScript> ().restart ();
	}

	Color rgba(int r, int g, int b, int a){
		return new Color ((float)r / 255, (float)g / 255, (float)b / 255, a);
	}
}
