using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{

	public AudioSound[] audioClips;

	void Awake()
	{
		foreach (AudioSound audioClip in audioClips)
		{
			audioClip.source = gameObject.AddComponent<AudioSource>();
			audioClip.source.clip = audioClip.soundClip;

			audioClip.source.volume = audioClip.volume;
			audioClip.source.pitch = audioClip.pitch;
			audioClip.source.loop = audioClip.looping;
		}
	}

	public void Play(string name){
		AudioSound sfx = Array.Find(audioClips, audioClip => audioClip.name == name);
		sfx.source.Play();
	}

	public void Stop(string name){
		AudioSound sfx = Array.Find(audioClips, audioClip => audioClip.name == name);
		sfx.source.Stop();
	}
}