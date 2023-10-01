using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textHolder;

    private void Start()
    {
        if (GameManager.Instance)
        {
            float timeInSeconds = GameManager.Instance.finalTime;
            int minutes = (int)(timeInSeconds / 60);
            int seconds = (int)(timeInSeconds % 60);

            textHolder.text = "FINAL TIME: " + string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }
        
    }
}
