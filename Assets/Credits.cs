using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{

	void Start()
    {
		FindObjectOfType<GameController>().StartCredits();
	}

}
