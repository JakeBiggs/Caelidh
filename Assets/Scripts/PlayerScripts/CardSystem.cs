using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.TextCore;
using TMPro;

public class CardSystem : MonoBehaviour
{
    GameObject uiCanvas;

    private bool canUseLeftCard;
    private bool canUseRightCard;

    public GameObject cardInQueue1;
    public GameObject cardInQueue2;
    public GameObject cardInQueue3;
    public GameObject cardInQueue4;
    public GameObject cardInQueue5;
    public GameObject cardLeftRender;
    public GameObject cardRightRender;
    public List<GameObject> cardRenders;

    public Sprite lifeStealSprite;
    public Sprite speedSprite;
    public Sprite instantHealthSprite;
    public Sprite strengthSprite;
    public Sprite maxHealthSprite;

    public int maxQueueSize;
    public bool shouldReQueueACard;
    public float cardCooldown;
    public Queue<Card> cardsQueue;
    public List<Card> cardsList;
    public Card cardLeft;
    public Card cardRight;
    PlayerControllerScript pc;

    public Dictionary<GameControllerScript.BuffTypes,Sprite> cardTypeSprites;
    // Start is called before the first frame update
    void Start()
    {
        InitialiseVariables();
        SetupCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void InitialiseVariables()
    {
        cardTypeSprites = new Dictionary<GameControllerScript.BuffTypes, Sprite>();
        SetupSpriteDictionary();
        uiCanvas = GameObject.FindGameObjectWithTag("UICanvas");
        canUseLeftCard = true;
        canUseRightCard = true;
        cardsQueue = new Queue<Card>();
        cardsList = new List<Card>();
        pc = GameObject.Find("Player").GetComponent<PlayerControllerScript>();

        cardRenders.Add(cardInQueue1);
        cardRenders.Add(cardInQueue2);
        cardRenders.Add(cardInQueue3);
        cardRenders.Add(cardInQueue4);
        cardRenders.Add(cardInQueue5);
        cardRenders.Reverse();
    }

    private void SetupSpriteDictionary()
    {
        cardTypeSprites.Add(GameControllerScript.BuffTypes.LifeSteal, lifeStealSprite);
        cardTypeSprites.Add(GameControllerScript.BuffTypes.Speed, speedSprite);
        cardTypeSprites.Add(GameControllerScript.BuffTypes.InstantHealth, instantHealthSprite);
        cardTypeSprites.Add(GameControllerScript.BuffTypes.Strength, strengthSprite);
        cardTypeSprites.Add(GameControllerScript.BuffTypes.MaxHealth, maxHealthSprite);
    }
    private void CardCreator()
    {
        Buff buff = new Buff();
        Card card = new Card();




        //SpeedBuff Cards
        buff = new Buff(GameControllerScript.BuffTypes.Speed,0.1f,0,7.5f,false);
        card = new Card();
        card.buff = buff;
        card.name = "Weak";
        cardsList.Add(card);

        buff = new Buff(GameControllerScript.BuffTypes.Speed, 0.2f, 0, 3.75f, false);
        card = new Card();
        card.buff = buff;
        card.name = "Strong";
        cardsList.Add(card);

        //LifeSteal Cards
        buff = new Buff(GameControllerScript.BuffTypes.LifeSteal, 0.025f, 0, 10, false);
        card = new Card();
        card.buff = buff;
        card.name = "Weak";
        cardsList.Add(card);

        buff = new Buff(GameControllerScript.BuffTypes.LifeSteal, 0.05f, 0, 5, false);
        card = new Card();
        card.buff = buff;
        card.name = "Strong";
        cardsList.Add(card);

        //Strength Cards
        buff = new Buff(GameControllerScript.BuffTypes.Strength, 0.25f, 0, 10, false);
        card = new Card();
        card.buff = buff;
        card.name = "Weak";
        cardsList.Add(card);

        buff = new Buff(GameControllerScript.BuffTypes.Strength, 0.5f, 0, 5f, false);
        card = new Card();
        card.buff = buff;
        card.name = "Strong";
        cardsList.Add(card);

        //MaxHealth Cards
        buff = new Buff(GameControllerScript.BuffTypes.MaxHealth, 0.5f, 0, 60, false);
        card = new Card();
        card.buff = buff;
        card.name = "Weak";
        cardsList.Add(card);

        buff = new Buff(GameControllerScript.BuffTypes.MaxHealth, 1, 0, 30, false);
        card = new Card();
        card.buff = buff;
        card.name = "Strong";
        cardsList.Add(card);

        //InstantHealth Cards
        buff = new Buff(GameControllerScript.BuffTypes.InstantHealth, 0.5f, 0, 0, false);
        card = new Card();
        card.buff = buff;
        card.name = "Weak";
        cardsList.Add(card);

        buff = new Buff(GameControllerScript.BuffTypes.InstantHealth, 1, 0, 0, false);
        card = new Card();
        card.buff = buff;
        card.name = "Strong";
        cardsList.Add(card);
    }
    private void SetupCards()
    {
        CardCreator();
        for(int i = 0; i < maxQueueSize; i++)
        {
            QueueARandomCard();
        }
        cardLeft = cardsQueue.Dequeue();
        QueueARandomCard();
        cardRight = cardsQueue.Dequeue();
        QueueARandomCard();
        MoveDownTheQueue();
        //cardLeftText.GetComponent<TextMeshProUGUI>().text = cardLeft.name + " " + cardLeft.buff.affectIntensity;
        //cardRightText.GetComponent<TextMeshProUGUI>().text = cardRight.name + " " + cardRight.buff.affectIntensity;
        cardRightRender.GetComponent<UnityEngine.UI.Image>().sprite = cardTypeSprites[cardRight.buff.type];
        cardRightRender.transform.Find("Potency").GetComponent<TextMeshProUGUI>().text = cardRight.name + " - " + cardRight.buff.affectDuration + "s";
        cardLeftRender.GetComponent<UnityEngine.UI.Image>().sprite = cardTypeSprites[cardLeft.buff.type];
        cardLeftRender.transform.Find("Potency").GetComponent<TextMeshProUGUI>().text = cardLeft.name + " - " + cardLeft.buff.affectDuration + "s";
    }

    private IEnumerator OnUseCardLeft()
    {
        if (!canUseLeftCard) yield break;
        print(cardLeft.name + " " + cardLeft.buff.affectIntensity);
        pc.ApplyBuff(cardLeft.buff);
        cardsList.Add(cardLeft);
        if (cardsQueue.Count > 0) cardLeft = cardsQueue.Dequeue();
        if (shouldReQueueACard) QueueARandomCard();
        MoveDownTheQueue();
        canUseLeftCard = false;
        //cardLeftText.GetComponent<TextMeshProUGUI>().text = cardLeft.name + " " + cardLeft.buff.affectIntensity;
        cardLeftRender.GetComponent<UnityEngine.UI.Image>().sprite = cardTypeSprites[cardLeft.buff.type];
        cardLeftRender.transform.Find("Potency").GetComponent<TextMeshProUGUI>().text = cardLeft.name + " - " + cardLeft.buff.affectDuration + "s";
        yield return new WaitForSeconds(cardCooldown);
        canUseLeftCard = true;
    }

    private IEnumerator OnUseCardRight()
    {
        if (!canUseRightCard) yield break;
        print(cardRight.name + " " + cardRight.buff.affectIntensity);
        pc.ApplyBuff(cardRight.buff);
        cardsList.Add(cardRight);
        if(cardsQueue.Count > 0) cardRight = cardsQueue.Dequeue();
        if(shouldReQueueACard) QueueARandomCard();
        MoveDownTheQueue();
        canUseRightCard = false;
        //cardRightText.GetComponent<TextMeshProUGUI>().text = cardRight.name + " " + cardRight.buff.affectIntensity;
        cardRightRender.GetComponent<UnityEngine.UI.Image>().sprite = cardTypeSprites[cardRight.buff.type];
        cardRightRender.transform.Find("Potency").GetComponent<TextMeshProUGUI>().text = cardRight.name + " - " + cardRight.buff.affectDuration + "s";
        yield return new WaitForSeconds(cardCooldown);
        canUseRightCard = true;
    }

    private void QueueARandomCard()
    {
        int rand = Random.Range(0, cardsList.Count);
        cardsQueue.Enqueue(cardsList[rand]);
        cardsList.Remove(cardsList[rand]);
    }
    private void MoveDownTheQueue()
    {
        int i = 0;
        foreach(Card card in cardsQueue)
        {
            //print(i + " " + card.buff.type + " " + card.buff.affectIntensity);
            //cardRenders[i].GetComponent<TextMeshProUGUI>().text = card.name + " " + card.buff.affectIntensity;
            cardRenders[i].GetComponent<UnityEngine.UI.Image>().sprite = cardTypeSprites[card.buff.type];
            if(i == 0)
            {
                cardRenders[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = card.name + " - " + card.buff.affectDuration + "s";
            }
            i++;
        }
    }
    private void EmptyQueue()
    {
        for(int i = 0; i < cardsQueue.Count; i++)
        {
            cardsList.Add(cardsQueue.Dequeue());
        }
    }
}
