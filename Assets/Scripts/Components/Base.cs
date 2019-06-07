using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	public delegate void BaseEvent(Base b);

	//This is the bae you must defend, most enemies will target this directly
	[RequireComponent(typeof(SimpleKillable))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class Base : MonoBehaviour {
		[SerializeField]
		private eKillableTeam _team = eKillableTeam.Player1;

		public eKillableTeam Team { get => _team; set => _team = value; }
		public Rigidbody Body { get; private set; }
		public Vector3 Size { get => _collider.bounds.size; }
		public Transform Transform { get => this.transform; }
		public Vector3 CenterPosition { get => this.transform.position + Vector3.up * 0.5f; }
		public BaseEvent OnBaseKilled { get; set; }

		private bool _killed = false;
		private Collider _collider;
		private void Start() {
			Container.AutoInject(this);
			Body = GetComponent<Rigidbody>();
			_collider = GetComponent<Collider>();
		}

		[ContextMenu("Kill Base")]
		private void TestKill() {
			OnKill(null);
		}

		private void OnKill(IKillable us) {
			if (_killed) { return; }
			_killed = true;//e may get hit multiple time in the lastframe
			if (OnBaseKilled != null) { OnBaseKilled(this); }
		}
	}
}