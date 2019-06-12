using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(SimpleKillable))]
	public class SimpleProjectile : MonoBehaviour, IProjectile {
		[SerializeField]
		protected Rigidbody _body = null;
		protected SimpleKillable _killable = null;

		public Transform Transform { get => this.transform; }
		public Weapon Weapon { get; set; }
		public Rigidbody Body { get => _body; }

		private void Awake() {
			_killable = GetComponent<SimpleKillable>();
		}

		public virtual void Launch(Cannon cannon, Transform target) { }

		public void OnFire() {
			Invoke("Destroy", Weapon.ProjectileLifetime);
		}

		//we may have been destroyed already
		private void Destroy() {
			if (this != null && this.gameObject != null) {
				Destroy(this.gameObject);
			}
		}

		public void OnDamaged(IKillable us) { }
	}
}