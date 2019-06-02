using Sojourn.PicnicIOC;
using Sojourn.ARDefense.Interfaces;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	public class SoundOnEvent : MonoBehaviour {
		[SerializeField]
		private AudioClip _clip = null;
		[SerializeField]
		private bool _preload = false;
		private AudioSource _source = null;

		[AutoInject]
		private IAudioManager _audioManager = null;

		public bool DestroyAfter { get; set; } = true;
		public string Tag { get; set; } = null;

		private void Start() {
			if (_preload) {
				_source = CreateObject();
			}
		}

		private AudioSource CreateObject() {
			//`Mat inject here because we may be getting called the same frame we get the OnFire and Start is not called yet
			Container.AutoInject(this);
			GameObject obj = new GameObject(string.Format("{0} {1}Sound", this.gameObject.name, Tag));
			obj.transform.SetParent(this.transform);
			AudioSource source = obj.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.outputAudioMixerGroup = _audioManager.SfxGroup;
			source.clip = _clip;
			obj.SetActive(false);
			return source;
		}

		[ContextMenu("Do Sound")]
		public void PlaySound() {
			if (_source == null) { _source = CreateObject(); }
			_source.gameObject.SetActive(true);
			_source.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
			_source.transform.SetParent(null);
			_source.Play();
			if (DestroyAfter) {
				DestroyAfterTime killer = _source.gameObject.AddComponent<DestroyAfterTime>();
				killer.Delay = _clip.length;
				killer.TriggerDestroy();
			}
		}
	}
}
