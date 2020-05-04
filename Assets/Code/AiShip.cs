using UnityEngine;
using System.Collections;

public class AiShip : Ship {
    [SerializeField]
    PathNode m_firstNode = null;

    PathNode m_nextNode = null;

    // TODO override Awake throw exception if nextNode is null
    protected override void Awake() {
        if( m_firstNode == null ) {
            throw new UnityException( "AI Ship must have first node to target." );
        }

        base.Awake();
    }

    public void resetPath( PathNode m_node ) {
        m_nextNode = m_node;
    }

	protected override void Update () {
        if ( m_nextNode == null ) {
            m_nextNode = m_firstNode;
        }

        if( m_nextNode == null ) {
            return;
        }

        var dir = m_nextNode.transform.position - transform.position;
        var targetAngle = Mathf.Atan2( dir.y, dir.x ) * Mathf.Rad2Deg - 90.0f;
        //transform.rotation = Quaternion.AngleAxis( targetAngle, Vector3.forward );
        setRotation( targetAngle );

        //var adjustedAngle = ( targetAngle >= 180 ) ? 180 - targetAngle : targetAngle;
        //var rotZ = transform.rotation.eulerAngles.z;
        //var currentAngle = ( rotZ >= 180 ) ? 360 - rotZ : rotZ;

        //var diff = targetAngle - rotZ;
        //Debug.Log( "diff " + diff );
        //Debug.Log( "Target: " + targetAngle + " | Current: " + rotZ );
        //+ " | Adjusted: " + adjustedAngle + " | Diff: " + diff + " Target: " + m_nextNode.name );

        /*
        var nodePos = m_nextNode.transform.position;
        var myPos = transform.position;
        var diff = myPos - myPos * 100.0f;
        var perp = new Vector2 ( -diff.y, diff.x );
        var d = Vector2.Dot( nodePos - myPos, perp );

        Debug.Log( "D: " + d + " | Target: " + m_nextNode.name );

        if( d > rotateSpeed ) {
            //moveLeft();
        } else if( d < rotateSpeed ) {
            //moveRight();
        } else {
            //stopStrafe();
        }
        */

        if( transform.position.y >= m_nextNode.transform.position.y ) {
            m_nextNode = m_nextNode.nextNode;
        }

        base.Update();
	}
}
