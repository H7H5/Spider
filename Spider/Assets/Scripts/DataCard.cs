using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCard : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    public string suit; //  масть
    public int value; //   номинал
    public bool faceUp = false; //положение карты вверх - низ 
    public Vector3 endPosition;
    public bool isMove = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        suit = transform.name[0].ToString();
        string valueString = transform.name.Substring(1);
        for (int i = 0; i < GameController.values.Length; i++)
        {
            if (valueString.Equals(GameController.values[i]))
            {
                value = i+1;
            }
        }
        List<string> deck = GameController.GenerateDeck();
        int j = 0;
        foreach (string card in deck)
        {
            if (this.name == card)
            {
                cardFace = GameController.Instance.cardFaces[j];
                break;
            }
            j++;
        }  
    }
    void Update()
    {
        if (isMove == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, 0.5f);
            if (Vector3.Distance(transform.position, endPosition) < 0.01f)
            {
                isMove = false;
            }
        }
    }
    public void Face(bool face)
    {
        faceUp = face;
        if (spriteRenderer == null)
        {
            Start();
        }
        if (face == true)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
    }
    public void RunInPosition(Vector3 position)
    {
        endPosition = position;
        isMove = true;
    }
}
