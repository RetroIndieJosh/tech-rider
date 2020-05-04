using UnityEngine;
using System.Collections;

public class Persistent : MonoBehaviour {
	void Awake () {
        DontDestroyOnLoad( gameObject );
	}
}
