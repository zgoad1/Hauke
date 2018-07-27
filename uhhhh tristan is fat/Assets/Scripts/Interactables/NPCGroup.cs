using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCGroup : NPC {

	[SerializeField] protected NPCGroup[] group;

	protected override void Start() {
		base.Start();
	}

	public override void Interact() {
		// if this dialogue conversation is empty, check if anyone else in the group has been spoken to and get the next conversation from them
		foreach(NPCGroup n in group) if(n.ttt > ttt) ttt = n.ttt;
		if(dialogue.Length == 0 || dialogue[ttt].items.Length == 0) {
			if(dialogue.Length == 0) dialogue = Array.Find(group, n => n.dialogue.Length > 0).dialogue;	// If dialogue is empty, just copy it from another NPC in the group
			else dialogue[ttt].items = Array.Find(group, n => n.dialogue[ttt].items.Length > 0).dialogue[ttt].items;	// I enjoy C#
		}
		Debug.Log("Dialogue.Length: " + dialogue.Length + "\nttt: " + ttt);
		base.Interact();
	}
}
