using UnityEngine;
using System.Collections;

public class ResetPlayerPrefs : MonoBehaviour {
	void Awake() {
        PlayerPrefs.DeleteAll();
	}
}
