using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpController : MonoBehaviour
{
	public Fader _fader;
	public event Action<GameObject> OnWarpEnter;
	public event Action OnWarpExit;


	public void WarpTo(GameObject warp){
		//StartCoroutine(StartWarp(warp));
		OnWarpEnter(warp);
	}

	IEnumerator StartWarp(GameObject warp){
		StartCoroutine(_fader.FadeIn(1f));
		yield return new WaitForSeconds(1f);
		OnWarpEnter(warp);		
		StartCoroutine(_fader.FadeOut(1f));
		yield return new WaitForSeconds(1f);
	}
}
