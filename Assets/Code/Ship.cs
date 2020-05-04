using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
    [SerializeField]
    Vector2 m_forwardVec = Vector2.up;

    // base game object properties
    [SerializeField]
    [Range(1.0f, 5.0f)]
    float m_statAcceleration = 3f;

    [SerializeField]
    [Range(1.0f, 5.0f)]
    float m_statTopSpeed = 3f;

    // TODO implement machine weight
    //[SerializeField]
    //[Range( 100, 400 )]
    //float m_statWeight = 250;

    private float m_curSpeed = 0;
    protected float curSpeed {
        get {
            if ( m_isBoosting ) return m_boostSpeedMult * m_curSpeed;
            return m_curSpeed;
        }
    } 
    
    // derived game object properties

    // accel-based
    float m_speedIncPerSec = 0.5f;
    float m_speedDecCollision = 0.1f;
    float m_rotateSpeed = 3.0f;
    protected float rotateSpeed {  get { return m_rotateSpeed; } }
    float m_turnSlow = 0.2f;

    // weight-based
    float m_attackJump = 1.5f;
    float m_chargeCooldown = 1.0f;
    float m_attackCooldown = 0.5f;
    float m_attackTime = 0.2f;

    // top speed based
    float m_topSpeed = 2.0f;
    protected float topSpeed {  get { return m_topSpeed; } }
    float m_boostTime = 1.0f;
    float m_boostSpeedMult = 1.5f;
    float m_boostMoveMult = 0.75f;

    // state
    float m_attackCooldownRemaining = 0.0f;
    float m_boostTimeElapsed = 0.0f;

    bool m_movingLeft = false;
    bool isMovingLeft {
        set { m_movingLeft = value;
            m_animator.SetBool( "isMovingLeft", value );
        }
    }

    bool m_movingRight = false;
    bool isMovingRight {
        set { m_movingRight = value;
            m_animator.SetBool( "isMovingRight", value );
        }
    }

    float m_chargeTime = 0.0f;
    public bool isCharged { get { return m_chargeTime > 0.0f; } }

    bool m_isAttacking = false;
    public bool isAttacking { get { return m_isAttacking; } }

    private bool m_isDead = false;
    
    bool m_isBoosting = false;

    // sub-objects
    [SerializeField]
    GroundDetector m_groundDetector = null;

    [SerializeField]
    AudioSource m_engineSound = null;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float m_engineVolume = 1.0f;

    [SerializeField]
    AudioSource m_scrapeSound = null;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float m_scrapeVolume = 1.0f;

    // components
    Rigidbody2D m_rigidBody = null;
    Renderer m_renderer = null;
    Animator m_animator = null;

    protected GameManager m_game;

    IEnumerator boost() {
        m_animator.SetBool( "isBoosting", true );

        // TODO custom boost time
        while( m_boostTimeElapsed < m_boostTime ) {
            m_boostTimeElapsed += Time.deltaTime;
            yield return null;
        }

        m_isBoosting = false;
        m_animator.SetBool( "isBoosting", false );
    }

    void startBoost() {
        m_boostTimeElapsed = 0.0f;

        if ( m_isBoosting ) {
            return;
        }

        m_isBoosting = true;
        StartCoroutine( boost() );
    }

    protected virtual void OnCollisionEnter2D( Collision2D a_collision ) {
        if ( a_collision.collider.tag == "road" ) {
            m_scrapeSound.volume = m_scrapeVolume;
        } 
    }

    void OnCollisionExit2D( Collision2D a_collision ) {
        if ( a_collision.collider.tag == "road" ) {
            m_scrapeSound.volume = 0.0f;
        }
    }

    void OnCollisionStay2D( Collision2D a_collision ) {
        if ( a_collision.collider.tag == "road" ) {
            slowDown();
        }
    }

    protected virtual void Awake() {
        m_game = FindObjectOfType<GameManager>();

        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_renderer = GetComponent<Renderer>();
    }

    void Start() {
        var statAccelerationFactor = ( m_statAcceleration - 1.0f ) / 4.0f;
        var timeToTopSpeed = 8.0f - statAccelerationFactor * 6.0f;
        m_speedIncPerSec = m_topSpeed / timeToTopSpeed;
        //Debug.Log( "Speed inc per sec: " + m_speedIncPerSec );
        m_speedDecCollision = 0.1f + statAccelerationFactor * 0.05f;
        m_rotateSpeed = m_statAcceleration;
        m_turnSlow = 0.01f - statAccelerationFactor * 0.001f;

        var statTopSpeedFactor = ( m_statTopSpeed - 1.0f ) / 4.0f;
        m_topSpeed = statTopSpeedFactor * 3.0f + 1.0f;
        m_boostTime = statTopSpeedFactor + 0.5f;
        m_boostSpeedMult = 1.0f + statTopSpeedFactor * 0.5f;
        m_boostMoveMult = 1.0f - statTopSpeedFactor * 0.05f;

        if ( m_groundDetector == null ) {
            Destroy( gameObject );
            throw new UnityException( "Ship must have a GroundDetector set" );
        }
    }

    protected void moveLeft() {
        isMovingRight = false;
        isMovingLeft = true;
    }

    protected void moveRight() {
        isMovingLeft = false;
        isMovingRight = true;
    }

    public void stopTurn() {
        isMovingLeft = isMovingRight = false;
    }

    float m_attackVelocity = 0.0f;
    void FixedUpdate() {
        if ( m_isDead ) return;

        if ( !m_game || m_game.raceStarted ) {
            var speed = m_curSpeed;

            if ( m_isBoosting ) {
                // TODO twiddle this value
                speed = Mathf.Max( speed * m_boostSpeedMult, m_topSpeed * 1.2f );
            }

            m_rigidBody.velocity = m_forwardVec * speed;
        }

        // can't do anything while attacking
        if ( m_isAttacking ) {
            m_rigidBody.velocity += Vector2.right * m_attackVelocity;
        }

        // if we're attacking or cooling down from attack, don't allow turn
        if ( m_isAttacking || m_attackCooldownRemaining > 0.0f ) {
            return;
        }

        var mult = 1.0f;
        if ( m_isBoosting ) mult = m_boostMoveMult;
        if ( m_movingLeft ) {
            addTurn( Vector2.left * mult );
        } else if ( m_movingRight ) {
            addTurn( Vector2.right * mult );
        }
    }

    protected virtual void Update() {
        if ( m_isDead ) return;

        if( m_engineSound && ( !m_game || m_game.raceStarted ) ) {
            m_engineSound.volume = Mathf.Min( m_curSpeed * 0.1f, m_engineVolume );
            m_engineSound.pitch = m_curSpeed;
        }

        // can't do anything while attacking
        if ( m_isAttacking ) return;

        updateInput();

        if ( m_game && !m_game.raceStarted ) {
            return;
        }

        accelerate();
        updateCharge();
        checkDeath();
        checkBoost();
        updateColor();
    }

    private void accelerate() {
        if ( m_isBoosting ) return;

        var speedInc = m_speedIncPerSec * Time.deltaTime;
        addSpeed( speedInc );
    }

    protected void setRotation( float a_angle ) {
        m_rigidBody.rotation = a_angle;

        var sin = Mathf.Sin( Mathf.Deg2Rad * m_rigidBody.rotation );
        var cos = Mathf.Cos( Mathf.Deg2Rad * m_rigidBody.rotation );

        m_forwardVec = new Vector2( -sin, cos ).normalized;
    }

    public void slowDown( float a_multiplier = 1.0f ) {
        addSpeed( -m_speedDecCollision * a_multiplier );
    }

    float m_speedMax = 0.0f;
    private void addSpeed( float speedInc ) {

        /*
        // catch up to our highest attained speed after slowing down
        m_speedMax = Mathf.Max( m_speedMax, m_curSpeed );
        if ( m_curSpeed < m_speedMax ) {

            // catch up in 30 steps, plus normal acceleration
            speedInc = ( m_speedMax - m_curSpeed ) * 1.0f / 30.0f + speedInc;
            //Debug.Log( "Catching up" );
        }
        */

        // TODO allow setting minimum speed (in GameManager?)
        // never go slower than 0.1f
        m_curSpeed = Mathf.Min( Mathf.Max( m_curSpeed + speedInc, 0.1f ), m_topSpeed );
    }

    protected virtual void updateCharge() {
        if ( m_chargeTime > 0.0f ) {
            m_chargeTime -= Time.deltaTime;
        }
    }

    protected void updateColor() {
        if ( isAttacking ) {
            m_renderer.material.color = Color.red;
        } else if ( isCharged ) {
            var t = m_chargeTime / m_chargeCooldown;
            m_renderer.material.color = new Color( 0.0f, t + 0.3f, 0.0f );
        } else if ( m_attackCooldownRemaining > 0.0f ) {
            m_renderer.material.color = Color.blue;
        } else if ( m_isBoosting ) {
            //m_renderer.material.color = Color.green;
        } else {
            m_renderer.material.color = Color.white;
        }
    }

    protected virtual void updateInput() { }

    protected virtual void checkBoost() {
        if( m_groundDetector.floorTag == "boost" ) {
            startBoost();
        }
    }

    protected virtual void checkDeath() {
        if( m_groundDetector.floorTag == "space" ) {
            //Debug.Log( gameObject.name + " fell into space" );
            // HACK keep the camera
            if( Camera.main.transform.parent == transform ) {
                Camera.main.transform.SetParent( null );
            }

            // TODO fall coroutine: shrink until invisible then do this stuff
            m_isDead = true;
            StartCoroutine( die() );
        }
    }

    protected void addTurn( Vector2 a_direction ) {
        addSpeed( -m_turnSlow * m_curSpeed );
        setRotation( m_rigidBody.rotation - a_direction.x * m_rotateSpeed );
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine( transform.position, (Vector2)transform.position + m_forwardVec * 2.0f );
    }

    protected IEnumerator attack( bool a_isRight ) {
        // TODO reimplement attack
        yield break;

        m_isAttacking = true;
        m_chargeTime = 0.0f;

        // calculate velocity to get to target position in attack time
        m_attackVelocity = ( a_isRight ? m_attackJump : -m_attackJump ) / m_attackTime;

        // wait for attack time to end
        float timeElapsed = 0.0f;
        while ( timeElapsed < m_attackTime ) {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        m_isAttacking = false;

        // reset x velocity to zero
        {
            var vel = m_rigidBody.velocity;
            vel.x = 0;
            m_rigidBody.velocity = vel;
        }

        // wait for the attack cooldown, a period of time where we can't move
        m_attackCooldownRemaining = m_attackCooldown;
        while( m_attackCooldownRemaining > 0.0f ) {
            m_attackCooldownRemaining -= Time.deltaTime;
            yield return null;
        }

        m_isAttacking = false;
    }

    protected IEnumerator die() {
        m_scrapeSound.volume = 0.0f;
        m_engineSound.volume = 0.0f;
        m_game.soundManager.playSound( SoundManager.Sound.ShipDeath, transform.position );

        // put us behind the track
        m_renderer.sortingOrder = 1;

        var originalScale = transform.localScale;
        var targetScale = originalScale * 0.1f;
        var timeElapsed = 0.0f;
        while( timeElapsed < 1.0f ) {
            timeElapsed += Time.deltaTime;

            var t = timeElapsed / 1.0f;
            transform.localScale = Vector2.Lerp( originalScale, targetScale, t );
            yield return null;
        }

        // remember which way to look for the track based on velocity before we stop
        bool goRight = m_rigidBody.velocity.x < 0;

        // stop forward movement & start explosion animation
        stop();

        // start explosion animation
        m_animator.SetBool( "isDead", true );
        yield return new WaitForSeconds( 0.2f );

        // return to original scale so we can see explosion
        transform.localScale = originalScale;

        // wait for explosion to animate
        yield return new WaitForSeconds( 0.5f );
        m_renderer.material.color = Color.clear;

        // RESPAWN
        m_animator.SetBool( "isDead", false );
        yield return new WaitForSeconds( 0.2f );
        m_renderer.material.color = Color.white;

        // respawn at the nearest path node
        var shortestDistance = 10000.0f;
        Vector2 closestNodePos = Vector2.zero;
        foreach( var node in FindObjectsOfType<PathNode>() ) {
            var distance = Vector2.Distance( node.transform.position, transform.position );
            if( distance < shortestDistance) {
                shortestDistance = distance;
                closestNodePos = node.transform.position;
            }
        }
        transform.position = closestNodePos;

        m_isDead = false;
        m_renderer.material.color = Color.white;
        m_renderer.sortingOrder = 4;
        setRotation( 0.0f );

        m_game.respawn();
    }

    protected void charge() {
        // TODO reimplement charge
        return;

        m_chargeTime = m_chargeCooldown;
    }

    public virtual void stop() {
        m_curSpeed = 0.0f;
        m_movingLeft = m_movingRight = false;
        m_chargeTime = 0.0f;
        m_rigidBody.velocity = Vector2.zero;
    }

    public void kill() {
        stop();
        m_animator.SetBool( "isDead", true );
        m_isDead = true;
        Destroy( gameObject, 0.5f );
    }
}
