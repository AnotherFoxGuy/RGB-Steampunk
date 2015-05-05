using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AGC.Tools;

public class ChangeColor : MonoBehaviour 
{
	public Sprite[] colors;

	int col = 0;

	Image img;

	// Use this for initialization
	void Start () 
	{
		img = gameObject.GetComponentInChildren<Image>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.R)) 
		{
			col++;
			if(col >= colors.Length)
				col = 0;
			img.sprite = colors[col];
		}
	
	}
}
