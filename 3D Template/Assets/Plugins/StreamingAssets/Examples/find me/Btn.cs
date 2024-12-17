using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class Btn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Event")]

    public bool enter;

    [Space(10)]

    public UnityEvent btnEvent;

    [Header("Gui")]

    [SerializeField] Image img;
    [SerializeField] TextMeshProUGUI label;

    [Header("Aniamte")]

    [SerializeField] bool animate;

    string initialText;
    [SerializeField] string hoverText;

    [Space(10)]

    [SerializeField] Color[] color = new Color[2];
    Color targetColor;

    [SerializeField] Color[] colorLabel = new Color[2];
    Color targetColorLabel;


    [SerializeField] Vector3[] scale = new Vector3[2];
    Vector3 targetScale;

    [Range(0, 1)] [SerializeField] float smooth = 1;

    protected virtual void Start()
    {
        img = (img == null) ? transform.GetChild(0).GetComponent<Image>() : img;
        label = (label == null) ? transform.GetChild(1)?.GetComponent<TextMeshProUGUI>() : label;

        initialText = label.text;
        hoverText = (hoverText == "") ? initialText : hoverText;


        //Animate

        if (!animate) return;

        targetColor = color[0];
        targetColorLabel = colorLabel[0];
    }

    protected virtual void Update()
    {
        if (!animate) return;
        img.color = Color.Lerp(img.color, targetColor, smooth);
        img.transform.localScale = Vector3.Lerp(img.transform.localScale, targetScale, smooth);

        if (label == null) return;
        label.color = Color.Lerp(label.color, targetColorLabel, smooth);
    }


    public virtual void OnPointerEnter(PointerEventData e)
    {
        enter = true;

        if (!animate) return;

        targetColor = color[1];
        targetScale = scale[1];

        if (label == null) return;
        targetColorLabel = colorLabel[1];
    }

    public virtual void OnPointerExit(PointerEventData e)
    {
        enter = false;

        if (!animate) return;

        targetColor = color[0];
        targetScale = scale[0];

        if (label == null) return;
        targetColorLabel = colorLabel[0];
    }

    public virtual void OnPointerDown(PointerEventData e)
    {
        btnEvent.Invoke();
    }
}
