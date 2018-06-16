using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneRoomChange : EventItem {

	[SerializeField] private string newScene;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		FindObjectOfType<RoomManager>().RoomChange(newScene);
		Destroy(gameObject); 
	}
}
