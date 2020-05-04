using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {
    [SerializeField]
    [Tooltip( "The amount of time, in seconds, allowed between left & right for pressing both 'at the same time'")]
    float m_doubleInputThreshold = 0.1f;
    public float doubleInputThreshold {  get { return m_doubleInputThreshold; } }

    [SerializeField]
    Text m_timeText = null;

    [SerializeField]
    Text m_speedText = null;
    public Text speedText {  get { return m_speedText; } }

    [SerializeField]
    Text m_countdownText = null;

    [SerializeField]
    int m_countdownTime = 3;

    [SerializeField]
    float m_readyTime = 2.0f;

    [SerializeField]
    int m_respawnTime = 1;

    // TODO this is a temporary track select solution
    [SerializeField]
    string m_trackSceneName = "track01";
    public string trackSceneName {  get { return m_trackSceneName; } }

    [SerializeField]
    int m_id = 0;
    public int id {  get { return m_id; } }

    private string m_playerShipName;
    public string playerShipName {  set { m_playerShipName = value; } }

    private Track m_track = null;
    public Track track {  get { return m_track; } }

    public float respawnTime {  get { return m_respawnTime; } }

    public SoundManager soundManager { get { return m_soundManager; } }
    SoundManager m_soundManager = null;

    public bool raceStarted {  get { return m_raceStarted; } }
    bool m_raceStarted = false;

    private bool m_raceEnded = false;

    float m_time = 0.0f;

    HighScore m_highScore = null;

    void Awake() {
        //Debug.Log( "123.433: " + timeToString( 123.433f ) );
        //Debug.Log( "12.3: " + timeToString( 12.3f ) );
        //Debug.Log( "1923.433: " + timeToString( 1923.433f ) );
        m_highScore = FindObjectOfType<HighScore>();
        m_soundManager = FindObjectOfType<SoundManager>();
        findTrack();

        if( m_countdownText == null ) {
            throw new UnityException( "Must have countdown text for race!" );
        }
    }

    public void findTrack() {
        m_track = FindObjectOfType<Track>();
    }

	public void startRace() {
        m_time = 0.0f;
        StartCoroutine( startRaceCor() );
	}

    IEnumerator techRiderUnlockCor() {
        bool unlock = PlayerPrefs.GetInt( "tech-rider-unlocked" ) == 0;

        // unlock tech rider
        PlayerPrefs.SetInt( "tech-rider-unlocked", 1 );

        var text = FindObjectOfType<Text>();
        float m_timeLeft = 5.0f;
        while ( m_timeLeft > 0.0f ) {
            m_timeLeft -= Time.deltaTime;
            text.text = "Congratulations\n\n"
                + "You've completed all the courses\n\n";

            if ( unlock ) {
                text.text += "You unlocked a new ship:\n"
                    + "TECH RIDER\n\n";
            }

            text.text += "Try to improve your time!\n\n"
                + "Restarting in " + m_timeLeft.ToString( "0.00" ) + " seconds";

            yield return null;
        }
        Destroy( gameObject );
        SceneManager.LoadScene( "title" );
    }

    bool m_isLoading = false;
    void Update() {
        if( m_raceEnded ) {
            if( InputManager.leftDown || InputManager.rightDown ) {
            //if( Input.GetButtonDown( "Left" ) || Input.GetButtonDown( "Right" ) ) {
                m_highScore.hideTimes();

                if( track.nextTrack != null && track.nextTrack != "" ) {
                    m_timeText.text = "";
                    m_raceEnded = false;
                    SceneManager.LoadScene( track.nextTrack );
                    startRace();
                } else {

                    // HACK destroy persistent objects so they don't duplicate
                    foreach( var obj in FindObjectsOfType<Persistent>() ) {
                        if( obj.gameObject != gameObject ) Destroy( obj.gameObject );
                    }

                    SceneManager.LoadScene( "end" );

                    StartCoroutine( techRiderUnlockCor() );

                    // have to destroy our original camera AFTER duplicating for some reason
                    Destroy( Camera.main );
                }
            }
            return;
        }

        if( m_raceStarted ) {
            m_time += Time.deltaTime;
            m_timeText.text = getTimeString();
        }

        // HACK to get track because it doesn't go active until after us
        if ( m_track == null ) {
            m_track = FindObjectOfType<Track>();

            if ( m_track != null ) {
                m_highScore.trackId = m_track.id;
                m_highScore.loadTimes();
            }
        }
    }

    public void respawn() {
        m_raceStarted = false;
        StartCoroutine( startRaceCor( true ) );
    }

    IEnumerator startRaceCor( bool a_isRespawn = false ) {

        // activate appropriate player racer
        int shipCount = 0;
        do {
            shipCount = FindObjectsOfType<PlayerShip>().Length;
            yield return true;
        } while ( shipCount == 0 );

        // deactivate other ships
        // + have camera follow active ship
        //Debug.Log( "Player ship: " + m_playerShipName );
        //Debug.Log( "Ship count: " + FindObjectsOfType<PlayerShip>().Length );
        foreach( var playerShip in FindObjectsOfType<PlayerShip>() ) {
            //Debug.Log( "Ship: " + playerShip.ShipName + " vs " + m_playerShipName );
            if ( playerShip.ShipName == m_playerShipName ) {
                Camera.main.GetComponent<MatchPosition>().setTarget( playerShip.transform );
            } else {
                //Debug.Log( "Deactivate" );
                playerShip.gameObject.SetActive( false );
            }
        }

        // reset camera size
        //Camera.main.orthographicSize = FindObjectOfType<PlayerShip>().originalCamSize;

        // display READY? for a bit before countdown
        m_countdownText.color = Color.red;
        //m_countdownText.text = "READY?";
        m_countdownText.text = FindObjectOfType<GameManager>().track.name;
        yield return new WaitForSeconds( a_isRespawn ? m_respawnTime : m_readyTime );

        int countdown = a_isRespawn ? 0 : m_countdownTime;
        int fontSize = m_countdownText.fontSize;

        while ( countdown > -1 ) {
            if ( countdown == 0 ) {
                m_countdownText.text = "GO!";
                m_countdownText.color = Color.green;
                m_raceStarted = true;

                m_soundManager.playSound( SoundManager.Sound.CountDownGo );

                if ( m_track && m_track.music && !m_track.music.isPlaying ) {
                    m_track.music.Play();
                }
            } else {
                m_countdownText.text = "" + countdown;

                if ( countdown > 1 ) {
                    m_countdownText.color = Color.red;
                } else {
                    m_countdownText.color = Color.yellow;
                }

                m_soundManager.playSound( SoundManager.Sound.CountDown );
            }

            // grow the countdown font
            var timeElapsed = 0.0f;
            while( timeElapsed < 1.0f ) {
                timeElapsed += Time.deltaTime;

                if ( countdown > 0 ) {
                    m_countdownText.fontSize = Mathf.CeilToInt( fontSize * (1.0f - timeElapsed ) );
                } else {
                    m_countdownText.fontSize = Mathf.CeilToInt( fontSize * timeElapsed );
                }
                yield return null;
            }

            --countdown;
        }

        m_countdownText.color = Color.clear;
    }

    public void endRace() {
        m_raceStarted = false;

        // destroy ships after a second
        foreach( var ship in FindObjectsOfType<Ship>() ) {
            Destroy( ship.gameObject, 1.0f );
        }

        Camera.main.GetComponent<MatchPosition>().setTarget( null );

        m_highScore.addTime( m_time, m_playerShipName );

        StartCoroutine( endRaceCor() );
    }

    public IEnumerator endRaceCor() {
        m_countdownText.color = Color.cyan;
        m_countdownText.text = "FINISH";

        int originalFontSize = m_countdownText.fontSize;
        float timeElapsed = 0.0f;

        // TODO add field for 2.0
        while( timeElapsed < 2.0f ) {
            timeElapsed += Time.deltaTime;
            var t = timeElapsed / 2.0f;

            m_countdownText.fontSize = Mathf.FloorToInt( ( 1.0f - t ) * originalFontSize );
            m_track.music.volume = 1.0f - t;

            yield return false;
        }

        m_countdownText.color = Color.clear;

        // reset font size
        m_countdownText.fontSize = originalFontSize;

        m_highScore.showTimes();

        // TODO make a field
        // delays input so we don't jump right to the next track
        m_timeText.text = "Please wait...";
        yield return new WaitForSeconds( 2.0f );
        m_timeText.text = "Press Left/A or Right/D to continue";
        m_raceEnded = true;
    }

    public static string timeToString( float a_seconds ) {
        var minutes = Mathf.FloorToInt( a_seconds ) / 60;
        var seconds = Mathf.FloorToInt( a_seconds - minutes * 60.0f );
        var fraction = a_seconds - ( ( minutes * 60.0f ) + seconds );
        return minutes.ToString( "00" ) + ":" + seconds.ToString( "00" ) + fraction.ToString( ".00");
    }

    private string getTimeString() {
        if ( m_time < 1.0f ) return null;
        return timeToString( m_time );
    }
}
