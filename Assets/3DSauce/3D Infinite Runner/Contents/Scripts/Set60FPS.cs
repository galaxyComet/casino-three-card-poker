using UnityEngine;

public class Set60FPS : MonoBehaviour {
	
	// See docs for more info:
	// http://docs.unity3d.com/Documentation/ScriptReference/Application-targetFrameRate.html
	
	void Awake() {
		Application.targetFrameRate = 60;
	}
	
}