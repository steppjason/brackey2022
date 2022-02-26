using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    void Start()
    {
		StartCoroutine(StartBattle());
	}

	IEnumerator StartBattle(){
		yield return new WaitForSeconds(30f);
		transform.position = FindObjectOfType<PlayerController>().transform.position;
	}
}
