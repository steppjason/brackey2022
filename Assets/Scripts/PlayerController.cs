using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public bool action = false;
	public bool canMove = true;

	[SerializeField] private GameController _gameController;
	[SerializeField] private Camera _camera;
    [SerializeField] private float _moveSpeed = 5;
	[SerializeField] private GameObject _actionIcon;

	private GameObject _collider;
	private Vector2 _input;
	private Animator _animator;


	private void Awake() {
		_animator = GetComponent<Animator>();
	}


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		_camera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
		
		if(action && (Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl))){
			
			if(_collider.GetComponent<DialogueTrigger>())
				_collider.GetComponent<DialogueTrigger>().TriggerDialogue();

			
		}
	}

	private void FixedUpdate() {
		if (canMove)
		{
			_input.x = Input.GetAxisRaw("Horizontal");
			_input.y = Input.GetAxisRaw("Vertical");

			transform.position = transform.position +
						new Vector3(_input.x, _input.y, 0).normalized * _moveSpeed * Time.deltaTime;
		}

	}


	private void OnTriggerStay2D(Collider2D other) {
		_actionIcon.SetActive(true);
		_animator.SetBool("isShowing", true);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		action = true;
		_collider = other.gameObject;
	}

	private void OnTriggerExit2D(Collider2D other) {
		_animator.SetBool("isShowing", false);
		action = false;
		_collider = null;
	}

	private void HideActionIcon(){
		_actionIcon.SetActive(false);
	}
}
