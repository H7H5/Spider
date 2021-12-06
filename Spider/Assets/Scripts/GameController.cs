using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject winPanel;
    public Transform[] finalPosition;
    private int numberFinalPosition = 0;
    public Transform startPosition;
    public Sprite[] cardFaces;               // спрайты карт
    public GameObject cardPref;              // префаб карты
    public GameObject[] cellsPositions;      // позиции карт
    public GameObject buttonDeck;
    public static int level = 0;             //уровень сложности
    public static string[,]suits = new string[,] { { "D", "D", "D", "D" }, { "D", "D", "S", "S" }, { "D", "S", "H", "C" }};//масть                                        // масть
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };  //номинал
    private int[] _countCardInPos = new int[]{ 5, 5, 5, 5, 4, 4, 4, 4, 4, 4 };
    public List<string> deck;
    public int strikeCount = 1; //количество карт в собраном стеке
    private List<GameObject> strikeObj = new List<GameObject>();  //сюда помещаются карты из собраного стека
    private bool flagWin = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        PlayCards();
    }

    public void PlayCards()   //роздать карты
    {
        deck = GenerateDeck();  // генерируем колоду карт
        Shuffle(deck);          // перетасовка карт 
        Distribute();        // создание объектов карт и назмещение их по позициях
    }
    public static List<string> GenerateDeck() // генератор карт
    {
        List<string> newDeck = new List<string>();
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j<4; j++)
            {
                foreach (string v in values)
                {
                    newDeck.Add(suits[level,j] + v);
                }
            }
        }
        
        return newDeck;
    }

    private void Shuffle(List<string> list) // перетасовка карт
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            string temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
 
    void Distribute()  // создание объектов карт и размещение их по позициях
    {
        for (int i = 0; i < cellsPositions.Length; i++)
        {
            float yOffset = 0;
            float zOffset = 0.09f;
            for (int j = 0; j < _countCardInPos[i];j++)
            {
                string card = deck.Last<string>();
                deck.RemoveAt(deck.Count - 1);
                Vector3 tempPosition = new Vector3(cellsPositions[i].transform.position.x,
                    cellsPositions[i].transform.position.y - yOffset, cellsPositions[i].transform.position.z - zOffset);
                GameObject newCard = Instantiate(cardPref, startPosition.position, Quaternion.identity, cellsPositions[i].transform);
                newCard.name = card;
                newCard.GetComponent<DataCard>().RunInPosition(tempPosition);
                if (j == _countCardInPos[i] - 1)
                {
                    newCard.GetComponent<DataCard>().Face(true);
                }
                yOffset += 0.3f;
                zOffset += 0.03f;
            }
        }
    }

    public void ButtonDeck()    //добавление карт в позиции
    {
        int temp = cellsPositions[0].transform.childCount;
        Transform gameObj = GetlastTraтsform(0);
        if (gameObj.GetComponent<DataCard>().isMove) // проверка чтобы карты не двигались
        {
            return;
        }
        if (deck.Count <= 0) // проверка чтобы в колоде были карты 
        {
            Debug.Log("В колоде закончились карты");
            return;
        }
        for (int j = 0; j < 10; j++)  // проверка на пустые ячейки
        {
            if (cellsPositions[j].transform.childCount == 0)
            {
                Debug.Log("пустые ячейки");
                return;
            }
        }
        HandOneCard();
        if (deck.Count <= 0)
        {
            Debug.Log("В колоде закончились карты");
            buttonDeck.SetActive(false);
        }
    }

    void HandOneCard()  // создание объектов карт и размещение их по позициях(по одной карте)
    {
        for (int i = 0; i < cellsPositions.Length; i++)
        {
            float yOffset = 0.3f;
            float zOffset = 0.03f;
            string card = deck.Last<string>();
            deck.RemoveAt(deck.Count - 1);
            Transform lastCard = GetlastTraтsform(i);
            Vector3 tempPosition = new Vector3(lastCard.position.x,
                    lastCard.position.y - yOffset, lastCard.position.z - zOffset);
            GameObject newCard = Instantiate(cardPref, startPosition.position, Quaternion.identity, cellsPositions[i].transform);
            newCard.transform.parent = lastCard;
            newCard.name = card;
            newCard.GetComponent<DataCard>().RunInPosition(tempPosition);
            newCard.GetComponent<DataCard>().Face(true);
        }
        FindStrikeAll();
    }

    Transform GetlastTraтsform(int i) //нахождение последнего потомка в последнего потомка
    {
        int countPosCard = cellsPositions[i].transform.childCount;
        return LastChild(cellsPositions[i].transform.GetChild(countPosCard - 1));
    }
    Transform LastChild(Transform ParentTransform) // рекурсия
    {
        if (ParentTransform.transform.childCount>0)
        {
            return LastChild(ParentTransform.transform.GetChild(0));
        }
        else
        {
            return ParentTransform;
        }
    }

    public void FindStrikeAll()
    {
        for (int i = 0; i < cellsPositions.Length; i++)
        {
            FindStrike(i);
        }
    }
    public void FindStrike(int numPosition)
    {
        if (flagWin == false)
        {
            strikeCount = 1;
            if (cellsPositions[numPosition].transform.childCount == 0)
            {
                return;
            }
            int countPosCard = cellsPositions[numPosition].transform.childCount;
            GameObject currentObj = cellsPositions[numPosition].transform.GetChild(countPosCard - 1).gameObject;
            if (currentObj.transform.childCount > 0)
            {
                string suitParent = currentObj.GetComponent<DataCard>().suit;
                int valueParent = currentObj.GetComponent<DataCard>().value;
                strikeObj = new List<GameObject>(); ;
                strikeObj.Add(currentObj);
                NextChild(currentObj, suitParent, valueParent);
            }
            else
            {
                return;
            }
        }
    }
    void NextChild(GameObject obj, string suitParent, int valueParent) // проверка всех потомков подходят ли они 
    {
        string suitChild = obj.transform.GetChild(0).GetComponent<DataCard>().suit;
        int valueChild = obj.transform.GetChild(0).GetComponent<DataCard>().value;
        if (suitParent.Equals(suitChild) && valueParent == valueChild + 1)
        {
            strikeObj.Add(obj.transform.GetChild(0).gameObject);
            strikeCount++;
            if (strikeCount >= 13)   //собрали стек от короля до туза
            {
                flagWin = true;
                StartCoroutine(Win());
                return;
            }
            if (obj.transform.GetChild(0).transform.childCount > 0)
            {
                NextChild(obj.transform.GetChild(0).gameObject, suitParent, valueChild);
            }
        }
        else
        {
            strikeObj = new List<GameObject>(); ;
            strikeObj.Add(obj.transform.GetChild(0).gameObject);
            strikeCount = 1;
            if (obj.transform.GetChild(0).transform.childCount > 0)
            {
                NextChild(obj.transform.GetChild(0).gameObject, suitChild, valueChild);
            }  
        }
    }
    IEnumerator Win()
    {
        int d = 0;
        for (int i = strikeObj.Count-1; i >= 0; i--)
        {
            yield return new WaitForSeconds(0.2f); // чтоби карти шли по одной
            if (i==0)
            {
                GameObject parent = strikeObj[i].transform.parent.gameObject;
                if (parent.CompareTag("Bottom"))
                {
                    int countChild = parent.transform.childCount;
                    if (countChild > 1)
                    {
                        parent.transform.GetChild(countChild - 2).gameObject.GetComponent<DataCard>().Face(true);
                    }
                }
            }
            strikeObj[i].transform.SetParent(finalPosition[numberFinalPosition]);
            Vector3 newPosition = new Vector3(finalPosition[numberFinalPosition].position.x,
                finalPosition[numberFinalPosition].position.y, finalPosition[numberFinalPosition].position.z);
            strikeObj[i].GetComponent<DataCard>().RunInPosition(newPosition);
            strikeObj[i].GetComponent<SpriteRenderer>().sortingOrder = d;
            strikeObj[i].GetComponent<MoveCard>().enabled = false;
            strikeObj[i].tag = "brake";
            d++;   
        }
        numberFinalPosition++;
        if (numberFinalPosition > 7)
        {
            Congratulations();// поздравление
        }
        flagWin = false;
    }
    public void Restart()
    {
        for (int i = 0; i < cellsPositions.Length; i++)
        {
            for (int j = cellsPositions[i].transform.childCount-1; j>=0; j--)
            {
                Destroy(cellsPositions[i].transform.GetChild(j).gameObject);
            }
        }
        for (int i = 0; i < finalPosition.Length; i++)
        {
            for (int j = finalPosition[i].transform.childCount - 1; j >= 0; j--)
            {
                Destroy(finalPosition[i].transform.GetChild(j).gameObject);
            }
        }
        numberFinalPosition = 0;
        PlayCards();
        buttonDeck.SetActive(true);

    }

    public void Congratulations()
    {
        winPanel.gameObject.SetActive(true);
    }
}
