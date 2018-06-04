using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeFarController : MonoBehaviour {

	private List<FadeWhenFar> toRemove = new List<FadeWhenFar>();
	[SerializeField] private Transform gameCam;

	public static Transform cam;

	// Use this for initialization
	void Start () {
		cam = gameCam;
	}
	
	// Update is called once per frame
	void Update () {
		
		foreach(FadeWhenFar g in FadeWhenFar.inactive) {
			float dist = Vector3.Distance(g.transform.position, cam.position);
			if(dist < g.distance) {
				toRemove.Add(g);
			}
		}
		foreach(FadeWhenFar g in toRemove) {
			g.gameObject.SetActive(true);
			FadeWhenFar.inactive.Remove(g);
		}
		toRemove.Clear();
	}
}
