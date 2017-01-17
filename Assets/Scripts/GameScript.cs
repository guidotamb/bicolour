using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour {

	const string gameRecord = "Game Record";

	public float offsetY = 2f;
	public float size;
	public Object circle;
	public GameObject gameOverObject;
	public GameObject flashImage;
	public GameObject pointsText;
	public GameObject pauseObject;
	public GameObject resumeObject;

	private int x;
	private int y;
	private Vector2 initialPos;
	private List<CircleScript> circles;
	private ClockScript clock;
	private int swapedCount;
	private int points = 0;
	private List<Color> colors;
	private bool inGame = true;
	private int lastColor1 = 0;
	private int lastColor2 = 0;

	// Use this for initialization
	void Start () {
		colors = initColors ();
		size = 1.15f;
		swapedCount = 0;
		clock = FindObjectOfType<ClockScript> ();
		circles = new List<CircleScript> (); 
		//Init count of circles and position
		x = Random.Range (2, 5);
		y = Random.Range (2, 5);
		float middleX = x * size / 2;
		float middleY = y * size / 2;
		float middleCircle = (size / 2);
		middleX -= middleCircle;
		middleY -= middleCircle - offsetY;
		initialPos = new Vector2 (-middleX, -middleY);
		// Init 2 colors
		int n1 = Random.Range (0, colors.Count);
		int n2 = Random.Range (0, colors.Count);
		while (n1 == lastColor1 || n1 == lastColor2){
			n1 = Random.Range (0, colors.Count);
		}
		while (n1 == n2 || n2 == lastColor1 || n2 == lastColor2) {
			n2 = Random.Range (0, colors.Count);
		}
		lastColor1 = n1;
		lastColor2 = n2;
		Color color1 = colors [n1];
		Color color2 = colors [n2];
		initCircles (color1, color2);
	}

	private List<Color> initColors(){
		List<Color> colors = new List<Color> ();
		colors.Add (rgba(244,67,54)); //red
		colors.Add (rgba(156,39,176)); //purple
//		colors.Add (rgba(63,81,181)); // indigo
		colors.Add (rgba(33,150,243)); // blue
		colors.Add (rgba(76,175,80)); //green
		colors.Add (rgba(255,235,59)); //yellow
		colors.Add (rgba(255,152,0)); //orange
//		colors.Add (rgba(255,87,34)); //deeporgane
//		colors.Add(rgba(121,85,72)); //brown
		return colors;
	}

	Color rgba(int r, int g, int b){
		return new Color ((float)r/255, (float)g/255, (float)b/255, 0.66f); 
	}

	void initCircles(Color color1, Color color2){
		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				Vector2 pos = new Vector2 ((i * size) + initialPos.x, (j * size) + initialPos.y);
				GameObject o = (GameObject) Instantiate (circle,  pos, Quaternion.identity);
				CircleScript c = o.GetComponent<CircleScript> ();
				c.setOne (color1);
				c.setTwo (color2);
				circles.Add (c);
			}
		}
		postInitCircles ();
	}

	void postInitCircles (){
		int count = circles.Count;
		int random = Random.Range (1, count - 1);
		while (random > 0){
			int i = Random.Range (0, count);
			circles [i].setColor(true);
			random--;
		}
		foreach (CircleScript c in circles) {
			c.chooseColor ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) { 
			SceneManager.LoadScene ("Start");
		}
		if (inGame) {
			foreach (Touch touch in Input.touches) {
				if (touch.phase == TouchPhase.Began) {
					Vector3 touchPos = Camera.main.ScreenToWorldPoint (touch.position);
					RaycastHit2D hit = Physics2D.Raycast (touchPos, Vector2.zero);
					if (hit.collider != null) {
						if (hit.collider.tag == "Circle") {
							CircleScript c = hit.collider.gameObject.GetComponent<CircleScript> ();
							c.swapColor ();
							if (swapedCount < 7) {
								swapedCount++;
							}
							bool win = checkWin ();
							if (win) {
								clock.extraSeconds (swapedCount);
								points += swapedCount;
								pointsText.GetComponent<Text> ().text = "SCORE: " + points.ToString ();
								swapedCount = 0;
								finish ();
							}
						}
					}
				}
			}
		}
	}

	public void finish(){
		flash ();
		foreach (CircleScript o in circles) {
			o.destroy ();
		}
		circles = new List<CircleScript> ();
		Start ();
	}

	private bool checkWin(){
		return sameColor();
	}

	private bool sameColor(){
		int count = 0 ;
		foreach (CircleScript o in circles) {
			count += o.getCount ();
		}
		return count == 0 || count == circles.Count;
	}

	public void gameOver(){
		gameOverObject.SetActive (true);
		int best = PlayerPrefs.GetInt (gameRecord, 0);
		if (points >= best) {
			flash (rgba(76, 175, 80));
		} else {
			flash (rgba (244, 67, 54));
		}
		gameOverObject.GetComponent<GameOver>().setPoints (points);
		clock.stop ();
		inGame = false;
		points = 0;
		swapedCount = 0;
		foreach (CircleScript o in circles) {
			o.destroy ();
		}
		circles = new List<CircleScript> ();
	}

	public void restart(){
		clock.start ();
		inGame = true;
		finish ();
	}

	private void flash(){
		flash (new Color (0.8f, 0.8f, 0.8f, 0.3f));
	}

	private void flash(Color flashColor){
		flashImage.SetActive (true);
		Image image = flashImage.GetComponent<Image> ();
		image.color = flashColor;
		image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
		InvokeRepeating ("doFlash", 0, 0.0001f);
	}

	private void doFlash(){
		Image image = flashImage.GetComponent<Image> ();
		Color temp = image.color;
		if (temp.a > 0) {
			image.color = new Color (temp.r, temp.g, temp.b, temp.a - 0.005f);
		} else {
			flashImage.SetActive (false);
			CancelInvoke ();
		}
	}

	public void pause(){
		clock.stop();
		inGame = false;
		pauseObject.SetActive (false);
		resumeObject.SetActive (true);
	}

	public void resume(){
		clock.resume();
		inGame = true;
		pauseObject.SetActive (true);
		resumeObject.SetActive (false);
	}

}
