using UnityEngine;

public class CameraControl : MonoBehaviour {

	public Transform lookAt;
	private Transform desTransform;      // desired position
	public Transform adjTransform;      // adjusted position
	public Transform camTransform;      // camera position (lerps towards adjusted position)
	public float rad = 0.5f;            // distance from solid to stop at
	public float lerpFac;
	public float iLerpFac = 0.2f;
	public float camSensitivity = 1f;
	public float idistance = 14;
	public float distance;
	public bool firstPerson = false;

	private float currentX = 0;
	private float currentY = 0;
	private float sensitivityX = 4;
	private float sensitivityY = 2;
	private Vector3 lookOffset = new Vector3(0f, 4f, 0f);
	private int inverted = 1;
	private int raymask;

	// Use this for initialization
	void Start() {
		desTransform = transform;
		lerpFac = iLerpFac;
		sensitivityX *= camSensitivity;
		sensitivityY *= camSensitivity;
		// layers to raycast upon
		raymask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("IgnoreCollision") | 1 << LayerMask.NameToLayer("Ground");
	}

	// Update is called once per frame
	void Update() {
		currentX += sensitivityX * Input.GetAxis("Mouse X");
		currentY = Mathf.Clamp(currentY - sensitivityY * inverted * Input.GetAxis("Mouse Y"), -60f, 75f);

		// invert y axis
		if(Input.GetButtonDown("Invert")) {
			inverted *= -1;
		}
	}

	void LateUpdate() {
		if(!firstPerson) {
			// keep the camera from going through solid colliders
			RaycastHit hit;
			Vector3 rayDir = Vector3.zero;
			rayDir = desTransform.position - (lookAt.position + lookOffset);
			if(Physics.Raycast(lookAt.position + lookOffset, rayDir, out hit, idistance, raymask)) {
				Vector3 newPos = hit.point + hit.normal * rad;
				distance = Mathf.Min(idistance, Vector3.Distance(lookAt.position + lookOffset, newPos));
				setCam(distance);
				adjTransform.position = newPos;
			} else {
				distance = idistance;
				setCam(distance);
			}
		} else {
			setCam(idistance);
		}
		camTransform.position = Vector3.Lerp(camTransform.position, adjTransform.position, lerpFac);
		if(!firstPerson) {
			camTransform.LookAt(lookAt.position + lookOffset);
		}
	}

	void setCam(float distance) {
		Vector3 dir = new Vector3(0, 0, -distance);
		Quaternion rot = Quaternion.Euler(currentY, currentX, 0);

		// move adjusted camera position
		adjTransform.position = (lookAt.position + lookOffset) + rot * dir;

		// move desired camera position
		Vector3 dDir = new Vector3(0, 0, -idistance);
		desTransform.position = (lookAt.position + lookOffset) + rot * dDir;

		// first-person camera rotation
		if(firstPerson) camTransform.rotation = Quaternion.Lerp(camTransform.rotation, rot, 0.4f);

	}

	// meant to be called for several frames in a row as "idistance" is lerped towards 0
	public void SetFirstPerson(bool value) {
		if(value) {
			firstPerson = true;
			lerpFac = Mathf.Lerp(lerpFac, 1, 0.05f);
		} else {
			firstPerson = false;
			lerpFac = iLerpFac;
		}
	}
}
