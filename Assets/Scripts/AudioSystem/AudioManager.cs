using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utility.Singleton;

namespace Utility
{
    public class AudioManager : SingletonMono<AudioManager>
    {
        public AudioSource bgmSource; 
        public AudioSource sFXSource;
        
        [SerializeField] private List<AudioClip> bgmClip;
        [SerializeField] private AudioClip winClip;
        
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            PlayRandomBGM();
        }
        
        public void PlayRandomBGM()
        {
            PlayBGM(bgmClip[UnityEngine.Random.Range(0, bgmClip.Count)]);
        }
        
        private void PlayBGM(AudioClip clip)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        
        public void PlayWinClip()
        {
            PlayAudio(winClip);
        }
        
        public void PlayAudio(AudioClip clip)
        {
            sFXSource.PlayOneShot(clip);
        }
    }
}