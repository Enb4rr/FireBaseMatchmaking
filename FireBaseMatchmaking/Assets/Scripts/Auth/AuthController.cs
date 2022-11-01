using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthController : MonoBehaviour
{
    public static AuthController instance;

    DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public static FirebaseUser User;
    public static DatabaseReference mDatabase;

    [SerializeField] private TMP_InputField emailLogin, passwordLogin;
    [SerializeField] private TMP_InputField usernameRegister, emailRegister, passwordRegister;
    [SerializeField] private TMP_Text _alertText;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != null) Destroy(this);
        dependencyStatus = DependencyStatus.Available;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    private void InitFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        User = auth.CurrentUser;
    }

    public void LoginButton()
    {
        StartCoroutine(LoginUser(emailLogin.text, passwordLogin.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(RegisterUser(emailRegister.text, passwordRegister.text, usernameRegister.text));
    }

    private IEnumerator LoginUser(string _email, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(() => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            var newAlert = "Failed to Login";
            StartCoroutine(SetAlertText(newAlert));
        }
        else
        {
            User = LoginTask.Result;
            UpdateStatus(true);
            SceneManager.LoadScene(1);
        }
    }

    private IEnumerator RegisterUser(string _email, string _password, string _username)
    {
        if (_username == null)
        {
            var newAlert = "Username Required";
            StartCoroutine(SetAlertText(newAlert));
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                var newAlert = "UFailed to Register User";
                StartCoroutine(SetAlertText(newAlert));
            }
            else
            {
                User = registerTask.Result;
                if (User == null) yield break;
                UserProfile profile = new UserProfile { DisplayName = _username };

                var profileTask = User.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(() => profileTask.IsCompleted);

                if (profileTask.Exception != null)
                {
                    var newAlert = "Failed to set Username";
                    StartCoroutine(SetAlertText(newAlert));
                }
                else
                {
                    Debug.Log("Succesfully registered user");

                    UserData data = new UserData();

                    data.username = GameObject.Find("RegisterUsernameField").GetComponent<TMP_InputField>().text;
                    string json = JsonUtility.ToJson(data);

                    FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(registerTask.Result.UserId).SetRawJsonValueAsync(json);

                    UpdateStatus(true);
                    SceneManager.LoadScene(1);
                }
            }
        }
    }

    public void UpdateStatus(bool isOnline)
    {
        StartCoroutine(isOnline? SetOnline() : SetOffline());
    }

    IEnumerator SetOnline()
    {
        var databaseTask = AuthController.mDatabase.Child("users-online")
            .Child(AuthController.User.UserId).SetValueAsync(AuthController.User.DisplayName);

        yield return new WaitUntil(() => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            var newAlert = AuthController.User.UserId + " is online";
            StartCoroutine(SetAlertText(newAlert));
        }
    }

    IEnumerator SetOffline()
    {
        var databaseTask = AuthController.mDatabase.Child("users-online").Child(AuthController.User.UserId).RemoveValueAsync();

        yield return new WaitUntil(() => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            var newAlert = AuthController.User.UserId + " is offline";
            StartCoroutine(SetAlertText(newAlert));
        }
    }

    private IEnumerator SetAlertText(string text)
    {
        _alertText.gameObject.SetActive(true);
        _alertText.text = text;

        yield return new WaitForSeconds(1f);

        _alertText.gameObject.SetActive(false);
    }
}
