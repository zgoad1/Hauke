﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePausePlayer : EventItem {

	// Use this for initialization
	protected override void Start () {
		base.Start();
		FindObjectOfType<Controllable>().Pause();
		eq.Dequeue();
		Destroy(gameObject);
	}
}