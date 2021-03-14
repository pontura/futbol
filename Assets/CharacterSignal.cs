using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSignal : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer sr;

    public SpriteRenderer[] all;
    public Transform parentT;
    public Transform container;

    public void Init(Color color, int playerID)
    {
        parentT = transform.parent.transform;
        sr.sprite = sprites[playerID - 1];
        foreach (SpriteRenderer sr in all)
            sr.color = color;
    }
    void Update()
    {
        if (parentT != null)
            container.transform.localScale = new Vector3(transform.parent.transform.localScale.x, 1, 1);
    }
}
