using System.Collections.Generic;
using _Code.KMJ.UnitSystem.Sound;
using UnityEngine;
using UnityEngine.Analytics;

namespace Code.Core
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField] private List<SoundClip> clips;
        [SerializeField] private Stack<AudioSource> audioSources;  
        
        private Dictionary<string, AudioClip> _clipDictionary = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> _loopingClipDictionary = new Dictionary<string, AudioClip>();

        protected override void Awake()
        {
            base.Awake();
            
            foreach (var audio in clips)
            {
                if (audio.IsLooping && !_loopingClipDictionary.ContainsKey(audio.AudioName))
                {
                    _loopingClipDictionary.Add(audio.AudioName, audio.Clip);
                }
                else if(!_clipDictionary.ContainsKey(audio.AudioName))
                {
                    _clipDictionary.Add(audio.AudioName, audio.Clip);
                }
            }
        }


        public void PlayLooping(string name)
        {
            _clipDictionary.TryGetValue(name, out AudioClip clip);

            var _pool = audioSources.Pop();

            _pool.gameObject.SetActive(true);
            _pool.clip = clip;
            _pool.loop = false;
            
            _pool.Play();
        }

        public void PlayClip(string name)
        {
            
        }

        public void StopClip(string name)
        {
            
        }

        public void StopLoopingClip(string name)
        {
            
        }
    }
}