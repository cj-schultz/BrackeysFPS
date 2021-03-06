﻿using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

	[SerializeField]
	private string weaponLayerName = "Weapon";

	[SerializeField]
	private Transform weaponHolder;

	[SerializeField]
	private PlayerWeapon primaryWeapon;

	private PlayerWeapon currentWeapon;
	private WeaponGraphics currentGraphics;

	void Start(){
		EquipWeapon (primaryWeapon);
	}

	public PlayerWeapon GetCurrentWeapon(){
		return currentWeapon;
	}

	public WeaponGraphics GetCurrentWeaponGraphics(){
		return currentGraphics;
	}

	void EquipWeapon(PlayerWeapon weapon){
		currentWeapon = weapon;

		GameObject weaponGO = (GameObject) Instantiate (weapon.graphics, weaponHolder.position, weaponHolder.rotation);
		weaponGO.transform.SetParent (weaponHolder);

		currentGraphics = weaponGO.GetComponent<WeaponGraphics> ();
		if (currentGraphics == null) {
			Debug.LogError ("No weapon graphics component on the weapon " + weaponGO.name);
		}

		if (isLocalPlayer) {
			Util.SetLayerRecursively (weaponGO, LayerMask.NameToLayer(weaponLayerName));
		}
			
	}

}
