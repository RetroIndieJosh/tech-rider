using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
    public enum Sound {
        CountDown,
        CountDownGo,
        ShipCrash,
        ShipDeath,
        ShipSelect
    }

    [SerializeField]
    AudioClip m_countdownSound = null;

    [SerializeField]
    AudioClip m_goSound = null;

    [SerializeField]
    AudioClip m_shipCrashSound = null;

    [SerializeField]
    AudioClip m_shipDeathSound = null;

    [SerializeField]
    AudioClip m_shipSelectSound = null;

    AudioClip getSound( Sound a_sound ) {
        switch( a_sound ) {
            case Sound.CountDown: return m_countdownSound;
            case Sound.CountDownGo: return m_goSound;
            case Sound.ShipCrash: return m_shipCrashSound;
            case Sound.ShipDeath: return m_shipDeathSound;
            case Sound.ShipSelect: return m_shipSelectSound;
            default: return null;
        }
    }

    public AudioSource playSound( Sound a_sound ) {
        return playSound( a_sound, Camera.main.transform.position );
    }

    int m_soundINstanceCount = 0;
    public AudioSource playSound( Sound a_sound, Vector2 a_position ) {
        var temp = new GameObject( "snd_tempAudio" + m_soundINstanceCount );
        ++m_soundINstanceCount;

        temp.transform.position = a_position;

        var source = temp.AddComponent<AudioSource>();

        var clip = getSound( a_sound );
        if ( !clip ) return null;
        source.clip = clip;

        source.Play();

        Destroy( temp, clip.length );

        return source;
    }
}
