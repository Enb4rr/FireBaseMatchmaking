using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ButtonLogIn : MonoBehaviour
{
    [SerializeField]
    private Button _loginButton;
    private Coroutine _loginCoroutine;

    public event Action<FirebaseUser> OnLogInSucceeded;
    public event Action<string> OnLogInFailed;

    DatabaseReference mDatabase;
    string userID;

    private void Reset()
    {
        _loginButton = GetComponent<Button>();
    }

    private void Awake()
    {
        _loginButton.onClick.AddListener(HandleLogInButtonClicked);
    }

    private void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void HandleLogInButtonClicked()
    {
        if (_loginCoroutine == null)
        {
            string email = GameObject.Find("LogInMailField").GetComponent<TMP_InputField>().text;
            string password = GameObject.Find("LogInPasswordField").GetComponent<TMP_InputField>().text;

            _loginCoroutine = StartCoroutine(LoginCoroutine(email, password));
        }
    }

    private IEnumerator LoginCoroutine(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning($"Login failed with {loginTask.Exception}");
            OnLogInFailed?.Invoke($"Failed to register task {loginTask.Exception}");
        }
        else
        {
            Debug.Log($"Login succeeded with {loginTask.Result}");
            OnLogInSucceeded?.Invoke(loginTask.Result);

            SetUserOnline();

            SceneManager.LoadScene("MainScene");
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
