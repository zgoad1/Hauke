using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour {

	private static DialogueBox regDbox;
	private static CutsceneDbox csDbox;
	private CameraControl cam;

	public Animator anim;
	protected Text text;
	protected Image face;
	protected FaceText[] items;
	protected int index = 0;
	protected int letters = 0;
	protected bool typing = false;

	protected Color faceColor;
	[SerializeField] protected int maxCharsNoFace = 1000;//44;
	[SerializeField] protected int maxCharsFace = 1000;//27;
	[SerializeField] protected int textXNoFace = -492;
	[SerializeField] protected int textXFace = -76;
	protected int maxChars;  // Number of characters before we assume end of line
	protected Vector3 textPosNoFace;
	protected Vector3 textPosFace;
	protected AudioManager am = AudioManager.instance;
	protected Controllable player;
	[HideInInspector] public List<NPCHead> heads;
	[HideInInspector] public List<NPC> bodies;

	// Use this for initialization
	protected virtual void Start () {
		/*
		anim = GetComponent<Animator>();
		text = GameObject.Find("DboxText").GetComponent<Text>();
		foreach(Image i in GetComponentsInChildren<Image>()) {
			if(i.name == "DboxFace") {
				face = i;
			}
		}
		enabled = false;
		*/
		Reset();
		if(am == null) am = AudioManager.instance;	// NOTE: SINGLETON OBJECTS THAT PERSIST BETWEEN ROOMS AND HAVE INSTANCES IN EACH ROOM CANNOT BE SET IN RESET()
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

	// Sets dialogue and activates all necessary items
	public virtual void ShowDialogue(FaceText[] items) {//string[] lines, Sprite[] faces) {
		// Start looking for input in Update()
		enabled = true;
		// Animate in
		anim.SetBool("active", true);
		// Set variables
		this.items = items;

		// Set initial face image
		SetFace(items[0].face);

		// Pause the player
		if(player != null) player.Pause();

		// Start showing text
		if(!CheckSwitch(index)) {	// ...if we're not supposed to switch dbox styles
			StartCoroutine("WaitForShowText");
		}
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

	// Button pressed to show next dialogue
	protected void AdvanceDialogue() {
		// If the dialogue is still typing out, advance to the end
		if(typing) {
			letters = items[index].text.Length;
			text.text = items[index].text;
			StopCoroutine("ShowText");
			typing = false;
			// Else show the next page of dialogue
		} else {
			// Increment dialogue page index
			index++;
			// Check if we've passed the end
			if(index >= items.Length) {
				Finish();
				return;
			}

			if(!CheckSwitch(index)) {
				// Go to the next face
				SetFace(items[index].face);

				// Start showing the dialogue
				StartCoroutine("ShowText");
			}
		}
	}

	// Switching dbox style
	protected static bool CheckSwitch(int index) {
		if(regDbox == null || csDbox == null) return false;	// If we only have one dbox, stop
		// If the next item is supposed to use the other dbox, switch to that one
		// switching regular to cutscene dbox
		if(regDbox.anim.GetBool("active") == true && regDbox.items[index].useCutsceneDbox) {
			regDbox.anim.SetBool("active", false);
			// Call the csDbox's ShowDialogue with an array of the remaining items
			FaceText[] newItems = new FaceText[regDbox.items.Length - regDbox.index];
			for(int i = regDbox.index; i < regDbox.items.Length; i++) {
				newItems[i - regDbox.index] = regDbox.items[i];
			}
			foreach(NPCHead h in regDbox.heads) {
				csDbox.heads.Add(h);
			}
			foreach(NPC n in regDbox.bodies) {
				csDbox.bodies.Add(n);
			}
			regDbox.Reset();
			csDbox.ShowDialogue(newItems);

			return true;

			// switching cutscene to regular dbox
		} else if(csDbox.anim.GetBool("active") == true && !csDbox.items[index].useCutsceneDbox) {
			csDbox.anim.SetBool("active", false);
			// Call the regDbox's ShowDialogue with an array of the remaining items
			FaceText[] newItems = new FaceText[csDbox.items.Length - csDbox.index];
			for(int i = csDbox.index; i < csDbox.items.Length; i++) {
				newItems[i - csDbox.index] = csDbox.items[i];
			}
			foreach(NPCHead h in csDbox.heads) {
				regDbox.heads.Add(h);
			}
			foreach(NPC n in csDbox.bodies) {
				regDbox.bodies.Add(n);
			}
			csDbox.Reset();
			regDbox.ShowDialogue(newItems);

			return true;
		}
		return false;
	}

	protected virtual void Finish() {
		anim.SetBool("active", false);  // exit animation
		// Turn back heads and bodies of player and NPCs
		foreach(NPC n in bodies) {
			n.TurnBack();
		}
		foreach(NPCHead h in heads) {
			h.FaceTransform(null);
		}
		// Unpause player
		if(player != null) player.Unpause();
		if(cam != null) cam.ZoomOut();
		Reset();
	}

	protected void Reset() {
		regDbox = GameObject.Find("Dbox") != null ? GameObject.Find("Dbox").GetComponent<DialogueBox>() : null;
		csDbox = GameObject.Find("Cutscene Dbox") != null ? GameObject.Find("Cutscene Dbox").GetComponent<CutsceneDbox>() : null;
		cam = FindObjectOfType<CameraControl>();
		anim = GetComponent<Animator>();
		if(gameObject.name == "Dbox") text = GameObject.Find("DboxText").GetComponent<Text>();
		else text = GameObject.Find("CSDboxText").GetComponent<Text>();
		player = FindObjectOfType<Controllable>();
		foreach(Image i in GetComponentsInChildren<Image>()) {
			if(i.name == "DboxFace" || i.name == "CSDboxFace") {
				//Debug.Log("Child name: " + i.name + "\nMy name: " + gameObject.name);
				face = i;
			}
		}
		index = 0;
		letters = 0;
		text.text = "";
		enabled = false;
		heads.Clear();
		bodies.Clear();
	}

	protected IEnumerator ShowText() {
		// Manually insert line breaks
		if(!items[index].text.Contains("\n")) {
			int startPos = 0;
			while(startPos < items[index].text.Length && items[index].text.Substring(startPos).Length > maxChars) {
				for(int i = startPos + maxChars; i > startPos; i--) {
					if(items[index].text[i] == ' ') {
						items[index].text = items[index].text.Insert(i + 1, "\n");
						startPos = i + 3;
					}
				}
			}
		}
		
		typing = true;
		// Reset number of characters to show
		for(letters = 0; letters < items[index].text.Length; letters++) {
			// Show the next character
			text.text = items[index].text.Substring(0, letters); // Can't let letters reach items[index].text.length or chaos ensues. Another Unity bug? Probably.
			if(items[index].text[Mathf.Min(items[index].text.Length - 1, letters)] != ' ' && letters % 3 == 0) {
				am.Play("textbeep" + (letters % 2));    // use alternating text beep sounds because Unity assumes you want to make a weird pop sound if you call Play() on a sound before it's finished playing
			}

			yield return null;
		}
		text.text = items[index].text;
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
