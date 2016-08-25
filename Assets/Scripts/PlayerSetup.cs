using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	private string remoteLayerName = "RemotePlayer";
	[SerializeField]
	private string dontDrawLayerName = "DontDraw";

	[SerializeField]
	private GameObject playerGraphics;
	[SerializeField]
	private GameObject playerUIPrefab;
	private GameObject playerUIInstance;
	private Camera sceneCamera;

	void Start(){
		if (!isLocalPlayer) {
			DisableComponents ();
			AssignRemoteLayer ();
		} else {
			sceneCamera = Camera.main;
			if(sceneCamera)
				sceneCamera.gameObject.SetActive (false);
			// disable player graphics
			Util.SetLayerRecursively (playerGraphics, LayerMask.NameToLayer (dontDrawLayerName));

			// create player UI
			playerUIInstance = (GameObject) Instantiate(playerUIPrefab);
			playerUIInstance.name = playerUIPrefab.name;
		}

		GetComponent<Player> ().Setup ();
	}

	public override void OnStartClient(){
		base.OnStartClient ();

		string netID = GetComponent<NetworkIdentity> ().netId.ToString();
		Player player = GetComponent<Player> ();

		GameManager.RegisterPlayer (netID, player);
	}

	void RegisterPlayer(){
		string ID = "Player " + GetComponent<NetworkIdentity> ().netId;
		gameObject.name = ID;
	}
		
	void DisableComponents(){
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false;
		}
	}

	void AssignRemoteLayer(){
		gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
	}

	void OnDisable(){
		Destroy (playerUIInstance);

		if (sceneCamera)
			sceneCamera.gameObject.SetActive (true);

		GameManager.UnRegisterPlayer (transform.name);
	}

}
