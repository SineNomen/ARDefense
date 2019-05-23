using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.Extensions;
using Sojourn.Utility;
using Sojourn.PicnicIOC;
using UnityEngine;
using UnityEngine.UI;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class LaserDisplay : SimpleDisplay, IDisplay {
	[SerializeField]
	private Button _fireButton = null;

	public Button PlaceButton { get => _fireButton; }

	[AutoInject]
	private IPlayer _player1;

	private void Start() {
		base.Start();
		_fireButton.onClick.AddListener(FireCannon);
	}

	private void FireCannon() {
		IPromise p = _player1.RequestFireCannon();
		if (p != null) {
			_reticule.Unload();
			p.Then(() => {
				_reticule.ReloadSpeed = _player1.CurrentWeapon.ReloadTime;
				_reticule.Reload();
			});
		}
	}
}