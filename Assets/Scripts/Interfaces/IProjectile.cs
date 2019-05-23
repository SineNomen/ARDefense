using AOFL.Promises.V1.Interfaces;
using Sojourn.ARDefense.ScriptableObjects;
using UnityEngine;

namespace Sojourn.ARDefense.Interfaces {
	interface IProjectile {
		Transform Transform { get; }
		eKillableTeam Team { get; set; }
		Weapon Weapon { get; set; }
		Rigidbody Body { get; }

		void OnFire();
	}
}