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
            secondary = "Truth of suffering acknowledged";
        }
        else if (temp == 30)
        {
            main = "30 fell creatures returned to dust!";
            secondary = "Cause of suffering revealed";
        }
        else if (temp == 60)
        {
            main = "60 abhorrent monsters crushed!";
            secondary = "End to suffering in sight";
        }
        else if (temp == 100)
        {
            main = "100 wretched beings eviscerated!";
            secondary = "Path from suffering found";
        }

        if (!main.Equals(""))
        {
            tryShowViolent(temp, main, secondary);
        }

        /*
        // Prologue
        showPeaceful(
            "Let us speak now of hatred and suffering. " +
            "The dark whisperings that spring forth in idle minds, and take hold. " +
            "Send them back, howling from whence they came! Call forth your vehemence, and stain your soul. " +
            "In time you will wash them all away to amnesia."
        );

        // When entering the sanctuary
        showPeaceful(
            "Breathe deep, small one. You are safe. You are warm. " +
            "The beasts cannot harm you here. Sit, sleep, and let forgetfulness take you."
        );


        // When healing
        showPeaceful(
            "Wash the blood from your wounds, and the pain from your mind. " +
            "Let your sins flow away to afterthought."
        );


        // When buying / equipping items
        showPeaceful(
            "My gift of sleep and your gift of death are not so different. " +
            "Choose wisely, and bring swift justice to those horrid beasts."
        );


        // When exiting the sanctuary
        showPeaceful(
            "Be well, small one. Dark horrors await you beyond my reach. " +
            "As always, I will await your end."
        );
        */
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
