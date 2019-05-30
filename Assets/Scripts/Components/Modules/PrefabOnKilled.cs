using Sojourn.ARDefense.Interfaces;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	public class PrefabOnKilled : MonoBehaviour {
		[SerializeField]
		private GameObject _prefab = null;
		[SerializeField]
		private bool _preload = false;
		private GameObject _preloadedObject = null;

		private void Awake() {
			if (_preload) {
				_preloadedObject = Instantiate(_prefab, null);
				_preloadedObject.SetActive(false);
			}
		}

		[ContextMenu("Do Prefab")]
		public void TestDamage() { OnKilled(null); }
		public void OnKilled(IKillable us) {
			GameObject obj = (_preload ? _preloadedObject : Instantiate(_prefab, null));
			obj.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
			// Debug.LogFormat("This: {0}, prefab: {1}", this.transform.position, obj.transform.position);
			obj.SetActive(true);
			ParticleSystem ps = obj.GetComponent<ParticleSystem>();
			if (ps != null) { ps.Play(); }
		}
	}
}
