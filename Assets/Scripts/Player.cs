using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

	[SyncVar]
	private bool _isDead = false;
	public bool isDead
	{
		get { return _isDead; }
		protected set{ _isDead = value; }
	}

	[SerializeField]
	private int maxHealth = 100;

	[SyncVar]
	private int currentHealth;

	[SerializeField]
	private Behaviour[] disableOnDeath;

	private bool[] wasEnabled;

	public void Setup(){
		wasEnabled = new bool[disableOnDeath.Length];
		for (int i = 0; i < wasEnabled.Length; i++) {
			wasEnabled [i] = disableOnDeath [i].enabled;
		}

		SetDefaults ();
	}

//	void Update(){
//		if (!isLocalPlayer)
//			return;
//
//		if (Input.GetKeyDown (KeyCode.K))
//			RpcTakeDamage (100);
//	}

	[ClientRpc]
	public void RpcTakeDamage(int damage){
		if (isDead)
			return;


		currentHealth -= damage;

		Debug.Log (gameObject.transform.name + " now has " + currentHealth + " health.");

		if (currentHealth <= 0)
			Die ();
	}

	private void Die(){
		isDead = true;

		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = false;
		}

		Collider collider = GetComponent<Collider> ();
		if(collider)
			collider.enabled = false;

		Debug.Log (transform.name + " is dead");

		StartCoroutine ("Respawn");
	}

	private IEnumerator Respawn(){
		yield return new WaitForSeconds (GameManager.instance.matchSettings.respawnTime);

		SetDefaults ();
		Transform spawnPoint = NetworkManager.singleton.GetStartPosition ();
		transform.position = spawnPoint.position;
		transform.rotation = spawnPoint.rotation;

		yield return null;
	}

	public void SetDefaults(){
		isDead = false;

		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = wasEnabled [i];
		}

		currentHealth = maxHealth;

		Collider collider = GetComponent<Collider> ();
		if(collider)
			collider.enabled = true;
	}

}
