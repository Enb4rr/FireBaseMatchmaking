using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SocialController : MonoBehaviour
{
    private static SocialController instance;
    [SerializeField] Transform onlineSpawn, friendSpawn, requestSpawn;
    [SerializeField] GameObject onlinePrefab, friendPrefab, requestPrefab;

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
                    var userInstanceText = userInstance.GetComponent<TMP_Text>().text;
                    userInstanceText = item.Value.ToString();
                    userInstance.transform.GetChild(1).gameObject.SetActive(false);
                    userInstance.name = item.Key;
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
                    var userInstanceText = userInstance.GetComponent<TMP_Text>().text;
                    userInstanceText = item.Value.ToString();
                    userInstance.name = item.Key;
                }
            }
        }
    }

    void InstantiteFriends(object sender, ValueChangedEventArgs args)
    {
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
                var friendInstanceText = friendInstance.GetComponent<TMP_Text>().text;
                friendInstanceText = item.Value.ToString();
                friendInstance.name = item.Key;
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
                var requestInstanceText = requestInstance.GetComponent<TMP_Text>().text;
                requestInstanceText = item.Value.ToString();
                requestInstance.name = item.Key;
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
}