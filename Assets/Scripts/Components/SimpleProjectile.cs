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
	public class SimpleProjectile : MonoBehaviour, IProjectile {
		[SerializeField]
		private Rigidbody _body = null;
		[SerializeField]
		private eKillableTeam _team = eKillableTeam.Player1;
		private SimpleKillable _killable = null;


		public eKillableTeam Team { get => _team; set => _team = value; }

		public Transform Transform { get => this.transform; }
		public Weapon Weapon { get; set; }
		public Rigidbody Body { get => _body; }

		private void Awake() {
			_killable = GetComponent<SimpleKillable>();
		}

		// private void Start() {
		// }

		public void OnFire() {
			Invoke("Destroy", Weapon.ProjectileLifetime);
		}

		//we may have been destroyed already
		private void Destroy() {
			if (this != null && this.gameObject != null) {
				Destroy(this.gameObject);
			}
		}

		public void OnKilled(IKillable us) {
			Destroy(this.gameObject);
		}
		public void OnDamaged(IKillable us) { }
	}
}