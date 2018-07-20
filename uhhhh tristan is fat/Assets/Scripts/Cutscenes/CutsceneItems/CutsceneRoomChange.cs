using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneRoomChange : EventItem {

	[SerializeField] private string newScene;
	[SerializeField] private bool whiteFade = false;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		if(whiteFade) {
			FindObjectOfType<RoomManager>().MakeWhite();
		} else {
			FindObjectOfType<RoomManager>().MakeBlack();
		}
		FindObjectOfType<RoomManager>().RoomChange(newScene);
		Destroy(gameObject); 
	}
}
