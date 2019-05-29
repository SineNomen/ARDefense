using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Sojourn.ARDefense.Components {
	public class Cannon : MonoBehaviour {
		[SerializeField]
		private int _ammo = 100;
		[SerializeField]
		private Transform _barrelTransform = null;

		public int Ammo { get => _ammo; set => _ammo = value; }

		public bool IsReadyToFire { get { return !IsFiring && IsLoaded && Ammo != 0; } }
		public bool IsFiring { get; private set; }
		//how long to wait until we are ready to go again
		public bool IsLoaded { get { return TimeUntilLoaded <= Time.time; } }
		//how long to wait until we are ready to go again
		public float TimeUntilLoaded { get { return _lastFireTime + Weapon.ReloadTime; } }
		//how long to wait until we are ready to go again
		public float ReloadProgress {
			get {
				return Mathf.Clamp01(((Time.time - _lastFireTime) / Weapon.ReloadTime));
			}
		}
		public Weapon Weapon { get; set; }

		public WeaponCallback OnFireStart { get; set; }
		public WeaponCallback OnFireEnd { get; set; }
		public WeaponCallback OnEmptyFire { get; set; }
		public WeaponCallback OnOutOfAmmo { get; set; }
		public WeaponCallback OnReload { get; set; }

		[AutoInject]
		private IGameManager _gameManager;

		private float _lastFireTime = 0.0f;
		public IKillable Killable { get; set; }

		private void Start() {
			Container.AutoInject(this);
			if (Killable == null) {
				Killable = GetComponent<IKillable>();
			}
		}

		public IEnumerator Fire() {
			if (!IsLoaded) {
				Debug.LogFormat("Cannot fire {0}, not loaded, Time: {1}, TimeUntilLoaded: {2} ", Weapon, Time.time, TimeUntilLoaded);
				if (OnEmptyFire != null) { OnEmptyFire(Weapon); }
				yield break;
			}
			if (_ammo == 0) {
				Debug.LogFormat("Cannot fire {0}, No ammo", Weapon);
				if (OnEmptyFire != null) { OnEmptyFire(Weapon); }
				yield break;
			}

			Debug.Log("Fire");
			IsFiring = true;
			for (int i = 0; i < Weapon.ProjectilesPerShot; i++) {
				Vector3 offset = Vector3.zero;
				if (Weapon.ProjectileOffsets.Length <= i) {
					offset = _barrelTransform.position;
				} else {
					offset = _barrelTransform.TransformPoint(Weapon.ProjectileOffsets[i]);
				}
				GameObject go = Instantiate(Weapon.ProjectilePrefab, offset, _barrelTransform.rotation, null);
				IProjectile projectile = go.GetComponent<IProjectile>();
				IKillable killable = go.GetComponent<IKillable>();
				projectile.Weapon = Weapon;
				killable.Team = Killable.Team;

				projectile.OnFire();
				_ammo--;
				if (Weapon.DelayBetweenProjectiles > 0.0f) {
					yield return new WaitForSeconds(Weapon.DelayBetweenProjectiles);
				}
				projectile.Body.velocity = projectile.Transform.forward * Weapon.Speed;
			}
			_lastFireTime = Time.time;
			yield return null;
			IsFiring = false;
		}
	}
}