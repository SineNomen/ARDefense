using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using System.Collections.Generic;
using UnityEngine;
using AOFL.Promises.V1.Interfaces;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(SimpleKillable))]
	public class Player : MonoBehaviour, IPlayer {
		[SerializeField]
		private Cannon _cannon = null;
		[SerializeField]
		private List<Weapon> weaponList = new List<Weapon>();

		public Weapon CurrentWeapon { get; private set; }
		public List<Weapon> WeaponList { get => weaponList; }

		[AutoInject]
		private ILevelManager _levelManager = null;
		[AutoInject]
		private IDisplayManager _displayManager = null;

		private void Awake() {
			Container.Register<IPlayer>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
		}

		public void SetCurrentWeapon(int index) {
			if (index < 0 || index >= WeaponList.Count) { return; }
			CurrentWeapon = WeaponList[index];
			_displayManager.PushDisplay(CurrentWeapon.DisplayPrefab);
		}

		public IPromise RequestFireCannon() {
			_cannon.Weapon = CurrentWeapon;
			if (_cannon.IsReadyToFire) {
				Transform target = null;
				if (_displayManager.CurrentDisplay != null && _displayManager.CurrentDisplay.Reticule.LockedObject != null) {
					target = _displayManager.CurrentDisplay.Reticule.LockedObject.transform;
				}
				return this.StartCoroutineAsPromise(_cannon.Fire(target));
			}
			return null;
		}

		public void OnKill(IKillable us) {
			_levelManager.OnPlayerKilled();
		}
		//eventually, we may want to tell the user somehow, maybe using UI
		// public void OnDamaged(IKillable us) {
		//flash red or something
		// }
	}
}