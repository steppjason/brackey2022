using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{

	[SerializeField] private PlayerController _player;

	private Queue<string> _lines;
	private bool _isActive;
	private float _textSpeed;
	private bool nextLine = false;

	public TMP_Text nameText;
	public TMP_Text dialogueText;
	public float textSpeed;




	[SerializeField] public Animator animator;

	// Start is called before the first frame update
	void Start()
    {
		_lines = new Queue<string>();
	}

	private void Update() {
		if(_isActive){
			if(nextLine && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))){
				DisplayNextSentence();
			} else if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)){
				_textSpeed = 0.0001f;
			}
		}
	}


	public void BeginDialogue(Dialogue dialogue){

		_player.action = false;
		_player.canMove = false;
		_isActive = true;

		nameText.text = dialogue.name;
		dialogueText.text = "";
		_lines.Clear();

		foreach(string line in dialogue.lines){
			_lines.Enqueue(line);
		}

		animator.SetBool("isShowing", true);
		StartCoroutine(WaitForBoxEnter(0.75f));
	}

	public void DisplayNextSentence(){
		nextLine = false;
		dialogueText.text = "";
		_textSpeed = 0.05f;

		if(_lines.Count <= 0){
			EndDialogue();
			return;
		}

		string line = _lines.Dequeue();
		Debug.Log(line);
		Debug.Log(_lines);

		StopAllCoroutines();
		StartCoroutine(DisplayLine(line));
	}

	IEnumerator WaitForBoxEnter(float seconds){
		yield return new WaitForSeconds(seconds);
		DisplayNextSentence();
	}

	IEnumerator WaitForBoxExit(float seconds){
		yield return new WaitForSeconds(seconds);
		_player.action = true;
	}

	IEnumerator DisplayLine(string line){
		
		foreach(char letter in line.ToCharArray()){
			dialogueText.text += letter;
			yield return new WaitForSeconds(_textSpeed);
		}

		nextLine = true;
	}

	void EndDialogue(){
		_isActive = false;
		_player.canMove = true;
		animator.SetBool("isShowing", false);
		StartCoroutine(WaitForBoxExit(0.75f));
	}

}
