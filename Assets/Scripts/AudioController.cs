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

	public void FadeIn(string name, float fadeTime, float volume){
		AudioSound sfx = Array.Find(audioClips, audioClip => audioClip.name == name);
		sfx.volume = 0f;
		sfx.source.Play();
		StartCoroutine(DoFadeIn(sfx.source, fadeTime, volume));
	}

	public void FadeOut(string name, float fadeTime, float volume){
		AudioSound sfx = Array.Find(audioClips, audioClip => audioClip.name == name);
		sfx.volume = 0f;
		StartCoroutine(DoFadeOut(sfx.source, fadeTime, volume));
	}
	

	public static IEnumerator DoFadeIn(AudioSource audioSource, float duration, float targetVolume)
    {	
		audioSource.Play();
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        yield break;
    }

	public static IEnumerator DoFadeOut(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
		audioSource.Stop();
		yield break;
    }
    
}