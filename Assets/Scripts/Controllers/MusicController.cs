using UnityEngine;

using static Helpers;

public class MusicController : MonoBehaviour
{
    private AudioSource musicSource;

    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: inefficient, should use an audio manager script instead

        if (collision.CompareTag("Player"))
        {
            SwitchMusic(gameObject.name, musicSource);
        }
    }
}
