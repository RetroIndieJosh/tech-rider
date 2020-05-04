using UnityEngine;
using System.Collections;

public class MatchPosition : MonoBehaviour {
    [SerializeField]
    Transform m_target;

    [SerializeField]
    Vector2 m_offset = Vector2.zero;

    public Transform target {  get { return m_target; } }

    public void setTarget( Transform a_target ) {
        m_target = a_target;
    }

	void Update () {
        if ( m_target == null ) return;

        var pos = transform.position;
        pos.x = m_target.position.x + m_offset.x;
        pos.y = m_target.position.y + m_offset.y;
        transform.position = pos;
	}
}
