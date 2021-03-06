﻿using UnityEngine;
using System.Collections;

public class InteractorScript : MonoBehaviour {
	public float range = 10f;
	public bool interacting = false;	

	public void Interact(){
		//if !interacting
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, range, GlobalScript.InteractableLayerMask)) {
			InteractableScript interactable = hit.transform.GetComponent<InteractableScript> ();
			if(interactable != null)
				interactable.Interact (this.gameObject);
		}
//		} else if (Physics.Raycast (transform.position, transform.up, out hit, range, GlobalScript.InteractableLayerMask)) {
//			hit.transform.GetComponent<InteractableScript> ().Interact (this.gameObject);
//		}

		Debug.DrawRay(transform.position, transform.forward, Color.red);
	}
}
