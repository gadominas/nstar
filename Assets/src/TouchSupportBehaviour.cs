using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSupportBehaviour : MonoBehaviour {
	protected bool touchSupported = false;

	void Start() {
		touchSupported = Input.touchSupported;

		#if UNITY_EDITOR
		touchSupported = true;
		#endif
	}
}
