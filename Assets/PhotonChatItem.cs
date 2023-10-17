using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonChatItem : MonoBehaviour
{
    Text myText;

    RectTransform rt;

    void Awake()
    {
        myText = GetComponent<Text>();

        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        
    }

    public void SetText(string chat, Color color)
    {
        myText.text = chat;

        myText.color = color;

        // height �� ����ũ��� ����
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, myText.preferredHeight);
    }
}
