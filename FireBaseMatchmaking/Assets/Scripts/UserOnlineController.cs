using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserOnlineController : MonoBehaviour
{
    DatabaseReference mDatabase;
    string UserId;

    GameState _GameState;

    [SerializeField]
    public GameObject friendLabel, addB;
    public Transform userOnlineLabelPos;
    private TMP_Text friendLabelText;

    [SerializeField]
    private GameObject mainCanva;

    private List<GameObject> mOnline = new List<GameObject>();

    public delegate void OnReceiveData(string id, string username);
    public event OnReceiveData onSendData;

    public string currentUsername;
    public string currentId;

    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState = GameObject.Find("Controller").GetComponent<GameState>();
        _GameState.OnDataReady += InitUsersOnlineController;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    public void InitUsersOnlineController()
    {
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

        var newLabel = Instantiate(friendLabel, new Vector2(userOnlineLabelPos.position.x, userOnlineLabelPos.position.y), Quaternion.identity);
        newLabel.transform.parent = mainCanva.transform;
        friendLabelText = newLabel.GetComponent<TMP_Text>();
        friendLabelText.text = userConnected["username"].ToString();

        var newAddButton = Instantiate(addB, new Vector2(userOnlineLabelPos.position.x + 250, userOnlineLabelPos.position.y), Quaternion.identity);
        newAddButton.transform.parent = mainCanva.transform;

        currentUsername = userConnected["username"].ToString();
        currentId = UserId;
        Button addButton = newAddButton.GetComponent<Button>();
        addButton.onClick.AddListener(SendData);

        userOnlineLabelPos.position = new Vector2(newLabel.transform.position.x, newLabel.transform.position.y - 90);
        newLabel.name = userConnected["username"].ToString();
        mOnline.Add(newLabel);
    }
    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> userDisconnected = (Dictionary<string, object>)args.Snapshot.Value;

        SetUserOffline();

        //foreach(GameObject label in mOnline)
        //{
        //    if(label.name == userDisconnected["username"].ToString())
        //    {
        //        //label.SetActive(false);
        //    }
        //}
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

    public void SendData()
    {
        onSendData?.Invoke(currentId, currentUsername);
    }
}
