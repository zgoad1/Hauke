using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour {

	private Image blackFade;
	[SerializeField] private int fadeFrames = 90;
	private Color newColor = new Color(0, 0, 0, 0);
	private EventQueue eq;
	private Animator fadeAnim;
	private string nextScene;
	
	void Start () {
		/*
		try {
			blackFade = GameObject.Find("BlackFade").GetComponent<Image>();
		} catch(System.NullReferenceException e) {
			blackFade = null;
			Debug.LogError("Ohhh! That makes me sad!\n" + e);
		}
		*/
		blackFade = GetComponent<Image>();
		eq = FindObjectOfType<EventQueue>();
		/*
		try {
			fadeAnim = GameObject.Find("BlackFade").GetComponent<Animator>();
		} catch(System.NullReferenceException e) {
			fadeAnim = null;
			Debug.LogError("Ohhh! That makes me sad!\n" + e);
		}
		*/
		fadeAnim = GetComponent<Animator>();
	}

	public void RoomChange(string newScene) {
		nextScene = newScene;
		fadeAnim.SetTrigger("active");
		/*
		IEnumerator cr = ChangeScene(newScene);
		StartCoroutine(cr);
		*/
	}

	public void OnAnimationFinish() {
		eq.WaitFrames(fadeFrames);
		SceneManager.LoadScene(nextScene);
	}

	/*
	private IEnumerator ChangeScene(string newScene) {
		fadeAnim.SetTrigger("active");
		for(int i = 0; i < fadeFrames; i++) {
			newColor.a = 0.5f + 0.5f * (Mathf.Cos(2f * Mathf.PI / (2f * fadeFrames) * i));
			//blackFade.material.color = newColor;
			blackFade.material.color = Color.red;
			Debug.Log("alpha: " + newColor.a);
			yield return null;
		}
		Debug.Log("Faded out");
		SceneManager.LoadScene(newScene);
		yield return null;
		Debug.Log("Loading new scene");
		blackFade = GameObject.Find("BlackFade").GetComponent<Image>();
		Debug.Log("New Blackfade: " + blackFade);
		for(int i = fadeFrames; i < 2 * fadeFrames; i++) {
			newColor.a = 1 - 0.5f * (Mathf.Cos(2f * Mathf.PI / (2f * fadeFrames) * i));
			blackFade.material.color = newColor;
			yield return null;
		}
		newColor.a = 0;
		blackFade.material.color = newColor;
		Debug.Log("Faded in");
		if(eq != null) eq.Dequeue();
	}
	*/
}
