using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

	private const string PLAYER_TAG = "Player";

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	private PlayerWeapon currentWeapon;
	private WeaponManager weaponManager;

	void Start(){
		if (cam == null) {
			Debug.LogError ("No Camera Referenced (PlayerShoot.cs)");
			this.enabled = false;
		}

		weaponManager = GetComponent<WeaponManager> ();
	}

	void Update(){
		currentWeapon = weaponManager.GetCurrentWeapon ();

		if (currentWeapon.fireRate <= 0f) {
			if (Input.GetButtonDown ("Fire1")) {
				Shoot ();
			}
		} else {
			if (Input.GetButtonDown ("Fire1")) {
				InvokeRepeating ("Shoot", 0f, 1f / currentWeapon.fireRate);
			} else if (Input.GetButtonUp ("Fire1")) {
				CancelInvoke ("Shoot");
			}
		}

	}

	// Is called on the server when a player shoots
	[Command]
	void CmdOnShoot(){
		RpcDoShootEffect ();
	}

	// Is called on all clients when we need to do a shoot effect
	[ClientRpc]
	void RpcDoShootEffect(){
		weaponManager.GetCurrentWeaponGraphics().muzzleFlash.Play ();
	}

	// Is called on the server when we hit something
	// Takes in the hit point and the normal of the surface that was hit
	[Command]
	void CmdOnHit(Vector3 hitPosition, Vector3 surfaceNormal){
		RpcDoHitEffect (hitPosition, surfaceNormal);
	}

	// Is called on all clients, here we can spawn in hit effects
	[ClientRpc]
	void RpcDoHitEffect(Vector3 hitPosition, Vector3 surfaceNormal){
		GameObject hitEffect = (GameObject) Instantiate (weaponManager.GetCurrentWeaponGraphics ().hitEffectPrefab, hitPosition, Quaternion.LookRotation (surfaceNormal));
		Destroy (hitEffect, 2f);
	}

	[Client]
	private void Shoot(){

		if (!isLocalPlayer) {
			return;
		}

		// We are shooting, call the on shoot method on the server
		CmdOnShoot ();

		RaycastHit hit;
		if (Physics.Raycast (cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask)) {
			if (hit.collider.tag == PLAYER_TAG) {
				CmdPlayerShot (hit.collider.name, currentWeapon.damage);
			}

			CmdOnHit (hit.point, hit.normal); // We hit something, call on hit method on the server
		}
	}

	[Command]
	void CmdPlayerShot(string playerID, int damage){
		Debug.Log(playerID + " has been shot!");

		Player player = GameManager.GetPlayer (playerID);
		player.RpcTakeDamage (damage);
	}
}
