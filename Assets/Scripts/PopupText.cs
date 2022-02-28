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

    private SceneLoader loader;

    private enum PeacefulLines { None, Prologue, Sanctuary, Healing, Buying, Exiting };

    private PeacefulLines currentLine;
    private bool hasLeftSanctuary = false;

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

        loader = FindObjectOfType<SceneLoader>();

        if (loader == null)
        {
            loader = Instantiate(Resources.Load<GameObject>("Scene Manager"), Vector2.zero, Quaternion.identity).GetComponent<SceneLoader>();
        }

        if (!loader.hasAlreadyPlayed)
        {
            showPeaceful(PeacefulLines.Prologue);
        }
        else
        {
            showPeaceful(PeacefulLines.Sanctuary);
        }
    }

    private void Update()
    {
        float pos = player.transform.position.x;

        if (pos >= 0)
        {
            hasLeftSanctuary = true;
            hidePeaceful();

            if (!hasLeftSanctuary && !currentLine.Equals(PeacefulLines.Exiting))
            {
                showPeaceful(PeacefulLines.Exiting);
            }
        }
        else if (pos >= -13 && pos < 0)
        {
            if (!hasLeftSanctuary && !currentLine.Equals(PeacefulLines.Exiting))
            {
                showPeaceful(PeacefulLines.Exiting);
            }
        }
        else
        {
            if (hasLeftSanctuary && !currentLine.Equals(PeacefulLines.Sanctuary))
            {
                showPeaceful(PeacefulLines.Sanctuary);
            }
        }

        tryShowViolent(player.killCount);
    }

    private void tryShowViolent(int kills)
    {
        if (!popups.Contains(kills) && (kills == 10 || kills == 30 || kills == 60 || kills == 100))
        {
            showViolent(kills);
            popups.Add(kills);
        }
    }

    private void showViolent(int line)
    {
        string main = "";
        string secondary = "";

        if (line == 10)
        {
            main = "10 unholy beasts slaughtered!";
            secondary = "Truth of suffering acknowledged";
        }
        else if (line == 30)
        {
            main = "30 fell creatures returned to dust!";
            secondary = "Cause of suffering revealed";
        }
        else if (line == 60)
        {
            main = "60 abhorrent monsters crushed!";
            secondary = "End to suffering in sight";
        }
        else if (line == 100)
        {
            main = "100 wretched beings eviscerated!";
            secondary = "Path from suffering found";
        }

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

    private void showPeaceful(PeacefulLines line)
    {
        currentLine = line;

        string main = "";

        switch (line)
        {
            case PeacefulLines.Prologue:
                main = "Let us speak now of hatred and suffering. " +
                    "The dark whisperings that spring forth in idle minds, and take hold. " +
                    "Send them back, howling from whence they came! Call forth your vehemence, and stain your soul. " +
                    "In time you will wash them all away to amnesia.";
                break;
            case PeacefulLines.Sanctuary:
                hasLeftSanctuary = false;
                main = "Breathe deep, small one. You are safe. You are warm. " +
                    "The beasts cannot harm you here. Sit, sleep, and let forgetfulness take you.";
                break;
            case PeacefulLines.Healing:
                main = "Wash the blood from your wounds, and the pain from your mind. " +
                    "Let your sins flow away to afterthought.";
                break;
            case PeacefulLines.Buying:
                main = "My gift of sleep and your gift of death are not so different. " +
                    "Choose wisely, and bring swift justice to those horrid beasts.";
                break;
            case PeacefulLines.Exiting:
                main = "Be well, small one. Dark horrors await you beyond my reach. " +
                    "As always, I will await your end.";
                break;
        }

        peaceful.SetActive(true);

        peacefulMain.text = main;
        peacefulMainDup.generate();

        //Invoke("hidePeaceful", main.Length / 10f);
    }

    private void hidePeaceful()
    {
        peaceful.SetActive(false);
    }
}
