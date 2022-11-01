using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FriendListController : MonoBehaviour
{
    [SerializeField]
    public GameObject requestLabel, acceptB;
    public Transform userOnlineLabelPos;
    private TMP_Text requestText;

    [SerializeField]
    private GameObject mainCanva;

    private string currentUsername;

    [SerializeField]
    public GameObject friendLabel;
    public Transform friendLabelPos;
    private TMP_Text friendText;

    DatabaseReference mDatabase;
    public UserOnlineController mUserOnlineController;
    string UserId;

    GameState _GameState;

    private void Awake()
    {
        mUserOnlineController = GameObject.Find("Controller").GetComponent<UserOnlineController>();
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
    }

    private void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState.OnDataReady += InitUsersOnFriendController;
        _GameState.OnDataReady += InitRequestReceived;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    private void OnEnable()
    {
        mUserOnlineController.onSendData += SendRequest;
    }

    private void OnDisable()
    {
        mUserOnlineController.onSendData -= SendRequest;
    }

    public void InitUsersOnFriendController()
    {
        var userFriendList = FirebaseDatabase.DefaultInstance.GetReference("users").Child(UserId).Child("friends");

        mDatabase.Child("friends").ChildAdded += HandleChildAdded;
        mDatabase.Child("friends").ChildRemoved += HandleChildRemoved;
    }

    public void InitRequestReceived()
    {
        var userRequestList = FirebaseDatabase.DefaultInstance.GetReference("users").Child(UserId).Child("requestReceived");

        mDatabase.Child("requestReceived").ChildAdded += HandleChildAddedRequest;
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userAddedToFriendList = (Dictionary<string, object>)args.Snapshot.Value;

        //Spawn label and accept button

        var newLabel = Instantiate(friendLabel, new Vector2(friendLabelPos.position.x, friendLabelPos.position.y), Quaternion.identity);
        newLabel.transform.parent = mainCanva.transform;
        friendText = newLabel.GetComponent<TMP_Text>();
        friendText.text = userAddedToFriendList["username"].ToString();

        //Reset label position for new labels

        friendLabelPos.position = new Vector2(newLabel.transform.position.x, newLabel.transform.position.y - 90);
        newLabel.name = userAddedToFriendList["username"].ToString();
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userRemovedFromFriendList = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userRemovedFromFriendList["username"] + " was removed from your friendlist");
    }

    private void HandleChildAddedRequest(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userAddedToRequest = (Dictionary<string, object>)args.Snapshot.Value;

        ReceiveRequest(userAddedToRequest["username"].ToString());

        //Spawn label and accept button

        var newLabel = Instantiate(requestLabel, new Vector2(userOnlineLabelPos.position.x, userOnlineLabelPos.position.y), Quaternion.identity);
        newLabel.transform.parent = mainCanva.transform;
        requestText = newLabel.GetComponent<TMP_Text>();
        requestText.text = userAddedToRequest["username"].ToString();

        var newAddButton = Instantiate(acceptB, new Vector2(userOnlineLabelPos.position.x + 250, userOnlineLabelPos.position.y), Quaternion.identity);
        newAddButton.transform.parent = mainCanva.transform;

        //Add listener to button

        currentUsername = userAddedToRequest["username"].ToString();
        Button addButton = newAddButton.GetComponent<Button>();
        addButton.onClick.AddListener(AcceptRequest);

        //Reset label position for new labels

        userOnlineLabelPos.position = new Vector2(newLabel.transform.position.x, newLabel.transform.position.y - 90);
        newLabel.name = userAddedToRequest["username"].ToString();
    }

    public void SendRequest(string id, string username)
    {
        mDatabase.Child("users").Child(UserId).Child("requestSend").SetValueAsync(username);
    }

    private void ReceiveRequest(string username)
    {
        mDatabase.Child("users").Child(UserId).Child("requestReceived").SetValueAsync(username);
    }

    private void AcceptRequest()
    {
        mDatabase.Child("users").Child(UserId).Child("friends").SetValueAsync(currentUsername);
    }
}
