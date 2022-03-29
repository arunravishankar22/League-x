using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Lancher : MonoBehaviourPunCallbacks
{

    public static Lancher Instance;


    [SerializeField] TMP_Text ErrorText;
    [SerializeField] TMP_Text RoomText;
    [SerializeField] TMP_InputField roomNameInputFeild;
    [SerializeField] TMP_InputField NameInputFeild, NameInputFeildInJoin;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPreferb;
    [SerializeField] GameObject playerListItemPreferb;
    [SerializeField] GameObject startGameButton;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("Title");
        Debug.Log("Joined Lobby");
        
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 100).ToString("000"); //link with firebase and add email id or enter name
    }

    public void PlayerName()
    {
        if(NameInputFeild.text!="")
        {
            PhotonNetwork.NickName = NameInputFeild.text;
        }
        else
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 100).ToString("000");
        }
    }


    public void JoinPlayerName()
    {
        if (NameInputFeildInJoin.text != "")
        {
            PhotonNetwork.NickName = NameInputFeildInJoin.text;
        }
        else
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 100).ToString("000");
        }
    }

    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputFeild.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputFeild.text);
        MenuManager.Instance.OpenMenu("Loading");
        PlayerName();
    }


    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        RoomText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }


        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPreferb, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ErrorText.text = "Room Creation Faild : " + message;
        MenuManager.Instance.OpenMenu("Error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        JoinPlayerName();
        PhotonNetwork.JoinRoom(info.Name);
        
        MenuManager.Instance.OpenMenu("Loading");

        
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPreferb, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPreferb, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
