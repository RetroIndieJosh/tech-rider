using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Collision2D ) )]
public class GroundDetector : MonoBehaviour {
    string m_floorTag = "none";

    public string floorTag { get { return m_floorTag; } }

    void OnTriggerEnter2D( Collider2D a_collider ) {
        m_floorTag = a_collider.tag;
        Debug.Log( "Floor tag: " + m_floorTag );
    }

    void OnTriggerExit2D( Collider2D a_collider ) {
        m_floorTag = "none";
    }
}
