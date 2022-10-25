using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingController : MonoBehaviour
{
    [SerializeField]
    Button matchmakingB, cancelMatchmakingB;

    DatabaseReference mDatabase;
    string UserId;

    GameState _GameState;

    private void Awake()
    {
        matchmakingB = GameObject.Find("MatchmakingB").GetComponent<Button>();
        cancelMatchmakingB = GameObject.Find("CancelMatchmakingB ").GetComponent<Button>();
    }

    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
        _GameState.OnDataReady += InitUsersOnMatchmakingController;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    public void InitMatchmaking()
    {
        matchmakingB.gameObject.SetActive(false);
        cancelMatchmakingB.gameObject.SetActive(true);

        SetUserMatchmaking();
    }

    public void CancelMatchmaking()
    {
        cancelMatchmakingB.gameObject.SetActive(false);
        matchmakingB.gameObject.SetActive(true);

        SetUserOffMatchmaking();
    }

    public void InitUsersOnMatchmakingController()
    {
        Debug.Log("Init users on matchmaking controller");

        var userOnMatchmakingRef = FirebaseDatabase.DefaultInstance.GetReference("matchmaking-queue");

        mDatabase.Child("matchmaking-queue").ChildAdded += HandleChildAdded;
        mDatabase.Child("matchmaking-queue").ChildRemoved += HandleChildRemoved;
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userConnectedToQueue = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userConnectedToQueue["username"] + " is on a queue");
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userDisconnectedFromQueue = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userDisconnectedFromQueue["username"] + " is off the queue");
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
                Dictionary<string, object> userOnQueue = (Dictionary<string, object>)userDoc.Value;
                Debug.Log("ONLINE:" + userOnQueue["username"]);
            }
        }
    }

    private void SetUserMatchmaking()
    {
        mDatabase.Child("matchmaking-queue").Child(UserId).Child("username").SetValueAsync(_GameState.username);
    }

    private void SetUserOffMatchmaking()
    {
        mDatabase.Child("matchmaking-queue").Child(UserId).SetValueAsync(null);
    }

    private void SetGameMatch()
    {
        mDatabase.Child("match").Child(UserId).Child("username").SetValueAsync(_GameState.username);
    }

    private void UnsetGameMatch()
    {
        mDatabase.Child("match").Child(UserId).SetValueAsync(null);
    }

    void OnApplicationQuit()
    {
        SetUserOffMatchmaking();
        UnsetGameMatch();
    }
}
