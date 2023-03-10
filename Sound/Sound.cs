using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Sound")]
public class Sound : ScriptableObject {
    public AudioClip Clip;
    [Space]
    [SerializeField] private SoundType.Type _type;
    [HideInInspector] public  AudioSource Source;
    [Space] 
    [SerializeField] private bool _repeatingWithTimer;
    [SerializeField] private float _timer;
    [SerializeField] private bool _loop;
    [Space]
    [SerializeField][Range(0f,100f)] private float _volume = 100;
    [SerializeField][Range(0.1f,3f)] private float _pitch = 1;
    [SerializeField][Range(-1f,1f)] private float _stereoPan = 0;
    [SerializeField][Range(0f,1f)] private float _spatialBlend = 1;
    public float Volume => _volume/100;
    public float Pitch => _pitch;
    public float StereoPan => _stereoPan;
    public bool Loop =>_loop;
    public bool RepeatingWithTimer => _repeatingWithTimer;
    public float Timer => _timer;
    public float SpatialBlend => _spatialBlend;
    public SoundType.Type Type => _type;
}
