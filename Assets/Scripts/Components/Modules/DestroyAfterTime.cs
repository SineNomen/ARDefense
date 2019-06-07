using UnityEngine;

namespace Sojourn.ARDefense.Components {
	[DisallowMultipleComponent]
	public class DestroyAfterTime : MonoBehaviour {
		[SerializeField]
		private float _delay;

		public float Delay { get => _delay; set => _delay = value; }

		public void TriggerDestroy() { Invoke("Kill", _delay); }
		private void Kill() { Destroy(this.gameObject); }
	}
}
