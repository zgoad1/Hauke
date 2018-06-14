using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour {

	private Animator anim;
	private Text text;
	private Image face;
	private string[] lines;
	private int index = 0;
	private int letters = 0;
	private bool typing = false;

	private Color faceColor;

	// Use this for initialization
	void Start () {
		Debug.Log("Start()");
		anim = GetComponent<Animator>();
		text = GameObject.Find("DboxText").GetComponent<Text>();
		face = GetComponentInChildren<Image>();
		enabled = false;

		faceColor = face.color;

		// test
		string[] cat = { "This is a line of dialogue.", "This one in particular is\ntwo lines of dialogue." };
		ShowDialogue(cat, new int[1], new int[1]);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1")) {
			AdvanceDialogue();
		}
	}

	public void ShowDialogue(string[] lines, int[] chars, int[] faces) {
		Debug.Log("ShowDialogue()");
		// Start looking for input in Update()
		enabled = true;
		// Animate in
		anim.SetBool("active", true);
		// Set variables
		this.lines = lines;

		// Start showing text
		StartCoroutine("WaitForShowText");
	}

	private void AdvanceDialogue() {
		Debug.Log("AdvanceDialogue()");
		Debug.Log("index: " + index);
		// If the dialogue is still typing out, advance to the end
		if(typing) {
			letters = lines[index].Length;      // maybe -1 depending on how C# string methods work
			text.text = lines[index];
			StopCoroutine("ShowText");
			typing = false;
		// Else show the next page of dialogue
		} else {
			// Increment dialogue page index
			index++;
			Debug.Log("incrementing index: " + (index - 1) + " -> " + index);
			// Check if we've passed the end
			if(index >= lines.Length) {
				anim.SetBool("active", false);
				Reset();
				return;
			}
			// Go to the next character and face

			// TODO: that

			// Start showing the dialogue
			StartCoroutine("ShowText");
		}
	}

	private void Reset() {
		anim = GetComponent<Animator>();
		text = GameObject.Find("DboxText").GetComponent<Text>();
		face = GetComponentInChildren<Image>();
		Debug.Log("Reset()");
		index = 0;
		letters = 0;
		text.text = "";
		enabled = false;
	}

	private IEnumerator ShowText() {
		Debug.Log("ShowText()");
		Debug.Log("Line length: " + lines[index].Length);
		typing = true;
		// Reset number of characters to show
		for(letters = 0; letters < lines[index].Length; letters++) {
			// Show the next character
			letters++;
			text.text = lines[index].Substring(0, letters);	// Can't let letters reach lines[index].length or chaos ensues. Another Unity bug? Probably.
			Debug.Log("Typed letter #" + letters);
			// Wait 2 frames
			yield return null;
			yield return null;
		}
		text.text = lines[index];
		typing = false;
	}

	private IEnumerator WaitForShowText() {
		// Wait 8 frames
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		StartCoroutine("ShowText");
	}
}
