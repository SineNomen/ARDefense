using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.ARFoundation;

namespace Sojourn.ARDefense.Components {
	//allows the player to move around using a keyboard in non-device builds
	public class DebugController : MonoBehaviour {
		[SerializeField] private bool _useDebugControls = false;
		[SerializeField] private float _moveSpeed = 1.0f;
		[SerializeField] private float _rotateSpeed = 2.0f;
		[SerializeField] private float _turboMultiplier = 2.50f;

		[SerializeField] private KeyCode _forwardKey = KeyCode.W;
		[SerializeField] private KeyCode _backKey = KeyCode.S;
		[SerializeField] private KeyCode _strafeLeftKey = KeyCode.A;
		[SerializeField] private KeyCode _strafeRightKey = KeyCode.D;
		[SerializeField] private KeyCode _raiseKey = KeyCode.Q;
		[SerializeField] private KeyCode _lowerKey = KeyCode.E;

		[SerializeField] private KeyCode _turnLeftKey = KeyCode.LeftArrow;
		[SerializeField] private KeyCode _turnRightKey = KeyCode.RightArrow;
		[SerializeField] private KeyCode _turnUpKey = KeyCode.UpArrow;
		[SerializeField] private KeyCode _turnDownKey = KeyCode.DownArrow;

		[SerializeField] private KeyCode _resetOrientationKey = KeyCode.Escape;
		[SerializeField] private KeyCode _resetAllKey = KeyCode.Space;
		[SerializeField] private KeyCode _turboKey = KeyCode.LeftShift;
		[SerializeField] private KeyCode _baseKey = KeyCode.B;

		[AutoInject]
		private IGameManager _gameManager = null;
		[AutoInject]
		private ILevelManager _levelManager = null;

		private Quaternion _startingRotation;
		private Vector3 _startingPosition;
		private Transform _transform;
		private bool _turboOn = false;


#if UNITY_EDITOR || UNITY_STANDALONE
		private void Start() {
			Container.AutoInject(this);
			if (_useDebugControls) {
				Destroy(_gameManager.DeviceCamera.gameObject.GetComponent<ARCameraBackground>());
				Destroy(_gameManager.DeviceCamera.gameObject.GetComponent<TrackedPoseDriver>());
				_transform = _gameManager.DeviceCamera.transform;
				_startingRotation = _gameManager.DeviceCamera.transform.rotation;
				_startingPosition = _gameManager.DeviceCamera.transform.position;
			}
		}

		private void Update() {
			if (_useDebugControls) {
				_turboOn = Input.GetKey(_turboKey);
				if (Input.GetKey(_forwardKey)) { MovePosition(Vector3.forward); }
				if (Input.GetKey(_backKey)) { MovePosition(Vector3.back); }
				if (Input.GetKey(_strafeLeftKey)) { MovePosition(Vector3.left); }
				if (Input.GetKey(_strafeRightKey)) { MovePosition(Vector3.right); }
				if (Input.GetKey(_raiseKey)) { MovePosition(Vector3.up); }
				if (Input.GetKey(_lowerKey)) { MovePosition(Vector3.down); }

				if (Input.GetKey(_turnLeftKey)) { MoveRotation(Vector3.down); }
				if (Input.GetKey(_turnRightKey)) { MoveRotation(Vector3.up); }
				if (Input.GetKey(_turnUpKey)) { MoveRotation(Vector3.right); }
				if (Input.GetKey(_turnDownKey)) { MoveRotation(Vector3.left); }

				if (Input.GetKey(_baseKey)) { _transform.LookAt(_levelManager.PlayerBase.transform); }
				if (Input.GetKey(_resetOrientationKey)) { _transform.rotation = _startingRotation; }
				if (Input.GetKey(_resetAllKey)) {
					_transform.rotation = _startingRotation;
					_transform.position = _startingPosition;
				}
			}
		}

		private void MovePosition(Vector3 adjust) {
			Vector3 delta = Vector3.zero;
			if (_turboOn) {
				delta = adjust * _moveSpeed * _turboMultiplier;
			} else {
				delta = adjust * _moveSpeed;
			}
			_transform.Translate(delta);
		}

		private void MoveRotation(Vector3 adjust) {
			Vector3 rot = Vector3.zero;
			if (_turboOn) {
				rot = adjust * _rotateSpeed * _turboMultiplier;
			} else {
				rot = adjust * _rotateSpeed;
			}
			_transform.Rotate(rot);
		}
#endif// UNITY_EDITOR || UNITY_STANDALONE
	}
}