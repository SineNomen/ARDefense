using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using UnityEngine;
using AOFL.Promises.V1.Interfaces;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(SimpleKillable))]
	public class Player : MonoBehaviour, IPlayer {
		[SerializeField]
		private Cannon _cannon = null;
		public Weapon CurrentWeapon { get; set; }

		[AutoInject]
		private ILevelManager _levelManager = null;

		private void Awake() {
			Container.Register<IPlayer>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
		}

		public IPromise RequestFireCannon() {
			_cannon.Weapon = CurrentWeapon;
			if (_cannon.IsReadyToFire) {
				return this.StartCoroutineAsPromise(_cannon.Fire());
			}
			return null;
		}

		public void OnKill(IKillable us) {
			_levelManager.OnPlayerKilled();
			Destroy(this.gameObject);
		}
		//eventually, we may want to tell the user somehow, maybe using UI
		// public void OnDamaged(IKillable us) {
		//flash red or something
		// }
	}
}