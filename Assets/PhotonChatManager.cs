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

    // ä���� �Ѱ��ϴ� ��ü
    ChatClient chatClient;

    // �⺻ ä�� ä�� ���
    public List<string> channelNames = new List<string>();

    // ���� ���õ� ä��
    int currChannelIdx = 0;

    void Start()
    {
        // �ؽ�Ʈ�� �ۼ��ϰ� ���͸� ������ ȣ��Ǵ� �Լ� ���
        inputChat.onSubmit.AddListener(OnSubmit);

        // Photon Chat �ʱ� ����
        PhotonChatSetting();

        // ���ӽõ�
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
        // �ӼӸ����� �Ǵ�
        //  /w ���̵� �޽���
        string[] s = text.Split(" ",  3);
        if (s[0] == "/w")
        {
            // �ӼӸ��� ������
            if (s[1].Length > 0 && s[2].Length > 0)
            {
                chatClient.SendPrivateMessage(s[1], s[2]);
            }
        }
        else
        {
            // ä���� ������
            chatClient.PublishMessage(channelNames[currChannelIdx], text);
        }


        

        // inputChat ���� �ʱ�ȭ
        inputChat.text = "";
        // inputChat ������ ���õ� ���·�
        inputChat.ActivateInputField();
    }

    void PhotonChatSetting()
    {
        //���� ������ �����ͼ� ChatAppSettings �� ��������.
        AppSettings photonSettings = PhotonNetwork.PhotonServerSettings.AppSettings;

        // �� ������ ������ ChatAppSettings ����
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

        // ä���� �� NickName �� �����Ѵ�.
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues("������");
        // �ʱ⼳���� �̿��ؼ� ä�ü����� ���� �õ�
        chatClient.ConnectUsingSettings(chatAppSettings);
    }

    void CreateChat(string sender, string message, Color color)
    {
        // chatItem ������ (scrollView -> content �� �ڽ����� ���)
        GameObject go = Instantiate(chatItemFactory, trContent);
        // ������ ���ӿ�����Ʈ���� ChatItem ������Ʈ �����´�.
        PhotonChatItem item = go.GetComponent<PhotonChatItem>();
        // ������ ������Ʈ���� SetText �Լ� ����
        item.SetText(sender + " : " + message, color);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    // ������ ������ ��
    public void OnDisconnected()
    {
    }

    // ������ �������� ��
    public void OnConnected()
    {
        print("**** ä�� ���� ���� ���� ****");
        // ä�� �߰�
        if(channelNames.Count > 0)
        {
            chatClient.Subscribe(channelNames.ToArray());
        }

        // ���� ���¸� �¶������� �Ѵ�.
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    // Ư�� ä�ο� �޽����� ����
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for(int i = 0; i < senders.Length; i++)
        {
            CreateChat(senders[i], messages[i].ToString(), Color.black);
        }
    }

    // ������ ������ �Ӹ� ������
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        CreateChat(sender, message.ToString(), Color.blue);
    }

    // ä�� ä���� �߰����� ��
    public void OnSubscribed(string[] channels, bool[] results)
    {
        for(int i = 0; i <channels.Length; i++)
        {
            print("**** ä�� [" + channels[i] + "] �߰� ����");
        }
    }

    // ä�� ä���� �������� ��
    public void OnUnsubscribed(string[] channels)
    {
    }

    // ģ�� ���°� online, offline ���·� �������� ��
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    // ģ�� �߰� ���������� �̷������ ��
    public void OnUserSubscribed(string channel, string user)
    {
    }

    // ģ�� ������ ���������� �̷������ ��
    public void OnUserUnsubscribed(string channel, string user)
    {
    }
}