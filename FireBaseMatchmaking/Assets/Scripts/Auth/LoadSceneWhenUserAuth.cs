using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneWhenUserAuth : MonoBehaviour
{
    [SerializeField]
    private string _sceneToLoad;

    DatabaseReference mDatabase;
    string userID;

    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChange;
    }

    private void HandleAuthStateChange(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            SetUserOnline();
            SceneManager.LoadScene(_sceneToLoad);
            Time.timeScale = 1;
        }
    }

    private void SetUserOnline()
    {
        UserData data = new UserData();

        string json = JsonUtility.ToJson(data);
        string userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        mDatabase.Child("users-online").Child(userID).Child("username").SetValueAsync(data.username);
    }
}
