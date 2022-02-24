using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    public GameObject violent, peaceful;

    private TextAnimator violentMain;
    private TMP_Text violentSecondary, peacefulMain;
    private TextDuplicator violentMainDup, violentSecondaryDup, peacefulMainDup;

    private PlayerController player;
    private List<int> popups;

    private void Start()
    {
        violentMain = violent.GetComponentsInChildren<TextAnimator>()[0];
        violentSecondary = violent.GetComponentsInChildren<TMP_Text>()[1];
        peacefulMain = peaceful.GetComponentInChildren<TMP_Text>();

        violentMainDup = violentMain.GetComponent<TextDuplicator>();
        violentSecondaryDup = violentSecondary.GetComponent<TextDuplicator>();
        peacefulMainDup = peacefulMain.GetComponent<TextDuplicator>();

        player = FindObjectOfType<PlayerController>();
        popups = new List<int>();
    }

    private void Update()
    {
        int temp = player.killCount;
        string main = "";
        string secondary = "";

        if (temp == 10)
        {
            main = "10 unholy beasts slaughtered!";
            secondary = "A taste of blood";
        }
        else if (temp == 30)
        {
            print(30);
            main = "30 glorious monster deaths!";
            secondary = "Rising prowess";
        }
        else if (temp == 50)
        {
            print(50);
            main = "50 unspeakable horrors banished!";
            secondary = "Master of carnage";
        }

        if (!main.Equals(""))
        {
            tryShowViolent(temp, main, secondary);
        }
    }

    private void tryShowViolent(int kills, string main, string secondary)
    {
        if (!popups.Contains(kills))
        {
            showViolent(main, secondary);
            popups.Add(kills);
        }
    }

    private void showViolent(string main, string secondary)
    {
        violent.SetActive(true);

        violentMain.SetText("<shake a=0.3>" + main + "</shake>", false);
        violentMainDup.generate();

        violentSecondary.text = "--- " + secondary + " ---";
        violentSecondaryDup.generate();

        Invoke("hideViolent", (main.Length + secondary.Length) / 10f);
    }

    private void hideViolent()
    {
        violent.SetActive(false);
    }

    private void showPeaceful(string main)
    {
        peaceful.SetActive(true);

        peacefulMain.text = main;
        peacefulMainDup.generate();

        Invoke("hidePeaceful", main.Length / 10f);
    }

    private void hidePeaceful()
    {
        peaceful.SetActive(false);
    }
}
