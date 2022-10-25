using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRegister : MonoBehaviour
{
    [SerializeField]
    private Button _registrationButton;
    private Coroutine _registrationCoroutine;

    public event Action<FirebaseUser> OnUserRegistered;
    public event Action<string> OnUserRegistrationFailed;

    [SerializeField]
    private TMP_Text _alertText;

    private void Reset()
    {
        _registrationButton = GetComponent<Button>();
    }

    private void Awake()
    {
        _registrationButton.onClick.AddListener(HandleRegistrationButtonClick);
    }

    private void HandleRegistrationButtonClick()
    {
        string email = GameObject.Find("RegisterMailField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("RegisterPasswordField").GetComponent<TMP_InputField>().text;
        _registrationCoroutine = StartCoroutine(RegisterUser(email, password));
    }

    private IEnumerator RegisterUser(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            var newAlert = $"Failed to register";
            StartCoroutine(SetAlertText(newAlert));
            OnUserRegistrationFailed?.Invoke($"Failed to register task {registerTask.Exception}");
        }
        else
        {
            Debug.Log($"Succesfully registered user {registerTask.Result.Email}");

            UserData data = new UserData();

            data.username = GameObject.Find("RegisterUsernameField").GetComponent<TMP_InputField>().text;
            string json = JsonUtility.ToJson(data);

            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(registerTask.Result.UserId).SetRawJsonValueAsync(json);


            OnUserRegistered?.Invoke(registerTask.Result);
        }

        _registrationCoroutine = null;
    }

    private IEnumerator SetAlertText(string text)
    {
        _alertText.gameObject.SetActive(true);
        _alertText.text = text;

        yield return new WaitForSeconds(1f);

        _alertText.gameObject.SetActive(false);
    }
}
