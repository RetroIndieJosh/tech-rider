using UnityEngine;
using System.Collections;

public class Track : MonoBehaviour {
    [SerializeField]
    AudioSource m_music = null;
    public AudioSource music {  get { return m_music; } }

    [SerializeField]
    PathNode m_firstNode = null;
    public PathNode firstNode {  get { return m_firstNode; } }

    [SerializeField]
    int m_id = 0;
    public int id {  get { return m_id; } }

    [SerializeField]
    string m_nextTrack = null;
    public string nextTrack {  get { return m_nextTrack; } }

    [SerializeField]
    string m_name = null;
    public string name {  get { return m_name; } }
}
