﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618

public class GameManager : NetworkBehaviour {

    #region Variable  
    [SerializeField] private Board m_GameBoard;
    [SerializeField] private GameObject m_PlayerListRef;
    [SerializeField] private GameObject m_StartGameButton;
    [SerializeField] private List<Player> m_PlayerList;

    [Header("Network Variable")]
    [SyncVar]
    private int m_GameDuration;
    [SyncVar]
    private int m_DeckSeed;

    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;
    public int playerNum;
    public bool startScoreTrack = false;
    public TextMeshProUGUI scoreText;
    public GameObject finishMenu;
    
    #endregion

    #region Getter & Setter
    public Board GameBoard
    {
        get
        {
            return m_GameBoard;
        }

        set
        {
            m_GameBoard = value;
        }
    }
    public List<Player> PlayerList
    {
        get
        {
            return m_PlayerList;
        }

        set
        {
            m_PlayerList = value;
        }
    }
    public int GameDuration
    {
        get
        {
            return m_GameDuration;
        }

        set
        {
            m_GameDuration = value;
        }
    }
    public int DeckSeed
    {
        get
        {
            return m_DeckSeed;
        }

        set
        {
            m_DeckSeed = value;
        }
    }

    public GameObject PlayerListRef
    {
        get
        {
            return m_PlayerListRef;
        }

        set
        {
            m_PlayerListRef = value;
        }
    }

    public GameObject StartGameButton
    {
        get
        {
            return m_StartGameButton;
        }

        set
        {
            m_StartGameButton = value;
        }
    }
    #endregion

    #region Private Method
    private void OnDisable()
    {
        if (playerNum == 2)
        {
            player1Text.text = "Player 1: Disconnected";
            AudioManager.Instance.PlaySoundEffect(E_SoundEffect.User_Left);
        }
    }

    public void UpdatePlayerList()
    {
        PlayerList = new List<Player>(PlayerListRef.transform.GetComponentsInChildren<Player>());
    }

    private void Update()
    {
        if(startScoreTrack)
        scoreText.text = Player.localPlayer.playerScore.ToString();
    }
    #endregion

    #region Public Method
    [ClientRpc]
    public void RpcPlayMusic()
    {
        AudioManager.Instance.ChangeBackground(E_BackGroundMusic.Main_Game_Background);
    }

    public void StartGame()
    {
        UpdatePlayerList();

        if (isServer)
        {
            GameObject.Find("Net Man").GetComponent<NetworkManager>().StopMatchMaker();
            Player.localPlayer.CmdServerCommand("Toggle Time");

            DeckSeed = Random.Range(1, 1000000);
            GameBoard.BoardDeck.RpcShuffleDeck(DeckSeed);
            GameBoard.BoardPile.RpcRandomStartingCard(DeckSeed);
            GameBoard.BoardDeck.RpcSetId();

            string convertedString = RpcStringConverter<Player>.ConvertString(PlayerList);

            RpcPlayMusic();
            GameBoard.RpcDealCard(convertedString, 5, 1f);
            GameBoard.RpcSetTurn();
        }

    }  
    #endregion
}
