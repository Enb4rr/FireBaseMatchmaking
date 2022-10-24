using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserOnlineController : MonoBehaviour
{
    DatabaseReference mDatabase;
    string UserId;

    GameState _GameState;

    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
        _GameState.OnDataReady += InitUsersOnlineController;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

    }

    public void InitUsersOnlineController()
    {
        //FirebaseDatabase.DefaultInstance.LogLevel = LogLevel.Verbose;

        Debug.Log("Init users online controller");

        var userOnlineRef = FirebaseDatabase.DefaultInstance.GetReference("users-online");

        mDatabase.Child("users-online").ChildAdded += HandleChildAdded;
        mDatabase.Child("users-online").ChildRemoved += HandleChildRemoved;

        SetUserOnline();
    }
    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userConnected = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userConnected["username"] + " is online");
    }
    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userDisconnected = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userDisconnected["username"] + " is offline");
    }


    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.Log(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> usersList = (Dictionary<string, object>)args.Snapshot.Value;

        if (usersList != null)
        {
            foreach (var userDoc in usersList)
            {
                Dictionary<string, object> userOnline = (Dictionary<string, object>)userDoc.Value;
                Debug.Log("ONLINE:" + userOnline["username"]);

            }
        }

    }
    private void SetUserOnline()
    {
        mDatabase.Child("users-online").Child(UserId).Child("username").SetValueAsync(_GameState.username);
    }
    private void SetUserOffline()
    {
        mDatabase.Child("users-online").Child(UserId).SetValueAsync(null);
    }

    void OnApplicationQuit()
    {
        SetUserOffline();
    }
}
