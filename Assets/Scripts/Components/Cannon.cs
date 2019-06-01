using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.PicnicIOC;
using UnityEngine;
using System.Collections;

namespace Sojourn.ARDefense.Components {
	//`Mat cannon should be placed on the Killable object, not on the transform that will fire (barrelTransform)
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
			Killable = GetComponent<IKillable>();
			if (Killable == null) {
				Debug.LogErrorFormat("Cannon {0} is not on a Killable object", gameObject.name);
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

				// projectile.OnFire();
				//`Mat Broadcast message
				projectile.Transform.gameObject.BroadcastMessage("OnFire", SendMessageOptions.RequireReceiver);
				_ammo--;
				projectile.Body.velocity = projectile.Transform.forward * Weapon.Speed;
				if (Weapon.DelayBetweenProjectiles > 0.0f) {
					yield return new WaitForSeconds(Weapon.DelayBetweenProjectiles);
				}
			}
			_lastFireTime = Time.time;
			yield return null;
			IsFiring = false;
		}
	}
}