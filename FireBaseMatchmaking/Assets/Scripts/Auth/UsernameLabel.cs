using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase.Extensions;

public class UsernameLabel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _label;

    private void Reset()
    {
        _label = GetComponent<TMP_Text>();
    }

    void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthChange;
    }

    private void HandleAuthChange(object sender, EventArgs e)
    {
        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;

        if (currentUser != null)
        {
            SetLabelUsername(currentUser.UserId);
        }

    }

    private void SetLabelUsername(string UserId)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + UserId + "/username")
            .GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                    _label.text = "Null";
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    _label.text = "Logged In as: " + (string)snapshot.Value;

                }
            });
    }
}
