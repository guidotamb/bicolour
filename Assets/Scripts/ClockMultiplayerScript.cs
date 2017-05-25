using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class ClockMultiplayerScript : NetworkBehaviour
{
	[SyncVar]
	private float seconds;
	[SyncVar]
	private float timeElapsed = 0;
	[SyncVar]
	private float dificultyFactor;
	[SyncVar]
	public float initialDificulty = 500;
	[SyncVar]
	private bool active = false;
	[SyncVar]
	private float extraSecond = 1;
	[SyncVar]
	public int initSeconds;

	public GameObject pointsText;
	public GameObject addPointsText;

	private GameObject newTime;
	private Image progress;
	private MultiplayerGameScript game;

	// Use this for initialization
	void Awake ()
	{
		progress = gameObject.GetComponent<Image> ();
		game = FindObjectOfType<MultiplayerGameScript> ();
		seconds = initSeconds;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (active) {
			Text ptsText = pointsText.GetComponent<Text> ();
			if (seconds < 0) {
				game.gameOver ();
				seconds = initSeconds;
				timeElapsed = 0;
				progress.fillAmount = 1;
				ptsText.text = seconds.ToString ("F1");
			} else {
				seconds -= Time.deltaTime;
				timeElapsed += Time.deltaTime;
				ptsText.text = seconds.ToString ("F1");
				if (seconds / initSeconds < 0) {
					progress.fillAmount = 0;	
				} else {
					progress.fillAmount = 1 - (seconds / initSeconds);
				}
			}

		}
	}

	public void extraSeconds (int minSolution, int hits)
	{
		float plusTouches = ((float) minSolution / hits);
		float plusVelocity = (0.2f * hits / timeElapsed);
		plusVelocity = plusVelocity > 1.2f ? 1.2f : plusVelocity;
		plusVelocity = plusVelocity < 0 ? 0 : plusVelocity;

		float plus = (plusTouches * plusVelocity) * dificultyFactor / initialDificulty;
		seconds += plus;
		if (seconds > initSeconds) {
			seconds = initSeconds;
		}
		addPoints (plus);
		timeElapsed = 0;
		if (dificultyFactor < initialDificulty / 4) {
			dificultyFactor--;
		}
	}

	void addPoints (float toAdd)
	{
		GameObject plusTime = (GameObject)Instantiate (addPointsText, addPointsText.transform.position, Quaternion.identity);
		plusTime.transform.SetParent (gameObject.transform);
		Text text = plusTime.GetComponent<Text> ();
		text.rectTransform.localScale = new Vector3 (5, 5, 1);
		text.rectTransform.localPosition = new Vector3 (-20, -430, 0);
		plusTime.GetComponent<PlusTime> ().doAddPoints (toAdd);
	}

	public void stop ()
	{
		active = false;
	}

	public void resume ()
	{
		active = true;
	}

	public void start ()
	{
		active = true;
		seconds = initSeconds;
		extraSecond = 1;
		dificultyFactor = initialDificulty;
	}
}
