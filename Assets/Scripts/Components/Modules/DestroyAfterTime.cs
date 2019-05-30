using Sojourn.PicnicIOC;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	public class DestroyAfterTime : MonoBehaviour {
		[SerializeField]
		private float _delay;

		public float Delay { get => _delay; set => _delay = value; }

		public void TriggerDestroy() { Invoke("Kill", _delay); }
		private void Kill() { Destroy(this.gameObject); }
	}
}
