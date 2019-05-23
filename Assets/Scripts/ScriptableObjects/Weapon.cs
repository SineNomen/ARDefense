using Sojourn.ARDefense.Components;
using Sojourn.ARDefense.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sojourn.ARDefense.ScriptableObjects {

	public delegate void WeaponCallback(Weapon weapon);

	[CreateAssetMenu(fileName = "Weapon", menuName = "ARDefense/Weapon", order = 0)]
	public class Weapon : ScriptableObject {
		[SerializeField]
		[Tooltip("The projectile that is fire")]
		private GameObject projectilePrefab = null;
		[SerializeField]
		[Tooltip("The display used for this weapon")]
		private GameObject displayPrefab = null;

		[SerializeField]
		[Tooltip("How much damage each projectile deals")]
		private int _damage = 1;
		[SerializeField]
		[Tooltip("How long it takes to fully reload once all shots are away")]
		private float _reloadTime = 1.0f;
		[SerializeField]
		[Tooltip("How many shots before we have to reload")]
		private int _shotPerLoad = 1;
		[SerializeField]
		[Tooltip("How many Projectiles fire per shot")]
		private int _projectilesPerShot = 1;
		[SerializeField]
		[Tooltip("How long to way between projectiles in a single shot (0 means they are all fired together)")]
		private float _delayBetweenProjectiles = 0.0f;
		[SerializeField]
		[Tooltip("How to space out the projectiles when they are fired")]
		private Vector3[] _projectileOffsets = null;
		[SerializeField]
		[Tooltip("How many projectiles can be fired")]
		//`Mat NOTE: This does not keep track of ammo, that is tied to the object using the weapon
		private int _maxAmmo = -1;
		[SerializeField]
		[Tooltip("How fast the projectiles move")]
		//`Mat NOTE: This does not keep track of ammo, that is tied to the object using the weapon
		private float _speed = -1;
		[SerializeField]
		[Tooltip("How long until each projectile is destroyed (if it hasn't already hit something)")]
		//`Mat NOTE: This does not keep track of ammo, that is tied to the object using the weapon
		private float _projectileLifetime = -10.0f;

		public GameObject ProjectilePrefab { get => projectilePrefab; }
		public GameObject DisplayPrefab { get => displayPrefab; }
		public int Damage { get => _damage; }
		public float ReloadTime { get => _reloadTime; }
		public float ShotPerLoad { get => _shotPerLoad; }
		public int ProjectilesPerShot { get => _projectilesPerShot; }
		public float DelayBetweenProjectiles { get => _delayBetweenProjectiles; }
		public Vector3[] ProjectileOffsets { get => _projectileOffsets; }
		public int MaxAmmo { get => _maxAmmo; }
		public float Speed { get => _speed; }
		public float ProjectileLifetime { get => _projectileLifetime; }
	}
}