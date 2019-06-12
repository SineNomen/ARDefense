using AOFL.Promises.V1.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.ARDefense.ScriptableObjects;
using UnityEngine;

namespace Sojourn.ARDefense.Interfaces {
	public interface IProjectile {
		Transform Transform { get; }
		Weapon Weapon { get; set; }
		Rigidbody Body { get; }

		void Launch(Cannon cannon, Transform target);
	}
}