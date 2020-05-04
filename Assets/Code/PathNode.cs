using UnityEngine;
using System.Collections;

public class PathNode : MonoBehaviour {
    [SerializeField]
    private PathNode[] m_nextNode = null;

    public PathNode nextNode {
        get {
            var index = Random.Range( 0, m_nextNode.Length );
            Debug.Log( "Node #" + index );
            return m_nextNode[index];
        }
    } 

    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere( transform.position, 0.2f );

        if ( m_nextNode == null ) return;

        foreach ( var node in m_nextNode ) {
            if ( node == null ) continue;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine( transform.position, node.transform.position );
        }
    }
}
