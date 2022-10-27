using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListController : MonoBehaviour
{
    [SerializeField]
    GameObject acceptB;

    DatabaseReference mDatabase;
    UserOnlineController mUserOnlineController;
    string UserId;

    GameState _GameState;

    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        mUserOnlineController = GameObject.Find("Controller").GetComponent<UserOnlineController>();
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
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
        Debug.Log("Init user friendlist");

        var userFriendList = FirebaseDatabase.DefaultInstance.GetReference("users").Child(UserId).Child("friends");

        mDatabase.Child("friends").ChildAdded += HandleChildAdded;
        mDatabase.Child("friends").ChildRemoved += HandleChildRemoved;
    }

    public void InitRequestReceived()
    {
        var userRequestList = FirebaseDatabase.DefaultInstance.GetReference("users").Child(UserId).Child("requestReceived");

        mDatabase.Child("requestReceived").ChildAdded += HandleChildAddedRequest;
        mDatabase.Child("requestReceived").ChildRemoved += HandleChildRemovedRequest;
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userAddedToFriendList = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userAddedToFriendList["username"] + " is on your friendlist");
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
        Dictionary<string, object> userAddedToFriendList = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userAddedToFriendList["username"] + " is on your friendlist");
    }

    private void HandleChildRemovedRequest(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userRemovedFromFriendList = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userRemovedFromFriendList["username"] + " was removed from your friendlist");
    }

    public void SendRequest(string id, string username)
    {
        Debug.Log("Request Sent");

        mDatabase.Child("users").Child(id).Child("requestSend").SetValueAsync(_GameState.username);
    }

    private void ReceiveRequest(string username)
    {
        mDatabase.Child("users").Child(UserId).Child("requestReceived").SetValueAsync(username);
    }

    private void AcceptRequest(string username)
    {
        mDatabase.Child("users").Child(UserId).Child("friends").SetValueAsync(username);
    }
}
