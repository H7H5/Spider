using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCard : MonoBehaviour
{
    private bool isDragging;
    private Vector3 oldTransform; 
    private GameObject downCard; // нижняя карта

    private void OnMouseDown() // если нажали на карту
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (GameLogic.Instance.Blocked(gameObject)==false)
        {
            downCard = GetDownCard(); // нашли нажнюю карту если она есть
            oldTransform = gameObject.transform.position;// заполнили текущее положение
            isDragging = true;
        }
        else
        {
            isDragging = false;
        }
    }
    private void OnMouseUp() // если отпустили карту
    {
        if (isDragging)
        {
            gameObject.SetActive(false);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
            gameObject.SetActive(true);
            if (hit.collider != null)
            {
                GameObject obj = hit.collider.transform.gameObject;
                if (GameLogic.Instance.CardMove(gameObject, obj) == false) //пытаемся положить карту
                {
                    transform.position = oldTransform; //возвращяем карту на старое место
                }
                else
                {
                    GameController.Instance.FindStrikeAll(); // проверяем на наличие победных комбинаций
                }
            }
            else
            {
                transform.position = oldTransform;//возвращяем карту на старое место
            }
        }
        isDragging = false;
    }
    private void Update()
    {
        if (isDragging)// движение за мышкой
        {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
           Input.mousePosition.y, 1));
        }
    }
    private GameObject GetDownCard() // поиск карты под низом 
    {
        gameObject.SetActive(false);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
        gameObject.SetActive(true);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Card"))
            {
                GameObject card = hit.collider.transform.gameObject;
                return card;
            }
        }
        return null;
    }
    public void ReturnCard()  // переворот карты под низом 
    {
        if (downCard != null)
        {
            downCard.GetComponent<DataCard>().Face(true);
        }
    }
}
