using AOFL.Promises.V1.Interfaces;
using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using System.Collections.Generic;

namespace Sojourn.ARDefense.Interfaces {
	public interface IPlayer {
		// eKillableTeam Team { get; }
		Weapon CurrentWeapon { get; }
		List<Weapon> WeaponList { get; }
		void SetCurrentWeapon(int index);
		IPromise RequestFireCannon();
	}
}