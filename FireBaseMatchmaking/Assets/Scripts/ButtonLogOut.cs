using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonLogOut : MonoBehaviour
{
    DatabaseReference mDatabase;
    string userID;

    private void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetUserOnline();
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("LogScene");
        Time.timeScale = 1;
    }

    private void SetUserOnline()
    {
        UserData data = new UserData();

        data.online = false;
        string json = JsonUtility.ToJson(data);

        mDatabase.Child("users").Child(userID).Child("online").SetValueAsync(false);
    }
}
