using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        instance = GetComponent<AudioManager>();

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        PlaySound("BGM");
    }

    public void PlaySound(string name)
    {
        Sound find = System.Array.Find(sounds, sound => sound.name == name);
        if (find == null)
        {
            Debug.LogWarning("Sound name " + name + " has not found.");
            return;
        }
        find.source.Play();
    }

    public void StopSound(string name)
    {
        Sound find = System.Array.Find(sounds, sound => sound.name == name);
        if (find == null)
        {
            Debug.LogWarning("Sound name " + name + " has not found.");
            return;
        }
        find.source.Stop();
    }
}
