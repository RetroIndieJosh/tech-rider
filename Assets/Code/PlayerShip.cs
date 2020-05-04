using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Rigidbody2D ) )]
[RequireComponent( typeof( Sprite ) )]
public class PlayerShip : Ship {
    [SerializeField]
    string m_shipName;
    public string ShipName { get { return m_shipName; } }

    bool m_leftDown = false;
    float m_timeSinceLeftDown = 100.0f;

    bool m_rightDown = false;
    float m_timeSinceRightDown = 100.0f;

    protected override void OnCollisionEnter2D( Collision2D a_collision ) {
        // TODO remove when ShipSide is implemented
        if ( a_collision.collider.tag == "ship" ) {
            var otherShip = a_collision.collider.GetComponent<Ship>();
            handleShipCollision( otherShip );
        }

        base.OnCollisionEnter2D( a_collision );
    }

    // TODO remove when ShipSide is implemented
    protected void handleShipCollision( Ship otherShip ) {
        if ( isAttacking ) {
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
                otherShip.kill();
            }
        } else {
            slowDown( 10.0f );
            Debug.Log( "We aren't attacking" );
        }
    }

    //void OnBecameInvisible() {
        //Debug.Log( "stopped outside camera" );
        //stop();
    //}

    // TODO make this flexible - setting it on awake doesn't work between scenes
    float m_originalCamSize = 0.7f;
    public float originalCamSize {  get { return m_originalCamSize; } }
    float m_targetCamSize = 1.0f;
    float m_camIncreaseFactor = 0.1f;

    protected override void Awake() {
        base.Awake();
        Camera.main.GetComponent<MatchPosition>().setTarget( transform );
    }

    protected override void Update() {
        base.Update();

        if ( m_game && !m_game.raceStarted ) return;

        // update HUD
        if ( m_game && m_game.speedText ) {
            m_game.speedText.text = "" + Mathf.FloorToInt( curSpeed * 50.0f ) + " MPH";
        }

        // adjust camera
        var t = curSpeed / topSpeed;
        const float MAX_CAM_MULT = 2.0f;
        m_targetCamSize = m_originalCamSize + m_originalCamSize * t * ( MAX_CAM_MULT - 1.0f );
        //Debug.Log( "Target cam size: " + m_targetCamSize + " | Current: " + Camera.main.orthographicSize );
        if( m_targetCamSize > Camera.main.orthographicSize ) {
            Camera.main.orthographicSize *= 1.0f + m_camIncreaseFactor * Time.deltaTime;
        } else if( m_targetCamSize < Camera.main.orthographicSize ) {
            Camera.main.orthographicSize *= 1.0f - m_camIncreaseFactor * Time.deltaTime;
        }
    }

    public override void stop() {
        base.stop();

        m_leftDown = m_rightDown = false;
    }

    override protected void updateInput() {
        if( InputManager.leftDown && InputManager.rightDown ) {
        //if ( Input.GetButtonDown( "Left" ) && Input.GetButtonDown( "Right" ) ) {
            if ( !isCharged ) {
                charge();
            }
            return;
        }

        bool doCharge = false;

        if ( InputManager.leftDown ) {
            //if ( Input.GetButtonDown( "Left" ) ) {
            if ( m_game && m_timeSinceRightDown < m_game.doubleInputThreshold ) {
                doCharge = true;
            } else {
                m_leftDown = true;
                m_timeSinceLeftDown = 0.0f;
            }
        } else if( InputManager.leftUp ) {
        //} else if ( Input.GetButtonUp( "Left" ) ) {
            m_leftDown = false;
        }

        if ( InputManager.rightDown ) {
            //if ( Input.GetButtonDown( "Right" ) ) {
            if ( m_game && m_timeSinceLeftDown < m_game.doubleInputThreshold ) {
                doCharge = true;
            } else {
                m_rightDown = true;
                m_timeSinceRightDown = 0.0f;
            }
        } else if ( InputManager.rightUp ) {
            //} else if ( Input.GetButtonUp( "Right" ) ) {
            m_rightDown = false;
        }

        if ( doCharge ) {
            charge();
            m_leftDown = m_rightDown = false;
            return;
        }

        m_timeSinceLeftDown += Time.deltaTime;
        m_timeSinceRightDown += Time.deltaTime;

        if ( m_leftDown ) {
            if ( isCharged ) {
                StartCoroutine( attack( false ) );
                m_leftDown = m_rightDown = false;
            } else {
                moveLeft();
            }
        } else if ( m_rightDown ) {

            if ( isCharged ) {
                StartCoroutine( attack( true ) );
                m_leftDown = m_rightDown = false;
            } else {
                moveRight();
            }
        } else {
            stopTurn();
        }
    }
}

