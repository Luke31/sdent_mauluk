using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitText : MonoBehaviour
{
	public LevelBuilder LevelBuilder;
	public GameObject InitTextCanvas;
	public Text UiInitText;
	private int _level;

	// Use this for initialization
	void Start ()
	{
		_level = LevelBuilder.GetLevel();
		if (_level == 1)
		{
			InitTextCanvas.SetActive(true);
			StartCoroutine(AnimateText("Hello?... \nWhere am I?... \nIt is dark in here" +
									   "|My body feels strange... \nAn aching pain in my left wing...\nAs if something has been implanted in my body *cluck*" +
									   "|CLUCK!...\nThere's even a hook under my wing!" +
			                           "|I need to escape from this room! " +
			                           "|Maybe there's a loose block in one of the walls..."));
			//... is a pause
			//| new screen
		}
		else
		{
			InitTextCanvas.SetActive(false);
		}
	}

	IEnumerator AnimateText(string strComplete)
	{
		int i = 0;
		UiInitText.text = "";
		while (i < strComplete.Length)
		{
			if ('|'.Equals(strComplete[i]))
			{
				yield return new WaitForSeconds(1F);
				UiInitText.text = "";
				i++;
			}
			if (i > 3 && "...".Equals(strComplete.Substring(i-3, 3)))
			{
				yield return new WaitForSeconds(0.5F);
			}
			UiInitText.text += strComplete[i++];
			yield return new WaitForSeconds(0.05F);
		}
		yield return new WaitForSeconds(2F);
		UiInitText.text = "";
	}

	// Update is called once per frame
	void Update () {
		
	}
}
