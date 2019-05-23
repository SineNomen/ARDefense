using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;

/*
Has a list of weapons
	a current weapon
Is damagable
has a team

*/
namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(SimpleKillable))]
	public class Player : MonoBehaviour, IPlayer {
		[SerializeField]
		private Cannon _cannon = null;
		public Weapon CurrentWeapon { get; set; }

		[AutoInject]
		private IGameManager _gameManager = null;

		private void Awake() {
			Container.Register<IPlayer>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
		}

		public IPromise RequestFireCannon() {
			if (_cannon.ReadyToFire) {
				return this.StartCoroutineAsPromise(_cannon.Fire());
			}
			return null;
		}

		public void OnKilled(IKillable us) {
			_gameManager.OnPlayerKilled();
			Destroy(this.gameObject);
		}

		public void OnDamaged(IKillable us) {
			//flash red or something
		}
	}
}