using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour
{

	public  float _speed = 1f; 

	// Update is called once per frame
	void Update()
	{
		transform.position += new Vector3(1, -0.25f, 0) * _speed * Time.deltaTime;
	}

}