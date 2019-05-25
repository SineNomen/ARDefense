using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System;
/*
	A level is a series of waves When the player finishes each wave, the Level ends
	A wave is a series of events
*/


namespace Sojourn.ARDefense.Components {
	public class Wave {

	}

	public class Level : ScriptableObject {
		public Wave[] WaveList;
	}
}