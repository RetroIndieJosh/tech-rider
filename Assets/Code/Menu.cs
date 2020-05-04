using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour {
    [SerializeField]
    GameObject m_cursor = null;

    [System.Serializable]
    struct Target {
        public GameObject obj;
        public string label;
    }
    
    [SerializeField]
    Target[] m_targetList = null;

    [SerializeField]
    Text m_labelText = null;

    [SerializeField]
    Text m_instructionText = null;

    private int m_curIndex = 0;
    private int curIndex {
        get { return m_curIndex; }
        set {
            m_curIndex = value;
            bool inc = ( value > m_curIndex );

            // skip locked targets
            var count = 0;
            while ( true ) {
                if ( m_curIndex < 0 ) m_curIndex = m_targetList.Length - 1;
                if ( m_curIndex > m_targetList.Length - 1 ) m_curIndex = 0;

                ++count;
                if ( count > 1000 ) {
                    Debug.LogError( "Infinite loop detected" );
                    break;
                }
                Debug.Log( "Index: " + m_curIndex );

                var targetObj = m_targetList[m_curIndex].obj;
                var unlockable = targetObj.GetComponent<Unlockable>();
                if ( unlockable && unlockable.locked ) {
                    if ( inc ) ++m_curIndex;
                    else --m_curIndex;
                } else {
                    break;
                }
            }

            m_timeSinceChange = 0.0f;
            if ( m_labelText ) m_labelText.text = m_targetList[m_curIndex].label;
        }
    }

    // TODO make this more flexible
    private float m_timeSinceChange = 0.0f;
    private float m_timeToSelect = 3.0f;
    private float m_startDistance = 0.2f;
    private float m_distance = 0;

    private string m_selection = null;

    void positionCursor() {
        var t = m_timeSinceChange / m_timeToSelect * 0.5f;
        m_distance = m_startDistance - t * m_startDistance;
        m_cursor.transform.position = (Vector2)m_targetList[m_curIndex].obj.transform.position + Vector2.up * m_distance;
    }

    void Start() {

        // so we set everything up
        curIndex = m_curIndex;
    }

	void Update () {
        if ( m_targetList == null || m_targetList.Length == 0 ) return;

        // make selection
        m_timeSinceChange += Time.deltaTime;
        if( m_timeSinceChange >= m_timeToSelect ) {
            m_selection = m_targetList[m_curIndex].label;
            Debug.Log( "Selection: " + m_selection );

            // TODO make this more flexible
            var game = FindObjectOfType<GameManager>();
            game.playerShipName = m_selection;

            var source = game.soundManager.playSound( SoundManager.Sound.ShipSelect );
            DontDestroyOnLoad( source );

            SceneManager.LoadScene( game.trackSceneName );
            game.findTrack();
            game.startRace();

            // make us invisible
            GetComponent<Renderer>().material.color = Color.clear;

            // HACK destroy label text + instruction text
            if ( m_labelText != null ) Destroy( m_labelText );
            if ( m_instructionText ) Destroy( m_instructionText );

            // clear target list to stop updating
            m_targetList = null;
            return;
        }

        if ( InputManager.leftDown ) {
            //if ( Input.GetButtonDown( "Left" ) ) {
            --curIndex;
        } else if( InputManager.rightDown ) {
        //} else if ( Input.GetButtonDown( "Right" ) ) {
            ++curIndex;
        }

        positionCursor();
	}
}
