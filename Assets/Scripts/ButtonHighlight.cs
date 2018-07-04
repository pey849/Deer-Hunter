using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class ButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler// required interface when using the OnSelect method.
{
	//Do this when the selectable UI object is selected.
	public void OnSelect(BaseEventData eventData)
	{
		GetComponent<Image> ().color = Color.white;
	}

	//Do this when the selectable UI object is selected.
	public void OnDeselect(BaseEventData eventData)
	{
		GetComponent<Image> ().color = Color.gray;
	}
}