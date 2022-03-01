using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    [Header("Violent")]
    public GameObject violent;
    public AudioClip combatPopup10, combatPopup30, combatPopup60, combatPopup100;

    [Header("Peaceful")]
    public GameObject peaceful;
    public AudioClip prologuePopup, sanctuaryPopup, healingPopup, buyingPopup, exitingPopup;

    private AudioSource audio;

    private TextAnimator violentMain;
    private TMP_Text violentSecondary, peacefulMain;
    private TextDuplicator violentMainDup, violentSecondaryDup, peacefulMainDup;

    private PlayerController player;
    private List<int> popups;

    private SceneLoader loader;

    private enum PeacefulLines { None, Prologue, Sanctuary, Healing, Buying, Exiting };

    private PeacefulLines currentPeacefulLine;
    private bool hasLeftSanctuary = false;

    private int currentViolentLine;

    private void Start()
    {
        audio = GetComponent<AudioSource>();

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
            if (!hasLeftSanctuary)
            {
                hasLeftSanctuary = true;
                hidePeaceful();
            }
        }
        else if (pos >= -13 && pos < 0)
        {
            if (!hasLeftSanctuary && !currentPeacefulLine.Equals(PeacefulLines.Exiting))
            {
                showPeaceful(PeacefulLines.Exiting);
            }
        }
        else
        {
            if (hasLeftSanctuary && !currentPeacefulLine.Equals(PeacefulLines.Sanctuary))
            {
                showPeaceful(PeacefulLines.Sanctuary);
            }
        }

        tryShowViolent(player.killCount);
    }

    private void tryShowViolent(int kills)
    {
        if (!popups.Contains(kills) && (
            (kills >= 10 && kills < 30 && currentViolentLine < 10) ||
            (kills >= 30 && kills < 60 && currentViolentLine < 30) ||
            (kills >= 60 && kills < 100 && currentViolentLine < 60) ||
            (kills >= 100 && currentViolentLine < 100)))
        {
            showViolent(kills);
            popups.Add(kills);
        }
    }

    private void showViolent(int line)
    {
        string main = "";
        string secondary = "";
        AudioClip sound = null;

        if (line == 10)
        {
            currentViolentLine = 10;
            main = "10 unholy beasts slaughtered!";
            secondary = "Truth of suffering acknowledged";
            sound = combatPopup10;
        }
        else if (line == 30)
        {
            currentViolentLine = 30;
            main = "30 fell creatures returned to dust!";
            secondary = "Cause of suffering revealed";
            sound = combatPopup30;
        }
        else if (line == 60)
        {
            currentViolentLine = 60;
            main = "60 abhorrent monsters crushed!";
            secondary = "End to suffering in sight";
            sound = combatPopup60;
        }
        else if (line == 100)
        {
            currentViolentLine = 100;
            main = "100 wretched beings eviscerated!";
            secondary = "Path from suffering found";
            sound = combatPopup100;
        }

        violent.SetActive(true);

        violentMain.SetText("<shake a=0.3>" + main + "</shake>", false);
        violentMainDup.generate();

        violentSecondary.text = "--- " + secondary + " ---";
        violentSecondaryDup.generate();

        audio.clip = sound;
        audio.volume = 1;
        audio.Play();

        Invoke("hideViolent", (main.Length + secondary.Length) / 10f);
    }

    private void hideViolent()
    {
        violent.SetActive(false);
        audio.Stop();
    }

    private void showPeaceful(PeacefulLines line)
    {
        currentPeacefulLine = line;

        string main = "";
        AudioClip sound = null;

        switch (line)
        {
            case PeacefulLines.Prologue:
                main = "Let us speak now of hatred and suffering. " +
                    "The dark whisperings that spring forth in idle minds, and take hold. " +
                    "Send them back, howling from whence they came! Call forth your vehemence, and stain your soul. " +
                    "In time you will wash them all away to amnesia.";
                sound = prologuePopup;
                break;
            case PeacefulLines.Sanctuary:
                hasLeftSanctuary = false;
                main = "Breathe deep, small one. You are safe. You are warm. " +
                    "The beasts cannot harm you here. Sit, sleep, and let forgetfulness take you.";
                sound = sanctuaryPopup;
                break;
            case PeacefulLines.Healing:
                main = "Wash the blood from your wounds, and the pain from your mind. " +
                    "Let your sins flow away to afterthought.";
                sound = healingPopup;
                break;
            case PeacefulLines.Buying:
                main = "My gift of sleep and your gift of death are not so different. " +
                    "Choose wisely, and bring swift justice to those horrid beasts.";
                sound = buyingPopup;
                break;
            case PeacefulLines.Exiting:
                main = "Be well, small one. Dark horrors await you beyond my reach. " +
                    "As always, I will await your end.";
                sound = exitingPopup;
                break;
        }

        peaceful.SetActive(true);

        peacefulMain.text = main;
        peacefulMainDup.generate();

        audio.clip = sound;
        audio.volume = 1;
        audio.Play();

        //Invoke("hidePeaceful", main.Length / 10f);
    }

    private void hidePeaceful()
    {
        peaceful.SetActive(false);
        audio.Stop();
    }
}
