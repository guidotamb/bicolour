using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CircleScript : NetworkBehaviour
{

	private Color one;
	private Color two;
	[SyncVar]
	private Color actualColor;

	private bool color = true;
	private bool swapped;
	private SpriteRenderer sr;
	public GameObject ciao;


	// Use this for initialization
	void Start ()
	{
		sr = gameObject.GetComponent<SpriteRenderer> ();
	}


	void Awake ()
	{
		sr = gameObject.GetComponent<SpriteRenderer> ();
		swapped = false;
	}

	[ServerCallback]
	void Update ()
	{
		sr.color = actualColor;
	}
		

	public void swapColor ()
	{
		color = !color;
		if (color) {
			sr.color = one;
			actualColor = one;
		} else {
			sr.color = two;
			actualColor = two;
		}
		prepareAndDoSplash ();
	}

	public void setOne (Color color)
	{
		this.one = color;
		actualColor = color;
		sr.color = color;
	}

	public void setTwo (Color two)
	{
		this.two = two;
	}

	public bool isSwapped ()
	{
		return swapped;
	}

	public void setSwapped (bool swapped)
	{
		this.swapped = swapped;
	}

	public Color getColor ()
	{
		return sr.color;
	}

	IEnumerator waitForSplash ()
	{
		yield return new WaitForSeconds (0.1f);
		prepareAndDoSplash ();
	}

	IEnumerator splashAndWait ()
	{
		prepareAndDoSplash ();
		yield return new WaitForSeconds (0.5f);
	}

	void prepareAndDoSplash ()
	{
		ciao.SetActive (true);
		ciao.transform.localScale = new Vector3 (0.2f, 0.2f, 1f);
		SpriteRenderer ciaoRenderer = ciao.GetComponent<SpriteRenderer> ();
		ciaoRenderer.color = new Color (0.9f, 0.9f, 0.9f, 0.66f);
		ciao.transform.rotation = new Quaternion (0, 0, 0, 1);
		InvokeRepeating ("doSplash", 0, 0.0001f);
	}

	void doSplash ()
	{
		float i = 0.1f;
		rotate (i);
		scale (i);
		alpha (i);
	}

	public void splashAndDestroy ()
	{
		StartCoroutine (splashAndWait ());
		Destroy (gameObject);
	}

	void rotate (float i)
	{
		Quaternion rot = ciao.transform.rotation;
		if (rot.z < 1) {
			ciao.transform.rotation = new Quaternion (rot.x, rot.y, rot.z + i, rot.w);
		} else {
			ciao.transform.rotation = new Quaternion (rot.x, rot.y, 0, rot.w);
		}
	}

	void scale (float i)
	{
		Vector3 scale = ciao.transform.localScale;
		if (ciao.transform.localScale.x <= 1f) {
			ciao.transform.localScale = new Vector3 (scale.x + i, scale.y + i, scale.z);
		}
	}

	void alpha (float i)
	{
		SpriteRenderer ciaoRenderer = ciao.GetComponent<SpriteRenderer> ();
		Color temp = ciaoRenderer.color;
		if (temp.a > 0) {
			ciaoRenderer.color = new Color (temp.r, temp.g, temp.b, temp.a - (i / 5));
		} else {
			CancelInvoke ();
			Debug.Log ("Cancel invoke");
			ciao.SetActive (false);
		}
	}

	public void destroy ()
	{
		Destroy (gameObject);
	}
}
