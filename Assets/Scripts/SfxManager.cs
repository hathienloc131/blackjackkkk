using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{

    public AudioSource Audio;
    public AudioClip Click, Winning, Losing, Draw, Shuffe;

    public static SfxManager sfxInstance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        if (sfxInstance != null && sfxInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        sfxInstance = this;
        DontDestroyOnLoad(this);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
