using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	// Start is called before the first frame update
	public Dialogue diaogue;

	public void TriggerDialogue(){
		FindObjectOfType<DialogueController>().BeginDialogue(diaogue);
	}
}
