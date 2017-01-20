using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    private int col;
    public Sprite[] colors;

    private Image img;

    // Use this for initialization
    private void Start()
    {
        img = gameObject.GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            col++;
            if (col >= colors.Length)
                col = 0;
            img.sprite = colors[col];
        }
    }
}