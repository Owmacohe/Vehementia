using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextDuplicator : MonoBehaviour
{
    public Color textColour;
    public Vector2 offset = new Vector2(1, 0);

    private GameObject duplicate;

    private void Start()
    {
        generate();
    }

    public void generate()
    {
        if (duplicate != null)
        {
            Destroy(duplicate);
        }

        duplicate = Instantiate(gameObject, gameObject.transform.parent.transform);
        duplicate.transform.localPosition += new Vector3(offset.x, offset.y, 0);
        duplicate.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex());

        duplicate.GetComponent<TMP_Text>().color = textColour;

        Destroy(duplicate.GetComponent<TextDuplicator>());
    }
}
