using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListController : MonoBehaviour
{
    [SerializeField]
    GameObject acceptB, rejectB, deleteB;

    DatabaseReference mDatabase;
    UserOnlineController mUserOnlineController;
    string UserId;

    GameState _GameState;

    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        mUserOnlineController = GameObject.Find("Controller").GetComponent<UserOnlineController>();
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
        _GameState.OnDataReady += InitUsersOnMatchmakingController;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    public void InitUsersOnMatchmakingController()
    {
        Debug.Log("Init user frinedlist");

        var userFriendList = FirebaseDatabase.DefaultInstance.GetReference("users").Child(UserId).Child("friends");

        mDatabase.Child("friends").ChildAdded += HandleChildAdded;
        mDatabase.Child("friends").ChildRemoved += HandleChildRemoved;
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


    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.Log(args.DatabaseError.Message);
            return;
        }

        Dictionary<string, object> friendList = (Dictionary<string, object>)args.Snapshot.Value;

        if (friendList != null)
        {
            foreach (var userDoc in friendList)
            {
                Dictionary<string, object> userOnFriendList = (Dictionary<string, object>)userDoc.Value;
                Debug.Log("FRIEND:" + userOnFriendList["username"]);
            }
        }
    }

    private void AddFriend()
    {
        mDatabase.Child("users").Child(UserId).Child("friends").SetValueAsync(_GameState.username);
    }

    private void RemoveFriend()
    {
        mDatabase.Child("users").Child(UserId).SetValueAsync(null);
    }

    private void SendRequest()
    {
    }

    private void RejectRequest()
    {
    }
}
