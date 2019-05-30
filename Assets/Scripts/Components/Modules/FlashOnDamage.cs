using Sojourn.ARDefense.Interfaces;
using UnityEngine;
using DG.Tweening;

namespace Sojourn.ARDefense.Components {
	[DisallowMultipleComponent]
	public class FlashOnDamage : MonoBehaviour {
		[SerializeField]
		private Color _flashColor = Color.red;
		[SerializeField]
		private float _transitionTime = 0.3f;
		[SerializeField]
		private int _flashCount = 3;

		private Renderer _renderer = null;
		private Color _startColor = Color.white;
		private Tween _flashTween = null;

		// [AutoInject]
		// private IGameManager _gameManager = null;

		private void Awake() {
			_renderer = GetComponent<Renderer>();
			if (_renderer == null) {
				_renderer = GetComponentInChildren<Renderer>();
			}
			_startColor = _renderer.material.color;
		}

		[ContextMenu("Do Flash")]
		public void TestDamage() { OnDamaged(null); }
		public void OnDamaged(IKillable us) {
			if (_flashTween != null) { _flashTween.Kill(); }
			// _renderer.material.color = _startColor;

			Sequence seq = DOTween.Sequence();
			seq.Append(_renderer.material.DOColor(_flashColor, _transitionTime).SetEase(Ease.InSine));
			seq.Append(_renderer.material.DOColor(_startColor, _transitionTime).SetEase(Ease.OutSine));
			seq.SetLoops(_flashCount);
		}
	}
}