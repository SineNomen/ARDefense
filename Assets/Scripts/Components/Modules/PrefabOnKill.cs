using Sojourn.PicnicIOC;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	public class PrefabOnKill : MonoBehaviour {
		[SerializeField]
		private GameObject _prefab = null;
		[SerializeField]
		private bool _preload = false;
		private GameObject _preloadedObject = null;

		[AutoInject]
		private IGameManager _gameManager = null;

		private void Awake() {
			if (_preload) {
				_preloadedObject = Instantiate(_prefab, this.transform);
				_preloadedObject.SetActive(false);
			}
		}

		private void Start() {
			Container.AutoInject(this);
		}

		[ContextMenu("Do Prefab")]
		public void TestDamage() { OnKill(null); }
		public void OnKill(IKillable us) {
			GameObject obj = (_preload ? _preloadedObject : Instantiate(_prefab, _gameManager.WorldParent));
			obj.transform.SetParent(_gameManager.WorldParent);
			obj.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
			// Debug.LogFormat("This: {0}, prefab: {1}", this.transform.position, obj.transform.position);
			obj.SetActive(true);
			ParticleSystem ps = obj.GetComponent<ParticleSystem>();
			if (ps != null) { ps.Play(); }
			AudioSource audio = obj.GetComponent<AudioSource>();
			if (audio != null) { audio.Play(); }
		}
	}
}
