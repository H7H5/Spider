using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    public GameObject currentCard;  // выбраная карта
    private bool bottomFlag = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        currentCard = this.gameObject;
    }

    public bool Blocked(GameObject selected)// проверка на блокировку другими картами
    {
        DataCard dataCard = selected.GetComponent<DataCard>();
        if (dataCard.faceUp == true)
        {
            if (selected.transform.childCount >0)
            {
                string suitParent = dataCard.suit;
                int valueParent = dataCard.value;
                return CheckChild(selected, suitParent, valueParent);
            }
            else
            {
                return false;
            }    
        }
        else
        {
            return true;
        }
    }

    private bool CheckChild(GameObject parent, string suitParent, int valueParent) //проверка на блокировку всех потомков
    {
        string suitChild = parent.transform.GetChild(0).GetComponent<DataCard>().suit;
        int valueChild = parent.transform.GetChild(0).GetComponent<DataCard>().value;
        if (suitParent.Equals(suitChild) && valueParent == valueChild + 1)
        {
            if (parent.transform.GetChild(0).transform.childCount > 0)
            {
                return CheckChild(parent.transform.GetChild(0).gameObject, suitParent, valueChild);
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    public bool CardMove(GameObject currentCard, GameObject downObj)
    {
        if (downObj.CompareTag("Card"))
        {
            this.currentCard = currentCard;
            return Card(downObj);
        }
        else if (downObj.CompareTag("Bottom"))
        {
            bottomFlag = true;
            this.currentCard = currentCard;
            return Bottom(downObj);
        }
        return false;
    }

    bool Card(GameObject downObj)
    {
        if (Stackable(downObj))
        {
            currentCard.GetComponent<MoveCard>().ReturnCard();
            Stack(downObj);
            return true;
        }
        return false;
    }

    bool Bottom(GameObject downObj)// обработка нажатия на нижнею позицию (без карт)
    {
        if (downObj.transform.childCount > 0)
        {
            return false;
        }
        if (currentCard.CompareTag("Card"))
        {
            currentCard.GetComponent<MoveCard>().ReturnCard();
            Stack(downObj);
            return true;
        }
        return false;
    }

    bool Stackable(GameObject selected) // проверка можно ли стекать карты
    {
        DataCard dataCard1 = currentCard.GetComponent<DataCard>();
        DataCard dataCard2 = selected.GetComponent<DataCard>();
        if (dataCard1.value == dataCard2.value - 1)
        {
          return true;
        }
        return false;
    }

    void Stack(GameObject downObj) //стекает карты
    {
        DataCard dataCard1 = currentCard.GetComponent<DataCard>();
        DataCard dataCard2 = downObj.GetComponent<DataCard>();
        float yOffset = 0.3f;
        if (bottomFlag == true)
        {
            bottomFlag = false;
            yOffset = 0;
        }
        currentCard.transform.position = new Vector3(downObj.transform.position.x, downObj.transform.position.y - yOffset, downObj.transform.position.z - 0.01f);
        currentCard.transform.parent = downObj.transform; // это заставляет детей двигаться вместе с родителями
        currentCard = this.gameObject;
    }
}
