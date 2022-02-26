using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Croc : MonoBehaviour
{
	[SerializeField] private SpriteRenderer _sprite;
	[SerializeField] private float _rotationSpeed;

	private float _random;
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		_random = Random.Range(1, 65000);

		if(_random < 32500)
			_sprite.flipX = true;
		else
			_sprite.flipX = false;

		transform.Rotate(0, 0, (transform.rotation.z + _rotationSpeed) * Time.deltaTime, Space.Self);
	}
}
