using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour {
    //[SerializeField]
    //[Range( 1, 10)]
    //int m_multiplier = 4;

    [SerializeField]
    GameObject m_titleObj = null;

    [SerializeField]
    float m_distance = 1.0f;

    [SerializeField]
    [Tooltip( "Time in seconds" )]
    float m_timeToAnimate = 10.0f;

    [SerializeField]
    GameObject m_enableOnFinish = null;

    Vector2 m_targetPos = Vector2.zero;
    float m_speed = 0.0f;

    GameObject[] m_instances = null;

	void Start () {
        m_speed = m_distance / m_timeToAnimate;
        
        // TOOD change 4 to m_multiplier
        m_instances = new GameObject[4];
        for( var i = 0; i < 4; ++i ) {
            m_instances[i] = Instantiate( m_titleObj );

            var color = m_instances[i].GetComponent<Renderer>().material.color;
            // TODO replace 4
            color.a = 1.0f / 4.0f;
            m_instances[i].GetComponent<Renderer>().material.color = color;
        }

        // destroy original
        m_targetPos = m_titleObj.transform.position;
        Destroy( m_titleObj );

        // TODO radial movement
        m_instances[0].transform.position += Vector3.up * m_distance;
        m_instances[1].transform.position += Vector3.down * m_distance;
        m_instances[2].transform.position += Vector3.right * m_distance;
        m_instances[3].transform.position += Vector3.left * m_distance;

        if ( m_enableOnFinish ) m_enableOnFinish.SetActive( false );
	}

    float m_timeElapsed = 0.0f;
	void Update () {
        if ( m_timeElapsed >= m_timeToAnimate ) {
            if ( m_enableOnFinish ) m_enableOnFinish.SetActive( true );
            Destroy( gameObject );
        }

        // if we get input, skip animation
        if( InputManager.leftDown || InputManager.rightDown ) {
        //if( Input.GetButtonDown( "Left" ) || Input.GetButtonDown( "Right" ) ) {
            foreach( var instance in m_instances ) {
                instance.transform.position = m_targetPos;
            }
            m_timeElapsed = m_timeToAnimate;
            return;
        }

        // TODO move *toward* center (hacked to be opposite direction for now)
        m_instances[0].transform.position += Vector3.down * m_speed * Time.deltaTime;
        m_instances[1].transform.position += Vector3.up * m_speed * Time.deltaTime;
        m_instances[2].transform.position += Vector3.left * m_speed * Time.deltaTime;
        m_instances[3].transform.position += Vector3.right * m_speed * Time.deltaTime;

        m_timeElapsed += Time.deltaTime;
	}
}
