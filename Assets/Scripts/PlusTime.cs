using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlusTime : MonoBehaviour {

	private Text text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void doAddPoints(float time){
		text = gameObject.GetComponent<Text> ();
		text.text = "+" + time.ToString("F1") + "''";
		InvokeRepeating ("effect", 0, 0.001f);
	}

	void effect(){
		decrease ();
		alpha ();
	}

	void alpha ()
	{
		Color temp = text.color;
		if (temp.a > 0) {
			text.color = new Color (temp.r, temp.g, temp.b, temp.a - 0.003f);
		} else {
			Destroy (gameObject);
			CancelInvoke ();
		}
	}

	void decrease ()
	{
		if (text.fontSize < 55) {
			text.fontSize = text.fontSize + 1;
		}
	}
}
