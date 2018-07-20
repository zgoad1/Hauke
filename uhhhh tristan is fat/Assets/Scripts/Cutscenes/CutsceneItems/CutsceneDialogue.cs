using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDialogue : EventItem {

	protected CutsceneDbox dbox;
	[SerializeField] protected DialogueArray[] dialogue;

	protected override void Reset() {
		dbox = FindObjectOfType<CutsceneDbox>();
	}
	
	protected override void Start () {
		Reset();
		dbox.ShowDialogue(dialogue[0].items);//text, dialogue[0].faces);
		Destroy(gameObject);
	}
}
