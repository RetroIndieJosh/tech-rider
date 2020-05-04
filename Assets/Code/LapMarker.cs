using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LapMarker : MonoBehaviour {
    [SerializeField]
    float m_targetY = 0.0f;

    [SerializeField]
    int m_numLaps = 4;

    [SerializeField]
    Text m_lapText = null;

    PathNode m_startNode = null;

    int m_lapCount = 0;

    GameManager m_game = null;

    void Awake() {
        updateLapText();
    }

    void Update() {

        // HACK to get track because it doesn't go active until after us
        if ( m_startNode == null ) {
            m_game = FindObjectOfType<GameManager>();
            if ( m_game ) {
                var track = m_game.track;
                if ( track != null ) {
                    m_startNode = track.firstNode;
                    //Debug.Log( "Got node" );
                }
            }
        }
    }

    void OnTriggerEnter2D( Collider2D a_collider ) {
        var pos = a_collider.transform.position;
        pos.y = m_targetY;
        a_collider.transform.position = pos;

        if( a_collider.GetComponent<PlayerShip>() != null ) {
            ++m_lapCount;

            if( m_lapCount >= m_numLaps ) {
                m_game.endRace();
                return;
            }

            updateLapText();
        } else if( a_collider.GetComponent<AiShip>() != null ) {
            a_collider.GetComponent<AiShip>().resetPath( m_startNode );
        }

    }

    void updateLapText() {
        if( m_lapText ) {
            m_lapText.text = "LAP " + ( m_lapCount + 1 ) + " / " + m_numLaps;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine( Vector2.left * 1000.0f + Vector2.up * m_targetY, Vector2.right * 1000.0f + Vector2.up * m_targetY );
    }
}
