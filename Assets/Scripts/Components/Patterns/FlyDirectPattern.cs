using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(Fighter))]
	public class FlyDirectPattern : MonoBehaviour {
		[SerializeField]
		private float _speed = 50.0f;
		[SerializeField]
		private bool _startOnCreate = true;

		[AutoInject]
		private IGameManager _gameManager = null;

		private Fighter _fighter;

		public float Speed { get => _speed; }

		private void Awake() {
			_fighter = GetComponent<Fighter>();
		}

		private void Start() {
			Container.AutoInject(this);
			if (_startOnCreate) {
				Setup();
				StartPattern();
			}
		}

		public void Setup() {
			transform.LookAt(_gameManager.Player1Base.CenterPosition);
		}

		public void Update() {
			transform.LookAt(_gameManager.Player1Base.CenterPosition);
			_fighter.Body.velocity = transform.forward * _speed;
		}

		public void StartPattern() {
		}
	}
}