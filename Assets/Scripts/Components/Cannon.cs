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
		[AutoInject]
		private IGameManager _gameManager;
		[AutoInject]
		private IPlayer _player = null;

		[SerializeField]
		private eKillableTeam _team = eKillableTeam.Player1;
		[SerializeField]
		private int _ammo = 100;

		public int Ammo { get => _ammo; set => _ammo = value; }

		public bool ReadyToFire { get { return IsLoaded && Ammo != 0; } }
		//how long to wait until we are ready to go again
		public bool IsLoaded { get { return TimeUntilLoaded <= Time.time; } }
		//how long to wait until we are ready to go again
		public float TimeUntilLoaded { get { return _lastFireTime + _player.CurrentWeapon.ReloadTime; } }
		//how long to wait until we are ready to go again
		public float ReloadProgress {
			get {
				return Mathf.Clamp01(((Time.time - _lastFireTime) / _player.CurrentWeapon.ReloadTime));
			}
		}
		// private public Weapon weapon { get => _player.CurrentWeapon; }

		public WeaponCallback OnFireStart { get; set; }
		public WeaponCallback OnFireEnd { get; set; }
		public WeaponCallback OnEmptyFire { get; set; }
		public WeaponCallback OnOutOfAmmo { get; set; }
		public WeaponCallback OnReload { get; set; }


		private float _lastFireTime = 0.0f;


		private void Start() {
			Container.AutoInject(this);
		}

		public IEnumerator Fire() {
			if (!IsLoaded) {
				Debug.LogFormat("Cannot fire {0}, not loaded, Time: {1}, TimeUntilLoaded: {2} ", _player.CurrentWeapon, Time.time, TimeUntilLoaded);
				if (OnEmptyFire != null) { OnEmptyFire(_player.CurrentWeapon); }
				yield break;
			}
			if (_ammo == 0) {
				Debug.LogFormat("Cannot fire {0}, No ammo", _player.CurrentWeapon);
				if (OnEmptyFire != null) { OnEmptyFire(_player.CurrentWeapon); }
				yield break;
			}

			for (int i = 0; i < _player.CurrentWeapon.ProjectilesPerShot; i++) {
				Vector3 offset = Vector3.zero;
				if (_player.CurrentWeapon.ProjectileOffsets.Length <= i) {
					offset = this.transform.position;
				} else {
					offset = this.transform.TransformPoint(_player.CurrentWeapon.ProjectileOffsets[i]);
				}
				GameObject go = Instantiate(_player.CurrentWeapon.ProjectilePrefab, offset, this.transform.rotation, null);
				IProjectile projectile = go.GetComponent<IProjectile>();
				projectile.Weapon = _player.CurrentWeapon;
				projectile.Team = _team;
				projectile.OnFire();
				//`Mat delete after _player.CurrentWeapon.Lifetime;
				_ammo--;
				if (_player.CurrentWeapon.DelayBetweenProjectiles > 0.0f) {
					yield return new WaitForSeconds(_player.CurrentWeapon.DelayBetweenProjectiles);
				}
				projectile.Body.velocity = projectile.Transform.forward * _player.CurrentWeapon.Speed;
			}
			_lastFireTime = Time.time;
			yield return null;
		}
	}
}