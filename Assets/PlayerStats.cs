using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public GameObject[] fxPerLine;
    public TMP_Text pointsText;
    public TMP_Text linesAmount;
    public TMP_Text comboText;

    [HideInInspector]
    public int Combo = 0;

    [HideInInspector]
    public int Points = 0;

    [HideInInspector]
    public int Lines = 0;

    // Start is called before the first frame update
    private void Start()
    {
        GetComponentInChildren<Grid>().LinesChecked.AddListener(HandlePoints);
        GameManager.Instance.SetPoints(Points);
        linesAmount.text = Lines.ToString();
        pointsText.text = Points.ToString();
        comboText.text = Mathf.Max(0, Combo - 1).ToString();
    }

    private void HandlePoints(int linesDestroyed)
    {
        if (linesDestroyed == 0)
        {
            Combo = 0;
        }
        else
        {
            Combo++;
            Lines += linesDestroyed;
            var pointsPerLine = new int[] { 40, 100, 300, 1200 };
            var index = Mathf.Clamp(linesDestroyed - 1, 0, 4);
            Points += pointsPerLine[index] + 50 * (Combo - 1);
            fxPerLine[index].SetActive(true);
            if (linesDestroyed >= 4)
            {
                AudioManager.Instance.Play("Tetris - Button");
            }
            else
            {
                AudioManager.Instance.Play("LineClear");
            }

            linesAmount.text = Lines.ToString();
            pointsText.text = Points.ToString();
            GameManager.Instance.SetPoints(Points);
        }
        comboText.text = Mathf.Max(0, Combo - 1).ToString();
    }
}