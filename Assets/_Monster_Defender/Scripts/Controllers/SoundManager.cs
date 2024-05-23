﻿using UnityEngine;
using System.Collections;

namespace ClawbearGames
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Source References")]
        [SerializeField] private AudioSource soundSource = null;
        [SerializeField] private AudioSource musicSource = null;


        [Header("Audio Clips References")]
        public AudioClip Button = null;
        public AudioClip CoinItem = null;
        public AudioClip MergeTank = null;
        public AudioClip BulletExplode = null;
        public AudioClip EnemyExplode = null;
        public AudioClip BossExplode = null;
        public AudioClip LevelCompleted = null;
        public AudioClip LevelFailed = null;


        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }



        private void Start()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKey.SOUND_KEY))
                PlayerPrefs.SetFloat(PlayerPrefsKey.SOUND_KEY, 1f);
            if (PlayerPrefs.HasKey(PlayerPrefsKey.MUSIC_KEY))
                PlayerPrefs.SetFloat(PlayerPrefsKey.MUSIC_KEY, 1f);

            soundSource.volume = PlayerPrefs.GetFloat(PlayerPrefsKey.SOUND_KEY, 1f);
            musicSource.volume = PlayerPrefs.GetFloat(PlayerPrefsKey.MUSIC_KEY, 1f);
        }


        /// <summary>
        /// Update the volume of the sound source.
        /// </summary>
        /// <param name="value"></param>
        public void SetSoundVolume(float value)
        {
            soundSource.volume = value;
            PlayerPrefs.SetFloat(PlayerPrefsKey.SOUND_KEY, value);
        }


        /// <summary>
        /// Update the volume of the music source.
        /// </summary>
        /// <param name="value"></param>
        public void SetMusicVolume(float value)
        {
            soundSource.volume = value;
            PlayerPrefs.SetFloat(PlayerPrefsKey.MUSIC_KEY, value);
        }


        /// <summary>
        /// Play one audio clip as sound.
        /// </summary>
        /// <param name="audioClip"></param>
        public void PlaySound(AudioClip audioClip)
        {
            soundSource.PlayOneShot(audioClip);
        }


        /// <summary>
        /// Plays the given sound clip as music clip (automatically loop).
        /// </summary>
        /// <param name="audioClip"></param>
        public void PlayMusic(AudioClip audioClip)
        {
            musicSource.clip = audioClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
}
