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
	public class Player : MonoBehaviour, IPlayer, IKillable {
		[SerializeField]
		private Cannon _cannon = null;

		[SerializeField]
		private eKillableTeam _team = eKillableTeam.Player1;
		[SerializeField]
		private int _maxHealth = 100;
		private int _currentHealth;

		public eKillableTeam Team { get => _team; }
		public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
		public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
		public Weapon CurrentWeapon { get; set; }

		[AutoInject]
		private IGameManager _gameManager;
		private Collider _collider;

		private void Awake() {
			Container.Register<IPlayer>(this).AsSingleton();
		}

		private void Start() {
			Container.AutoInject(this);
			_currentHealth = _maxHealth;
			_collider = GetComponent<Collider>();
		}

		private void OnCollisionEnter(Collision collision) {
			Debug.LogFormat("Collider Enter: {0}", collision.gameObject);
		}

		private void OnTriggerEnter(Collider collider) {
			Debug.LogFormat("Trigger Enter: {0}", collider.gameObject);
		}

		public IPromise RequestFireCannon() {
			if (_cannon.ReadyToFire) {
				return StartCoroutineAsPromise(_cannon.Fire());
			}
			return null;
		}

		private IPromise StartCoroutineAsPromise(IEnumerator co) {
			Promise p = new Promise();
			StartCoroutine(Runner(co, p));
			return p;
		}

		private IEnumerator Runner(IEnumerator co, IPromise p) {
			yield return StartCoroutine(co);
			p.Resolve();
		}
	}
}