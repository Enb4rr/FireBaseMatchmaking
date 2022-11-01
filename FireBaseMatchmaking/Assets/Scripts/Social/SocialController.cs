using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SocialController : MonoBehaviour
{
    private static SocialController instance;
    [SerializeField] Transform onlineSpawn, friendSpawn, requestSpawn;
    [SerializeField] GameObject onlinePrefab, friendPrefab, requestPrefab;

    TMP_Text text;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != null) Destroy(this);
    }

    private void Start()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users-online").ValueChanged += InstantiteUserOnline;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + AuthController.User.UserId + "/friends").ValueChanged += InstantiteFriends;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + AuthController.User.UserId + "/friend-requests").ValueChanged += InstantiteFriendRequest;
    }

    public void InstantiteUserOnline(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
        }
        else
        {
            for (int i = 0; i < onlineSpawn.childCount; i++)
            {
                Destroy(onlineSpawn.GetChild(i).gameObject);
            }

            Dictionary<string, object> currentUsers = (Dictionary<string, object>)args.Snapshot.Value;

            foreach (var item in currentUsers)
            {
                if (item.Key == AuthController.User.UserId)
                {
                    var userInstance = Instantiate(onlinePrefab, onlineSpawn);
                    text = userInstance.GetComponent<TMP_Text>();
                    text.text = item.Value.ToString();

                    userInstance.transform.GetChild(0).gameObject.SetActive(false);
                    userInstance.name = item.Key;

                    //onlineSpawn.position = new Vector2(userInstance.transform.position.x, userInstance.transform.position.y - 80);
                }
                else
                {
                    var userInstance = Instantiate(onlinePrefab, onlineSpawn);
                    for (int i = 0; i < friendSpawn.childCount; i++)
                    {
                        if (friendSpawn.GetChild(i).name == item.Key)
                        {
                            userInstance.transform.GetChild(0).gameObject.SetActive(false);
                        }
                    }
                    text = userInstance.GetComponent<TMP_Text>();
                    text.text = item.Value.ToString();
                    userInstance.name = item.Key;
                    userInstance.transform.GetChild(0).name = item.Key;

                    //onlineSpawn.position = new Vector2(userInstance.transform.position.x, userInstance.transform.position.y - 80);
                }
            }
        }
    }

    void InstantiteFriends(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("Instantiate Friends");
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
        }
        else if (args.Snapshot.Value != null)
        {
            for (int i = 0; i < friendSpawn.childCount; i++)
            {
                Destroy(friendSpawn.GetChild(i).gameObject);
            }
            Dictionary<string, object> currentFriends = (Dictionary<string, object>)args.Snapshot.Value;
            foreach (var item in currentFriends)
            {
                var friendInstance = Instantiate(friendPrefab, friendSpawn);
                text = friendInstance.GetComponent<TMP_Text>();
                text.text = item.Value.ToString();
                friendInstance.name = item.Key;

                //friendSpawn.position = new Vector2(friendInstance.transform.position.x, friendInstance.transform.position.y - 80);
            }
        }
        else
        {
            for (int i = 0; i < friendSpawn.childCount; i++)
            {
                Destroy(friendSpawn.GetChild(i).gameObject);
            }
        }
    }

    void InstantiteFriendRequest(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("Instantiate Friends Requests");
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
        }
        else if (args.Snapshot.Value != null)
        {
            for (int i = 0; i < requestSpawn.childCount; i++)
            {
                Destroy(requestSpawn.GetChild(i).gameObject);
            }

            Dictionary<string, object> currentRequest = (Dictionary<string, object>)args.Snapshot.Value;

            foreach (var item in currentRequest)
            {
                var requestInstance = Instantiate(requestPrefab, requestSpawn);
                text = requestInstance.GetComponent<TMP_Text>();
                text.text = item.Value.ToString();
                requestInstance.name = item.Key;

                //requestSpawn.position = new Vector2(requestInstance.transform.position.x, requestInstance.transform.position.y - 80);
            }
        }
        else
        {
            for (int i = 0; i < requestSpawn.childCount; i++)
            {
                Destroy(requestSpawn.GetChild(i).gameObject);
            }
        }
    }

    public void LogoutUser()
    {
        AuthController.instance.UpdateStatus(false);
        AuthController.instance.auth.SignOut();
        SceneManager.LoadScene(0);
    }

    private void OnApplicationQuit()
    {
        AuthController.instance.UpdateStatus(false);
        AuthController.instance.auth.SignOut();
        SceneManager.LoadScene(0);
    }
}