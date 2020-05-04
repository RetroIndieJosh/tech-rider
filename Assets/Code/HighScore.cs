using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScore : MonoBehaviour {
    [SerializeField]
    int m_trackId = 0;
    public int trackId {  set { m_trackId = value; } }

    [SerializeField]
    Text m_highScoreText = null;

    [SerializeField]
    AudioSource m_music = null;

    [SerializeField]
    GameObject m_fullscreenFade = null;

    const int MAX_NUM_SCORES = 10;

    float[] m_times = new float[MAX_NUM_SCORES];
    string[] m_ships = new string[MAX_NUM_SCORES];

    int m_bestTimeIndex = 0;
    int m_worstTimeIndex = 0;
    int m_mostRecentTimeIndex = 0;

    int m_timesCount = 0;

    float worstTime {  get { return m_times[m_worstTimeIndex]; } }
    float bestTime {  get { return m_times[m_bestTimeIndex]; } }

    public void addTime( float a_time, string a_shipName ) {
        if( m_timesCount == MAX_NUM_SCORES && a_time > worstTime ) {
            //Debug.Log( "Time not good enough for high score." );
            return;
        }

        if ( a_time < bestTime ) {
            //Debug.Log( "New record!" );
        }

        // replace the worst time with this one
        if( m_timesCount == MAX_NUM_SCORES ) {
            m_mostRecentTimeIndex = m_worstTimeIndex;

        // add to the end
        } else {
            m_mostRecentTimeIndex = m_timesCount;
        }

        string key = getTimeKey( m_mostRecentTimeIndex );
        string shipKey = getShipKey( m_mostRecentTimeIndex );

        //Debug.Log( "Setting '" + key + "' to " + a_time + " (" + a_shipName + ")" );
        PlayerPrefs.SetFloat( key, a_time );
        PlayerPrefs.SetString( shipKey, a_shipName );

        // reload times so we're synced
        loadTimes();
    }

    string baseKey { get { return "track" + m_trackId + "_"; } }

    string getShipKey( int a_index ) {
        return baseKey + "ship-name" + a_index;
    }

    string getTimeKey( int a_index ) {
        return baseKey + "time" + a_index;
    }

    private string getShip( int a_index ) {
        return m_ships[a_index];
    }

    private string getShipForTime( float a_time ) {
        for( int i = 0; i < MAX_NUM_SCORES; ++i ) {
            if ( m_times[i] == a_time ) return m_ships[i];
        }
        return null;
    }

    private string getTime( int a_index ) {
        var time = m_times[a_index];
        if ( time > -1 ) return GameManager.timeToString( time );
        return null;
    }

    public void loadTimes() {
        float bestTime = float.MaxValue;
        float worstTime = 0;

        m_timesCount = 0;

        for( int i = 0; i < MAX_NUM_SCORES; ++i ) {
            string key = getTimeKey( i );
            if ( PlayerPrefs.HasKey( key ) ) {
                ++m_timesCount;
                m_times[i] = PlayerPrefs.GetFloat( key );

                if( m_times[i] > worstTime ) {
                    m_worstTimeIndex = i;
                    worstTime = m_times[i];
                }

                if( m_times[i] < bestTime ) {
                    m_bestTimeIndex = i;
                    bestTime = m_times[i];
                }

                string shipKey = getShipKey( i );
                m_ships[i] = PlayerPrefs.GetString( shipKey );
            } else {
                m_times[i] = -1;
                m_ships[i] = null;
            }

            //Debug.Log( "Time #" + i + ": " + getTime( i ) + " (" + getShip( i ) + ")" );
        }

        //Debug.Log( "Best: #" + m_bestTimeIndex + " " + this.bestTime );
        //Debug.Log( "Worst: #" + m_worstTimeIndex + " " + this.worstTime );
    }

    public void hideTimes() {
        m_highScoreText.color = Color.clear;
        m_highScoreText.text = "";
        if ( m_fullscreenFade ) m_fullscreenFade.SetActive( false );
        if ( m_music ) m_music.Stop();
    }

    public void showTimes() {
        m_highScoreText.text = "BEST TIMES\n\n";

        if( bestTime == m_times[m_mostRecentTimeIndex] ) {
            m_highScoreText.text += "[new best time!]\n\n";
        }

        if ( m_fullscreenFade ) m_fullscreenFade.SetActive( true );

        // TODO sort scores
        var list = new ArrayList();
        list.AddRange( m_times );
        list.Sort();
        //list.Reverse();

        int current = 1;
        foreach( var timeObj in list ) {
            var time = (float)timeObj;
            if ( time == -1 ) continue;

            if( time == m_times[m_mostRecentTimeIndex] ) {
                m_highScoreText.text += "*** ";
            }

            var timeStr = GameManager.timeToString( time );
            var shipStr = "(" + getShipForTime( time ) + ")";
            m_highScoreText.text += "" + current + ". " + timeStr
                + " " + shipStr;

            if( time == m_times[m_mostRecentTimeIndex] ) {
                m_highScoreText.text += " ***";
            }

            m_highScoreText.text += "\n\n";

            ++current;
        }

        // make visible
        m_highScoreText.color = Color.white;

        if( m_music != null ) {
            m_music.Play();
        }
    }

}
