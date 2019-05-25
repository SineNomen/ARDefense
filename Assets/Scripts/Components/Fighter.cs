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
	[RequireComponent(typeof(Rigidbody))]
	public class Fighter : MonoBehaviour {
		[SerializeField]
		private Cannon[] _cannons = null;

		[SerializeField]
		private eKillableTeam _team = eKillableTeam.Player1;
		[SerializeField]

		public eKillableTeam Team { get => _team; set => _team = value; }
		public Rigidbody Body { get; private set; }
		public Transform Transform { get => this.transform; }


		[AutoInject]
		private IGameManager _gameManager = null;
		private static int _fighterCount = 0;

		private void Awake() {
			Body = GetComponent<Rigidbody>();
			gameObject.name = string.Format("Fighter #{0}", _fighterCount);
			_fighterCount++;
		}

		private void Start() {
			Container.AutoInject(this);
			_gameManager.RegisterEnemy(this.gameObject);
		}

		// private void OnCollisionEnter(Collision collision) {
		// 	Debug.LogFormat("Collider Enter: {0}", collision.gameObject);
		// }

		public IPromise FireCannons() {
			List<IPromise> list = new List<IPromise>();
			foreach (Cannon c in _cannons) {
				if (c.ReadyToFire) {
					list.Add(this.StartCoroutineAsPromise(c.Fire()));
				}
			}
			return new Promise().All(list);
		}

		public void OnKilled(IKillable us) {
			_gameManager.UnregisterEnemy(this.gameObject);
			Destroy(this.gameObject);
		}
		public void OnDamaged(IKillable us) { }
	}
}