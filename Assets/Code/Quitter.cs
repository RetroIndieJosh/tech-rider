using UnityEngine;
using System.Collections;

public class Quitter : MonoBehaviour {
	void Update () {
        if( Input.GetButtonDown( "Quit" ) ) {
            Debug.Log( "Quit" );
            Application.Quit();
        }
	}
}
