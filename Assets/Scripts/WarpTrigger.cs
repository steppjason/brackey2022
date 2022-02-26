using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTrigger : MonoBehaviour
{
	public GameObject _warp;

	public void TriggerWarp(){
		FindObjectOfType<WarpController>().WarpTo(_warp);
	}

	public void TriggerWarp(GameObject warp){
		FindObjectOfType<WarpController>().WarpTo(warp);
	}

}
