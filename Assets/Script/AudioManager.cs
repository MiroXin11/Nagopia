using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class AudioManager : SingletonMonobehaviour<AudioManager> {

        public void PlayAudioClip(string name,bool loop=false) {
            if(!audiosCache.TryGetValue(name,out AudioClip clip)){
                clip=GameDataBase.GetAudioClip(name);
                audiosCache.Add(name, clip);
            }
            sourcer.clip = clip;
            sourcer.loop = loop;
            sourcer.Play();
        }

        public void PlayOnce(string name) {
            if (!audiosCache.TryGetValue(name, out AudioClip clip)) {
                clip = GameDataBase.GetAudioClip(name);
                audiosCache.Add(name, clip);
            }
            sourcer.PlayOneShot(clip);
        }

        public AudioSource sourcer;

        private Dictionary<string,AudioClip>audiosCache=new Dictionary<string,AudioClip>();
    }
}