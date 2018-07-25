using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCGroup : NPC {

	protected NPC[] group;

	protected override void Start() {
		base.Start();
		group = GetComponentsInChildren<NPC>();
	}

	public override void Interact() {
		int[] ttts = new int[group.Length];
		for(int i = 0; i < group.Length; i++) {
			ttts[i] = group[i].ttt;
		}
		ttt = Mathf.Max(ttts);
		if(dialogue.Length == 0) {
			dialogue = Array.Find(group, n => n.dialogue.Length > 0).dialogue;	// I enjoy C#
		}
		base.Interact();
	}
}
