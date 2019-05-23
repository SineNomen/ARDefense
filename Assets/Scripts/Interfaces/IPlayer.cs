using AOFL.Promises.V1.Interfaces;
using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;

namespace Sojourn.ARDefense.Interfaces {
	interface IPlayer {
		eKillableTeam Team { get; }
		int MaxHealth { get; }
		int CurrentHealth { get; }
		Weapon CurrentWeapon { get; set; }
		IPromise RequestFireCannon();
	}
}