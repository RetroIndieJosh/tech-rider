using UnityEngine;
using System.Collections;

public class ShipSide : MonoBehaviour {
    [SerializeField]
    Ship m_ship = null;

    void OnCollisionEnter2D( Collision2D a_collision ) {
        Debug.Log( name + " collided with " + a_collision.collider.name + " (" + a_collision.collider.tag + ")" );
        if( a_collision.collider.tag == "ship" ) {
            var otherShip = a_collision.collider.GetComponent<Ship>();
            handleShipCollision( otherShip );
        }
    }

    void OnCollisionStay2D( Collision2D a_collision ) {
        if ( a_collision.collider.tag == "road" ) {
            handleWallCollision();
        }
    }

	void Awake() {
        if( m_ship == null ) {
            Destroy( gameObject );
        }
	}

    protected void handleShipCollision( Ship otherShip ) {
        if ( m_ship.isAttacking ) {
            if ( otherShip.isCharged ) {
                // TODO if we attack a charged ship, kill their charge and bounce away
                Debug.Log( "Other ship is charged" );
            } else if ( otherShip.isAttacking ) {
                // TODO if we attack at the same time, bounce away
                Debug.Log( "Other ship is attacking" );
            } else {
                Debug.Log( "Destroy other ship!" );
                // HACK keep the camera
                if ( Camera.main.transform.parent == otherShip.transform ) {
                    Camera.main.transform.SetParent( null );
                }

                // if we're attacking and they aren't charged or attacking, destroy them
                Destroy( otherShip.gameObject );
            }
        } else {
            m_ship.slowDown( 10.0f );
            Debug.Log( "We aren't attacking" );
        }
    }

    protected void handleWallCollision() {
        m_ship.slowDown();
    }

}
