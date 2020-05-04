using UnityEngine;
using System.Collections;

public class Unlockable : MonoBehaviour {
    [SerializeField]
    string m_playerPrefString = null;

    bool m_isLocked = true;
    public bool locked {  get { return m_isLocked; } }

    void Awake() {
        if( PlayerPrefs.HasKey( m_playerPrefString )) {
            if( PlayerPrefs.GetInt( m_playerPrefString ) == 1 ) {
                m_isLocked = false;
            }
        } else {
            PlayerPrefs.SetInt( m_playerPrefString, 0 );
        }

        if( m_isLocked ) {
            GetComponent<Renderer>().material.color = new Color( 0.2f, 0.2f, 0.2f, 1.0f );
        }
    }
}
