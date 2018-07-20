using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour {

	private static Color color = Color.black;

	private Image blackFade;
	//[SerializeField] private int fadeFrames = 90;
	private EventQueue eq;
	private Animator fadeAnim;
	private string nextScene;
	
	void Start () {
		blackFade = GetComponent<Image>();
		eq = FindObjectOfType<EventQueue>();
		fadeAnim = GetComponent<Animator>();

		blackFade.color = color;
		color = Color.black;
	}

	public void MakeWhite() {
		color = Color.white;
		blackFade.color = new Color(color.r, color.g, color.b, blackFade.color.a);
	}

	public void MakeBlack() {
		color = Color.black;
		blackFade.color = new Color(color.r, color.g, color.b, blackFade.color.a);
	}

	public void RoomChange(string newScene) {
		nextScene = newScene;
		fadeAnim.SetTrigger("active");
	}

	public void OnAnimationFinish() {
		eq = FindObjectOfType<EventQueue>();
		eq.WaitFrames(60);//fadeFrames);
		SceneManager.LoadScene(nextScene);
	}
}
