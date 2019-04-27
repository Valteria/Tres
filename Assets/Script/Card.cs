﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public enum E_CardColor { Red, Blue, Yellow, Green, Black}
public enum E_CardType
{
    Zero = 0, One = 1, Two = 2, Three = 3,
    Four = 4, Five = 5, Six = 6, Seven = 7,
    Eight = 8, Nine = 9, Reverse = 10, Draw = 11,
    Skip = 12, Change_Color = 13, Draw_4 = 14, Draw_2 = 15
}

public enum E_ZoneType { Hand, Deck, Pile }
public enum E_FaceType { Front, Back }
public enum E_PostionType { Top, Bottom }

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    #region Variable
    [Header("Card Data")]
    private int m_CardObjectId;
    [SerializeField] private E_CardColor m_CardColor;
    [SerializeField] private E_CardType m_CardType;
    [SerializeField] private E_ZoneType m_CardLocation;

    [Header("Sprite")]
    private Sprite m_FrontImage;
    private Sprite m_BackImage;
    [SerializeField] SpriteAtlas m_ImageAtlas;   
    [SerializeField] private Sprite m_OverlayTransparent;
    [SerializeField] private Sprite m_OverlayHighlighted;
    [SerializeField] private Sprite m_OverlayAvailable;
    
    [Header("Child Object")]
    [SerializeField] private GameObject m_CardOverlay;

    [Header("Card Variable")]
    [SerializeField] private bool m_IsSelected;
    #endregion

    #region Getter & Setter
    public int CardObjectId
    {
        get
        {
            return m_CardObjectId;
        }

        set
        {
            m_CardObjectId = value;
        }
    }
    public E_CardColor CardColor
    {
        get
        {
            return m_CardColor;
        }

        set
        {
            m_CardColor = value;
        }
    }
    public E_CardType CardType
    {
        get
        {
            return m_CardType;
        }

        set
        {
            m_CardType = value;
        }
    }
    public E_ZoneType CardLocation
    {
        get
        {
            return m_CardLocation;
        }

        set
        {
            m_CardLocation = value;
        }
    }
    public Sprite OverlayTransparent
    {
        get
        {
            return m_OverlayTransparent;
        }

        set
        {
            m_OverlayTransparent = value;
        }
    }
    public Sprite OverlayHighlighted
    {
        get
        {
            return m_OverlayHighlighted;
        }

        set
        {
            m_OverlayHighlighted = value;
        }
    }
    public Sprite OverlayAvailable
    {
        get
        {
            return m_OverlayAvailable;
        }

        set
        {
            m_OverlayAvailable = value;
        }
    }
    public SpriteAtlas ImageAtlas
    {
        get
        {
            return m_ImageAtlas;
        }

        set
        {
            m_ImageAtlas = value;
        }
    }
    public Sprite FrontImage
    {
        get
        {
            return m_FrontImage;
        }

        set
        {
            m_FrontImage = value;
        }
    }
    public Sprite BackImage
    {
        get
        {
            return m_BackImage;
        }

        set
        {
            m_BackImage = value;
        }
    }
    public GameObject CardOverlay
    {
        get
        {
            return m_CardOverlay;
        }

        set
        {
            m_CardOverlay = value;
        }
    }

    public bool IsSelected
    {
        get
        {
            return m_IsSelected;
        }

        set
        {
            m_IsSelected = value;
        }
    }
    #endregion

    public void Awake()
    {     
        string cardColor = m_CardColor.ToString();
        string cardType = m_CardType.ToString().Replace("_", " ");

        m_FrontImage = m_ImageAtlas.GetSprite(cardColor + " " + cardType);
        m_BackImage = m_ImageAtlas.GetSprite("Back Cover");

        IsSelected = false;
        Reveal();
    }

    public void Update()
    {
        
    }

    public void MoveCard(GameObject location)
    {
        StartCoroutine(MoveCard_IE(location));
    }

    public void PlayCard()
    {


    }

    public void Reveal()
    {
        GetComponent<Image>().sprite = m_FrontImage;
    }

    public void ToggleSelected()
    {
        if (IsSelected)
        {
            CardOverlay.GetComponent<Image>().sprite = m_OverlayTransparent;
            IsSelected = false;
        }
        else
        {
            CardOverlay.GetComponent<Image>().sprite = m_OverlayHighlighted;
            IsSelected = true;
        }
    }

    public void Hide()
    {
        GetComponent<Image>().sprite = m_BackImage;
    }

    //Move card back to (0,0) and Rotate card depending on move type
    IEnumerator MoveCard_IE(GameObject cardLocation)
    {
        cardLocation.GetComponent<Hand>().AddCard(this.gameObject);
        RectTransform cardRT = GetComponent<RectTransform>();

        //Variable for position lerp
        Vector2 currentPostion = cardRT.anchoredPosition;
        Vector2 targetPostion = new Vector2(0, 0);

        //Variable for scale lerp
        Vector3 currentScale = cardRT.localScale;
        Vector3 targetScale = cardLocation.GetComponent<RectTransform>().localScale;

        //Variable for rotation lerp
        Quaternion currentRotation = cardRT.rotation;
        Quaternion targetRotation = cardLocation.GetComponent<RectTransform>().rotation;

        //Anchor the image so that it centered when moving back to origin
        cardRT.anchorMax = new Vector2(0.5f, 0.5f);
        cardRT.anchorMin = new Vector2(0.5f, 0.5f);

        //Time
        float time = 0f;
        float timeToFinish = 2f;

        //Move the Card Back to origin over the time it takes to finish
        while (currentPostion != targetPostion)
        {
            time += Time.deltaTime / timeToFinish;

            currentPostion = cardRT.anchoredPosition;
            currentRotation = cardRT.rotation;
            currentScale = cardRT.localScale;

            cardRT.anchoredPosition = Vector2.Lerp(currentPostion, targetPostion, time);
            cardRT.rotation = Quaternion.Lerp(currentRotation, targetRotation, time);
            cardRT.localScale = Vector3.Lerp(currentScale, targetScale, time);

            yield return null;
        }

    }


    //Called when mouse pointer leaves area
    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardLocation == E_ZoneType.Hand)
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);        
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardLocation == E_ZoneType.Hand)
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 25, 0);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Card selectedCard = GetComponentInParent<Player>().playerHand.SelectedCard;

        if (selectedCard != null)
            selectedCard.ToggleSelected();

        GetComponentInParent<Player>().playerHand.SelectedCard = this;
        ToggleSelected();
    }
}



