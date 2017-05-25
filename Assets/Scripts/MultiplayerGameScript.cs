using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class MultiplayerGameScript : NetworkBehaviour
{

	const string gameRecord = "Game Record";

	public float offsetY = 2f;
	public float size = 1.25f;
	public Object circle;
	public GameObject gameOverObject;
	public GameObject flashImage;
	public GameObject pointsText;
	public GameObject pauseObject;
	public GameObject resumeObject;
	public GameObject waitingObject;
		
	private int x;
	private int y;
	private Vector2 initialPos;
	private List<CircleScript> circles = new List<CircleScript> ();
	private ClockMultiplayerScript clock;
	private List<Color> colors;

	static public MultiplayerGameScript singleton;

	[SyncVar]
	private int swapedCount = 0;
	[SyncVar]
	private int points = 0;
	[SyncVar]
	private bool inGame = false;
	[SyncVar]
	private bool waitingForUser = true;
	[SyncVar]
	private int lastColor1 = -1;
	[SyncVar]
	private int lastColor2 = -1;
	[SyncVar]
	private int randomColors;

	void Awake ()
	{
		singleton = this;
		colors = initColors ();
		clock = FindObjectOfType<ClockMultiplayerScript> ();
	}

	private List<Color> initColors ()
	{
		List<Color> colors = new List<Color> ();
		colors.Add (rgba (244, 67, 54)); //red
		colors.Add (rgba (156, 39, 176)); //purple
		//		colors.Add (rgba(63,81,181)); // indigo
		colors.Add (rgba (33, 150, 243)); // blue
		colors.Add (rgba (76, 175, 80)); //green
		colors.Add (rgba (255, 235, 59)); //yellow
		colors.Add (rgba (255, 152, 0)); //orange
		//		colors.Add (rgba(255,87,34)); //deeporgane
		//		colors.Add(rgba(121,85,72)); //brown
		return colors;
	}

	void startGame ()
	{
		inGame = true;
		clock.start ();
		
		//Init count of circles and position
		x = Random.Range (2, 5);
		y = Random.Range (2, 5);
		float middleCircle = (size / 2);
		float middleX = (x * size / 2) - middleCircle;
		float middleY = (y * size / 2) - middleCircle;
		initialPos = new Vector2 (-middleX, -middleY);
		// Init 2 colors
		int n1 = -1;
		int n2 = -1;
		while (true) {
			int i = Random.Range (0, colors.Count - 1);
			if (i != lastColor1 && i != lastColor2) {
				if (n1 == -1) {
					n1 = i;
					lastColor1 = n1;
				} else {
					n2 = i;
					lastColor2 = n2;
					break;
				}
			}
		}
		initCircles (colors [n1], colors [n2]);
	}

	Color rgba (int r, int g, int b)
	{
		return new Color ((float)r / 255, (float)g / 255, (float)b / 255, 0.66f); 
	}

	void initCircles (Color color1, Color color2)
	{
		int cantTotal = x * y;
		Debug.Log ("Cantidad Total: " + cantTotal);
		int random1 = (cantTotal / 2) - 1;
		int random2 = (cantTotal / 2) + 1;
		Debug.Log ("Random1: " + random1);
		Debug.Log ("Random2: " + random2);
		randomColors = Random.Range (random1, random2 + 1);

		List<Vector2> randomPos = getRandomPos (randomColors, x, y);

		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				Vector2 pos = new Vector2 ((i * size) + initialPos.x, (j * size) + initialPos.y);
				GameObject o = (GameObject)Instantiate (circle, pos, Quaternion.identity);
				CircleScript c = o.GetComponent<CircleScript> ();

				Vector2 position = new Vector2 (i, j);
				if (randomPos.Contains (position)) {
					setColors (color1, color2, c);
				} else {
					setColors (color2, color1, c);
				}

				circles.Add (c);
				NetworkServer.Spawn (o);
			}
		}
	}

	private void setColors (Color color1, Color color2, CircleScript c)
	{
		c.setOne (color1);
		c.setTwo (color2);
	}

	private List<Vector2> getRandomPos (int random, int x, int y)
	{
		List<Vector2> result = new List<Vector2> ();
		while (random > 0) {
			result.Add (new Vector2 (Random.Range (0, x), Random.Range (0, y)));
			random--;
		}
		return result;
	}

	[ServerCallback]
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) { 
			SceneManager.LoadScene ("Start");
		}
		if (waitingForUser) {
			if (NetworkManager.singleton.numPlayers == 2) {
				waitingObject.SetActive (false);
				pauseObject.SetActive (true);
				waitingForUser = false;
				startGame ();
			}
		}
		if (inGame) {
			foreach (Touch touch in Input.touches) {
				if (touch.phase == TouchPhase.Began) {
					Vector3 touchPos = Camera.main.ScreenToWorldPoint (touch.position);
					RaycastHit2D hit = Physics2D.Raycast (touchPos, Vector2.zero);
					if (hit.collider != null) {
						if (hit.collider.tag == "Circle") {
							CircleScript c = hit.collider.gameObject.GetComponent<CircleScript> ();
							if (!c.isSwapped ()) {
								swapedCount++;
							}
							c.swapColor ();
							c.setSwapped (true);
							bool win = sameColor ();
							if (win) {
								int minSolution = randomColors > circles.Count ? (circles.Count - randomColors) : randomColors;
								clock.extraSeconds (minSolution, swapedCount);
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

	public void finish ()
	{
		flash ();
		foreach (CircleScript o in circles) {
			o.destroy ();
		}
		circles = new List<CircleScript> ();
		startGame ();
	}

	private bool sameColor ()
	{
		Color color = circles [0].getColor ();
		for (int i = 1; i < circles.Count; i++) {
			if (color != circles [i].getColor ()) {
				return false;
			}
		}
		return true;
	}

	public void gameOver ()
	{
		gameOverObject.SetActive (true);
		pauseObject.SetActive (false);
		int best = PlayerPrefs.GetInt (gameRecord, 0);
		if (points > best) {
			flash (rgba (76, 175, 80));
		} else {
			flash (rgba (244, 67, 54));
		}
		gameOverObject.GetComponent<GameOver> ().setPoints (points);
		clock.stop ();
		inGame = false;
		points = 0;
		swapedCount = 0;
		foreach (CircleScript o in circles) {
			o.destroy ();
		}
		circles = new List<CircleScript> ();
	}

	public void restart ()
	{
		clock.start ();
		pauseObject.SetActive (true);
		inGame = true;
		finish ();
	}

	private void flash ()
	{
		flash (new Color (0.8f, 0.8f, 0.8f, 0.3f));
	}

	private void flash (Color flashColor)
	{
		flashImage.SetActive (true);
		Image image = flashImage.GetComponent<Image> ();
		image.color = flashColor;
		image.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height);
		InvokeRepeating ("doFlash", 0, 0.0001f);
	}

	private void doFlash ()
	{
		Image image = flashImage.GetComponent<Image> ();
		Color temp = image.color;
		if (temp.a > 0) {
			image.color = new Color (temp.r, temp.g, temp.b, temp.a - 0.005f);
		} else {
			flashImage.SetActive (false);
			CancelInvoke ();
		}
	}

	public void pause ()
	{
		clock.stop ();
		inGame = false;
		pauseObject.SetActive (false);
		resumeObject.SetActive (true);
	}

	public void resume ()
	{
		clock.resume ();
		inGame = true;
		pauseObject.SetActive (true);
		resumeObject.SetActive (false);
	}
		
	public void ExitGame()
	{
		if (NetworkServer.active)
		{
			NetworkManager.singleton.StopServer();
		}
		if (NetworkClient.active)
		{
			NetworkManager.singleton.StopClient();
		}
	}
}
