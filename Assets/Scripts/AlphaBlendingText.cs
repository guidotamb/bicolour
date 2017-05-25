using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaBlendingText : MonoBehaviour {

	private Text text;
	private bool up = false;
	// Use this for initialization
	void Start () {
		text = gameObject.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.activeSelf) {
			Color temp = text.color;

			if (up && temp.a >= 0.5f) {
				up = false;
			}
			if (!up && temp.a <= 0.05f) {
				up = true;
			}

			if (!up) {
				text.color = new Color (temp.r, temp.g, temp.b, temp.a - 0.01f);
			} else {
				text.color = new Color (temp.r, temp.g, temp.b, temp.a + 0.01f);
			}

		}
	}
}
