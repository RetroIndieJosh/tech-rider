using UnityEngine;
using System.Collections;

public class InputManager {
    public static bool leftDown {
        get {
            return Input.GetButtonDown( "Left" )
                || ( Input.GetMouseButtonDown( 0 ) && Input.mousePosition.x < Screen.width * 0.5f );
        }
    }

    public static bool leftUp {
        get {
            return Input.GetButtonUp( "Left" ) || Input.GetMouseButtonUp( 0 );
        }
    }

    public static bool rightDown {
        get {
            return Input.GetButtonDown( "Right" )
                || ( Input.GetMouseButtonDown( 0 ) && Input.mousePosition.x >= Screen.width * 0.5f );
        }
    }

    public static bool rightUp {
        get {
            return Input.GetButtonUp( "Right" ) || Input.GetMouseButtonUp( 0 );
        }
    }

    /*
    public enum InputKey {
        Left, Right, Length
    }

    static bool[] m_isDown = new bool[(int)InputKey.Length];
    static bool[] m_isPressed = new bool[(int)InputKey.Length];

    public static bool isDown( InputKey a_input ) {
        return m_isDown[(int)a_input];
    }
    
    public static bool isUp( InputKey a_input ) {
        return !isDown( a_input );
    }

    public static bool getAndClearPress( InputKey a_input ) {
        var result = m_isPressed[(int)a_input];
        m_isPressed[(int)a_input] = false;
        return result;
    }

    public static void update() {
        for( var i = 0; i < m_isDown.Length; ++i ) {
            var inputKey = (InputKey)i;
            var inputKeyString = inputKey.ToString();
            if( Input.GetButtonDown( inputKeyString ) ) {
                if ( !m_isDown[i] ) m_isPressed[i] = true;
                m_isDown[i] = true;
            }

            if ( Input.GetButtonUp( inputKeyString ) ) {
                m_isDown[i] = false;
            }
        }

        if( getAndClearPress(InputKey.Left ) ) {
            Debug.Log( "Left press" );
        }

        if ( isDown( InputKey.Left ) ) {
            Debug.Log( "Left down" );
        } else {
            //Debug.Log( "Left up" );
        }

        if( getAndClearPress(InputKey.Right ) ) {
            Debug.Log( "Right press" );
        }

        if ( isDown( InputKey.Right ) ) {
            Debug.Log( "Right down" );
        } else {
            //Debug.Log( "Right up" );
        }
    }
    */
}
