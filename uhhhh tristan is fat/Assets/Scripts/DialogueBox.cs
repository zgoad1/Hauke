using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour {

	protected Animator anim;
	protected Text text;
	protected Image face;
	protected string[] lines;
	protected Sprite[] faces;
	protected int index = 0;
	protected int letters = 0;
	protected bool typing = false;

	protected Color faceColor;
	[SerializeField] protected int maxCharsNoFace = 44;
	[SerializeField] protected int maxCharsFace = 27;
	[SerializeField] protected int textXNoFace = -492;
	[SerializeField] protected int textXFace = -76;
	protected int maxChars;  // Number of characters before we assume end of line
	protected Vector3 textPosNoFace;
	protected Vector3 textPosFace;

	// Use this for initialization
	protected virtual void Start () {
		anim = GetComponent<Animator>();
		text = GameObject.Find("DboxText").GetComponent<Text>();
		foreach(Image i in GetComponentsInChildren<Image>()) {
			if(i.name == "DboxFace") {
				face = i;
			}
		}
		enabled = false;

		faceColor = face.color;
		maxChars = maxCharsNoFace;
		textPosNoFace = new Vector3(textXNoFace, 76, 0);
		textPosFace = new Vector3(textXFace, 76, 0);
	}
	
	// Update is called once per frame
	protected void Update () {
		if(Input.GetButtonDown("Fire1")) {
			AdvanceDialogue();
		}
	}

	public void ShowDialogue(string[] lines, Sprite[] faces) {
		// Start looking for input in Update()
		enabled = true;
		// Animate in
		anim.SetBool("active", true);
		// Set variables
		this.lines = lines;
		this.faces = faces;

		// Set initial face image
		SetFace(faces[0]);

		// Start showing text
		StartCoroutine("WaitForShowText");
	}

	protected void SetFace(Sprite faceSprite) {
		face.sprite = faceSprite;
		if(faceSprite == null) {
			face.gameObject.SetActive(false);
			text.transform.localPosition = textPosNoFace;
			maxChars = maxCharsNoFace;
		} else {
			face.gameObject.SetActive(true);
			text.transform.localPosition = textPosFace;
			maxChars = maxCharsFace;
		}
	}

	protected void AdvanceDialogue() {
		// If the dialogue is still typing out, advance to the end
		if(typing) {
			letters = lines[index].Length;
			text.text = lines[index];
			StopCoroutine("ShowText");
			typing = false;
		// Else show the next page of dialogue
		} else {
			// Increment dialogue page index
			index++;
			// Check if we've passed the end
			if(index >= lines.Length) {
				Finish();
				return;
			}
			// Go to the next face
			SetFace(faces[index]);

			// Start showing the dialogue
			StartCoroutine("ShowText");
		}
	}

	protected virtual void Finish() {
		anim.SetBool("active", false);
		Reset();
	}

	protected void Reset() {
		anim = GetComponent<Animator>();
		text = GameObject.Find("DboxText").GetComponent<Text>();
		foreach(Image i in GetComponentsInChildren<Image>()) {
			if(i.name == "DboxFace") {
				face = i;
			}
		}
		index = 0;
		letters = 0;
		text.text = "";
		enabled = false;
	}

	protected IEnumerator ShowText() {
		// Manually insert line breaks
		if(!lines[index].Contains("\n")) {
			int startPos = 0;
			while(startPos < lines[index].Length && lines[index].Substring(startPos).Length > maxChars) {
				for(int i = startPos + maxChars; i > startPos; i--) {
					if(lines[index][i] == ' ') {
						lines[index] = lines[index].Insert(i + 1, "\n");
						startPos = i + 3;
					}
				}
			}
		}
		
		typing = true;
		// Reset number of characters to show
		for(letters = 0; letters < lines[index].Length; letters++) {
			// Show the next character
			letters++;
			text.text = lines[index].Substring(0, letters);	// Can't let letters reach lines[index].length or chaos ensues. Another Unity bug? Probably.
			// Wait 2 frames
			yield return null;
			yield return null;
		}
		text.text = lines[index];
		typing = false;
	}

	protected IEnumerator WaitForShowText() {
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
