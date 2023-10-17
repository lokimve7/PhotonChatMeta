using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    public InputField inputChat;

    public GameObject chatItemFactory;

    public Transform trContent;

    // Photon Chat Setting
    ChatAppSettings chatAppSettings;

    // 채팅을 총괄하는 객체
    ChatClient chatClient;

    // 기본 채팅 채널 목록
    public List<string> channelNames = new List<string>();

    // 현재 선택된 채널
    int currChannelIdx = 0;

    void Start()
    {
        // 텍스트를 작성하고 엔터를 쳤을때 호출되는 함수 등록
        inputChat.onSubmit.AddListener(OnSubmit);

        // Photon Chat 초기 설정
        PhotonChatSetting();

        // 접속시도
        Connect();
    }

    void Update()
    {
        if(chatClient != null)
        {
            chatClient.Service();
        }
    }

    void OnSubmit(string text)
    {
        // 귓속말인지 판단
        //  /w 아이디 메시지
        string[] s = text.Split(" ",  3);
        if (s[0] == "/w")
        {
            // 귓속말을 보내자
            if (s[1].Length > 0 && s[2].Length > 0)
            {
                chatClient.SendPrivateMessage(s[1], s[2]);
            }
        }
        else
        {
            // 채팅을 보내자
            chatClient.PublishMessage(channelNames[currChannelIdx], text);
        }


        

        // inputChat 내용 초기화
        inputChat.text = "";
        // inputChat 강제로 선택된 상태로
        inputChat.ActivateInputField();
    }

    void PhotonChatSetting()
    {
        //포톤 설정을 가져와서 ChatAppSettings 에 설정하자.
        AppSettings photonSettings = PhotonNetwork.PhotonServerSettings.AppSettings;

        // 위 설정을 가지고 ChatAppSettings 셋팅
        chatAppSettings = new ChatAppSettings();
        chatAppSettings.AppIdChat = photonSettings.AppIdChat;
        chatAppSettings.AppVersion = photonSettings.AppVersion;
        chatAppSettings.FixedRegion = photonSettings.FixedRegion;
        chatAppSettings.NetworkLogging = photonSettings.NetworkLogging;
        chatAppSettings.Protocol = photonSettings.Protocol;
        chatAppSettings.EnableProtocolFallback = photonSettings.EnableProtocolFallback;
        chatAppSettings.Server = photonSettings.Server;
        chatAppSettings.Port = (ushort)photonSettings.Port;
        chatAppSettings.ProxyServer = photonSettings.ProxyServer;
    }

    void Connect()
    {
        chatClient = new ChatClient(this);

        // 채팅할 때 NickName 을 설정한다.
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues("김현진");
        // 초기설정을 이용해서 채팅서버에 연결 시도
        chatClient.ConnectUsingSettings(chatAppSettings);
    }

    void CreateChat(string sender, string message, Color color)
    {
        // chatItem 생성함 (scrollView -> content 의 자식으로 등록)
        GameObject go = Instantiate(chatItemFactory, trContent);
        // 생성된 게임오브젝트에서 ChatItem 컴포넌트 가져온다.
        PhotonChatItem item = go.GetComponent<PhotonChatItem>();
        // 가져온 컴포넌트에서 SetText 함수 실행
        item.SetText(sender + " : " + message, color);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    // 접속이 끊겼을 때
    public void OnDisconnected()
    {
    }

    // 접속이 성공했을 때
    public void OnConnected()
    {
        print("**** 채팅 서버 접속 성공 ****");
        // 채널 추가
        if(channelNames.Count > 0)
        {
            chatClient.Subscribe(channelNames.ToArray());
        }

        // 나의 상태를 온라인으로 한다.
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    // 특정 채널에 메시지가 오면
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for(int i = 0; i < senders.Length; i++)
        {
            CreateChat(senders[i], messages[i].ToString(), Color.black);
        }
    }

    // 누군가 나한테 귓말 보내면
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        CreateChat(sender, message.ToString(), Color.blue);
    }

    // 채팅 채널을 추가했을 때
    public void OnSubscribed(string[] channels, bool[] results)
    {
        for(int i = 0; i <channels.Length; i++)
        {
            print("**** 채널 [" + channels[i] + "] 추가 성공");
        }
    }

    // 채팅 채널을 삭제했을 때
    public void OnUnsubscribed(string[] channels)
    {
    }

    // 친구 상태가 online, offline 상태로 변경했을 때
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    // 친구 추가 성공적으로 이루어졌을 때
    public void OnUserSubscribed(string channel, string user)
    {
    }

    // 친구 삭제가 성공적으로 이루어졌을 때
    public void OnUserUnsubscribed(string channel, string user)
    {
    }
}
