using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClockScript : MonoBehaviour {

	private float seconds;
	private Image progress;
	private GameScript game;
	public int initSeconds;
	private bool active = true;
	private float extraSecond = 1;
	private GameObject newTime;
	public GameObject pointsText;
	public GameObject addPointsText;

	// Use this for initialization
	void Start () {
		progress = gameObject.GetComponent<Image> ();
		game = FindObjectOfType<GameScript>();
		seconds = initSeconds;
		InvokeRepeating ("decreaseExtraSecond", 0, 2);
	}
	
	// Update is called once per frame
	void Update () {
		if (active) {
			Text ptsText = pointsText.GetComponent<Text> ();
			if (seconds < 0) {
				game.gameOver ();
				seconds = initSeconds;
				progress.fillAmount = 1;
				ptsText.text = ((int) seconds).ToString();
			} else {
				seconds -= Time.deltaTime;
				ptsText.text = ((int) seconds).ToString();
				if(seconds / initSeconds < 0){
					progress.fillAmount = 0;	
				} else {
					progress.fillAmount = 1 - (seconds / initSeconds);
				}
			}

		}
	}

	public void extraSeconds(int hits){
		float toAdd = extraSecond + Mathf.Round(hits/4 * 100f) / 100f;
		if (toAdd == 0) {
			toAdd = 0.5f;
		}
		seconds += toAdd;
		if (seconds > initSeconds) {
			seconds = initSeconds;
		}
		addPoints (toAdd);
	}

	void addPoints (float toAdd){
		GameObject plusTime = (GameObject)Instantiate (addPointsText, addPointsText.transform.position, Quaternion.identity);
		plusTime.transform.SetParent(gameObject.transform);
		Text text = plusTime.GetComponent<Text> ();
		text.rectTransform.localScale = new Vector3 (5, 5, 1);
		text.rectTransform.localPosition = new Vector3 (-30, -430, 0);
		plusTime.GetComponent<PlusTime> ().doAddPoints (toAdd);
	}

	void decreaseExtraSecond(){
		if (extraSecond < 0) {
			extraSecond = 0;
			CancelInvoke ();
		} else {
			extraSecond -= 0.1f;
		}
	}

	public void stop(){
		active = false;
	}

	public void resume(){
		active = true;
	}

	public void start(){
		active = true;
		progress.fillAmount = 1;
		seconds = initSeconds;
		extraSecond = 1;
		InvokeRepeating ("decreaseExtraSecond", 0, 2);
	}
}
