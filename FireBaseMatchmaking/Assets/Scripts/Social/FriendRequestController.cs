using System.Collections;
using UnityEngine;

public class FriendRequestController : MonoBehaviour
{
    public void SendFriendRequest()
    {
        StartCoroutine(IESendFriendRequest(gameObject.name));
    }

    public void DeleteFriendRequest()
    {
        StartCoroutine(IEDeleteFriendRequest(gameObject.name));
    }

    public void AddFriend()
    {
        StartCoroutine(IEAddFriend(gameObject.name));
    }

    IEnumerator IESendFriendRequest(string userId)
    {
        var databaseTask = AuthController.mDatabase.Child("users").Child(userId).Child("friend-requests")
            .Child(AuthController.User.UserId).SetValueAsync(AuthController.User.DisplayName);

        yield return new WaitUntil(() => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
    }
    IEnumerator IEDeleteFriendRequest(string userId)
    {
        var databaseTask = AuthController.mDatabase.Child("users").Child(AuthController.User.UserId)
            .Child("friend-requests").Child(userId).RemoveValueAsync();

        yield return new WaitUntil(() => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            Debug.Log("Friend request from " + AuthController.User.DisplayName + " deleted");
        }
    }
    IEnumerator IEAddFriend(string userId)
    {
        var databaseTask_1 = AuthController.mDatabase.Child("users").Child(AuthController.User.UserId)
            .Child("friends").Child(userId).SetValueAsync(AuthController.User.DisplayName);

        yield return new WaitUntil(() => databaseTask_1.IsCompleted);

        if (databaseTask_1.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask_1.Exception}");
        }
        else
        {
            var databaseTask_2 = AuthController.mDatabase.Child("users").Child(userId).Child("friends")
                .Child(AuthController.User.UserId).SetValueAsync(AuthController.User.DisplayName);
            yield return new WaitUntil(() => databaseTask_2.IsCompleted);

            if (databaseTask_2.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {databaseTask_2.Exception}");
            }
            else
            {
                StartCoroutine(IEDeleteFriendRequest(gameObject.name));
            }
        }
    }
}
