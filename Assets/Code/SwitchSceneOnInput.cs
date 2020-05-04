using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SwitchSceneOnInput : MonoBehaviour {
    [SerializeField]
    string m_targetSceneName = "";

    bool m_leftDown = false;
    bool m_rightDown = false;

    void Start() {
        m_leftDown = InputManager.leftDown;
        m_rightDown = InputManager.rightDown;
        //m_leftDown = Input.GetButtonDown( "Left" );
        //m_rightDown = Input.GetButtonDown( "Right" );
        Debug.Log( "Left/right: " + m_leftDown + " " + m_rightDown );
    }
	
	// Update is called once per frame
	void Update () {

        // if either of the buttons is still down, refresh state and return
        if( m_leftDown || m_rightDown ) {
            m_leftDown = InputManager.leftDown;
            m_rightDown = InputManager.rightDown;
            //m_leftDown = Input.GetButtonDown( "Left" );
            //m_rightDown = Input.GetButtonDown( "Right" );
            Debug.Log( "Left/right: " + m_leftDown + " " + m_rightDown );
            return;
        }

        // at this point both of the buttons have gone up - now we check if they've gone down
        if ( InputManager.leftDown || InputManager.rightDown ) {
        //if ( Input.GetButtonDown( "Left" ) || Input.GetButtonDown( "Right" ) ) {
            Destroy( gameObject );
            SceneManager.LoadScene( m_targetSceneName );
        }
	}
}
