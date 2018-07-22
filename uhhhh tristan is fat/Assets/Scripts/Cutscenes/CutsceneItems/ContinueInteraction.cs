using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueInteraction : EventItem {
	protected override void Start() {
		base.Start();
		FindObjectOfType<Controllable>().Interact();
	}
}
