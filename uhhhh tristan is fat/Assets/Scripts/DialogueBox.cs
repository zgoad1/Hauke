using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour {

	private Animator anim;
	private Text text;
	private Image face;
	private string[] lines;
	private Sprite[] faces;
	private int index = 0;
	private int letters = 0;
	private bool typing = false;

	private Color faceColor;

	// Use this for initialization
	void Start () {
		Debug.Log("Start()");
		anim = GetComponent<Animator>();
		text = GameObject.Find("DboxText").GetComponent<Text>();
		foreach(Image i in GetComponentsInChildren<Image>()) {
			if(i.name == "DboxFace") {
				face = i;
			}
		}
		enabled = false;

		faceColor = face.color;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1")) {
			AdvanceDialogue();
		}
	}

	public void ShowDialogue(string[] lines, Sprite[] faces) {
		Debug.Log("ShowDialogue()");
		// Start looking for input in Update()
		enabled = true;
		// Animate in
		anim.SetBool("active", true);
		// Set variables
		this.lines = lines;
		this.faces = faces;

		// Set initial face image
		face.sprite = faces[0];

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
			// Go to the next face
			face.sprite = faces[index];

			// Start showing the dialogue
			StartCoroutine("ShowText");
		}
	}

	private void Reset() {
		anim = GetComponent<Animator>();
		text = GameObject.Find("DboxText").GetComponent<Text>();
		foreach(Image i in GetComponentsInChildren<Image>()) {
			if(i.name == "DboxFace") {
				face = i;
			}
		}
		Debug.Log("Reset()");
		index = 0;
		letters = 0;
		text.text = "";
		enabled = false;
	}

	private IEnumerator ShowText() {
		// Manually insert line breaks
		if(!lines[index].Contains("\n")) {
			int maxChars = 27;  // Number of characters before we assume end of line
			int startPos = 0;
			while(startPos < lines[index].Length && lines[index].Substring(startPos).Length > maxChars) {
				for(int i = startPos + maxChars; i > startPos; i--) {
					if(lines[index][i] == ' ') {
						Debug.Log("Found a space");
						lines[index] = lines[index].Insert(i + 1, "\n");
						startPos = i + 3;
					}
				}
			}
		}

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
