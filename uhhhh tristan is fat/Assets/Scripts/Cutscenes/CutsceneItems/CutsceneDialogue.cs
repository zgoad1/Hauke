using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDialogue : EventItem {

	protected DialogueBox dbox;
	[SerializeField] protected DialogueArray[] dialogue;

	protected override void Reset() {
		dbox = FindObjectOfType<DialogueBox>();
	}
	
	protected override void Start () {
		Reset();
		dbox.ShowDialogue(dialogue[0].text, dialogue[0].faces);
		Destroy(gameObject);
	}
}
