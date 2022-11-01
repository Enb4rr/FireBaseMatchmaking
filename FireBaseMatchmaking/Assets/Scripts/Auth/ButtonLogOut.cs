using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonLogOut : MonoBehaviour
{
    [SerializeField]
    private Button _logOutButton;

    DatabaseReference mDatabase;
    string userID;

    private void Reset()
    {
        _logOutButton = GetComponent<Button>();
    }

    private void Awake()
    {
        _logOutButton.onClick.AddListener(LogOut);
    }

    private void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    public void LogOut()
    {
        SetUserOffline();
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("LogScene");
        Time.timeScale = 1;
    }

    private void SetUserOffline()
    {
        UserData data = new UserData();

        string json = JsonUtility.ToJson(data);

        mDatabase.Child("users-online").Child(userID).SetValueAsync(null);
    }
}
