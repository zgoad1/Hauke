using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	protected Animator anim;
	protected Controllable player;

	public float closeDistance = 20f;

	protected virtual void Reset() {
		anim = GetComponent<Animator>();
		player = FindObjectOfType<Controllable>();
	}

	protected virtual void Start() {
		Reset();
	}

	public void Open() {
		if(!anim.GetBool("open")) {
			anim.SetTrigger("opening");
			anim.SetBool("open", true);
			player.AddDoor(this);
		}
	}

	public void SetDistance(float value) {
		anim.SetFloat("playerDistance", value);
	}

	public void AnimCloseFinish() {
		Debug.LogWarning("Finished door animation");
		anim.SetBool("open", false);
		player.RemoveDoor(this);
	}

	public bool IsOpen() {
		return anim.GetBool("open");
	}
}
