using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed = 5f;
	[SerializeField]
	private float lookSensitivity = 10f;
	[SerializeField]
	private float thrusterForce = 1000f;

	[Header("Spring Settings:")]
	[SerializeField]
	private float jointSpring = 20f;
	[SerializeField]
	private float jointMaxForce = 40f;

	// Component caching
	private Animator anim;
	private PlayerMotor motor;
	private ConfigurableJoint joint;

	void Start(){
		anim = GetComponent<Animator> ();
		motor = GetComponent<PlayerMotor> ();
		joint = GetComponent<ConfigurableJoint> ();
		SetJointSettings (jointSpring);
	}

	void Update(){
		// calculate our movement velocity as a Vector3
		float xMovement = Input.GetAxis("Horizontal");
		float zMovement = Input.GetAxis("Vertical");

		Vector3 moveHorizontal = transform.right * xMovement;
		Vector3 moveVertical = transform.forward * zMovement;

		// final movement vector
		Vector3 velocity = (moveVertical + moveHorizontal) * speed;

		// animate movement
		anim.SetFloat("ForwardVelocity", zMovement);

		// apply movement
		motor.Move(velocity);

		// calculate rotation
		float yRotation = Input.GetAxisRaw("Mouse X");
		Vector3 rotation = new Vector3 (0f, yRotation, 0f) * lookSensitivity;

		// apply rotation to 3D object
		motor.Rotate(rotation);

		// calculate camera rotation
		float xRotation = Input.GetAxisRaw("Mouse Y");
		float cameraRotationX = xRotation * lookSensitivity;

		// apply rotation to camera
		motor.RotateCamera(cameraRotationX);

		Vector3 localThrusterForce = Vector3.zero;

		if (Input.GetButton ("Jump")) {
			localThrusterForce = Vector3.up * thrusterForce;
			SetJointSettings (0f);
		} else {
			SetJointSettings (jointSpring);
		}

		// apply thruster force
		motor.ApplyThruster(localThrusterForce);
	}

	private void SetJointSettings(float _jointSpring){
		joint.yDrive = new JointDrive { 
			positionSpring = _jointSpring, 
			maximumForce = jointMaxForce 
		};
	}

}
