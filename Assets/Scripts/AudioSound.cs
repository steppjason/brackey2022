using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class AudioSound
{
	public string name;
	public AudioClip soundClip;
	
	[Range(0f, 1f)]
	public float volume;
	
	[Range(0.1f, 3f)]
	public float pitch;

	public bool looping;

	[HideInInspector]
	public AudioSource source;
}
