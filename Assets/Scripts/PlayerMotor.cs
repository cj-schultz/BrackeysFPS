using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

	[SerializeField]
	Camera cam;

	private Vector3 velocity;
	private Vector3 rotation;
	private float cameraRotationX;
	private float currentCameraRotationX;
	private Vector3 thrusterForce;

	[SerializeField]
	private float cameraRotationLimit = 85f;

	private Rigidbody rb;

	void Start(){
		rb = GetComponent<Rigidbody> ();
	}

	public void Move(Vector3 _velocity){
		velocity = _velocity;
	}

	public void Rotate(Vector3 _rotation){
		rotation = _rotation;
	}

	public void RotateCamera(float _cameraRotationX){
		cameraRotationX = _cameraRotationX;
	}

	public void ApplyThruster(Vector3 _thrusterForce){
		thrusterForce = _thrusterForce;
	}

	void FixedUpdate(){
		PerformRotation ();
		PerformMovement ();
	}

	private void PerformRotation(){
		// rotation on the rb
		rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
		// rotation on the camera
		if (cam != null) {
			currentCameraRotationX -= cameraRotationX;
			currentCameraRotationX = Mathf.Clamp (currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
			cam.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0f, 0f);
		}
	}

	private void PerformMovement(){
		if (velocity != Vector3.zero) {
			rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
		}

		if (thrusterForce != Vector3.zero) {
			rb.AddForce (thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
		}
	}
}
