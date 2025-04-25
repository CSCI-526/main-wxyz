using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

public class FirebaseManager : MonoBehaviour
{
    // public static FirebaseManager Instance { get; private set; }

    private string databaseURL = "https://mergedefense-260f0-default-rtdb.firebaseio.com/";

    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    public void SaveData(string json)
    {
        // StartCoroutine(PostRequest(databaseURL + "AlphaPlayTestData.json", json));
        //StartCoroutine(PostRequest(databaseURL + "BetaPlayTestData.json", json));
        // StartCoroutine(PostRequest(databaseURL + "BetaPlayTestData1.json", json));
        // StartCoroutine(PostRequest(databaseURL + "BetaPlayTestData2.json", json));
        // StartCoroutine(PostRequest(databaseURL + "BetaPlayPublishData.json", json));
         StartCoroutine(PostRequest(databaseURL + "GoldPlayTestData.json", json));
        // StartCoroutine(PostRequest(databaseURL + "GoldPlayTestData1.json", json));
        // StartCoroutine(PostRequest(databaseURL + "GoldPlayTestData2.json", json));
        // StartCoroutine(PostRequest(databaseURL + "GoldPlayPublishData.json", json));
    }

    public void ReadData()
    {
        StartCoroutine(GetRequest(databaseURL + "BetaRankListTestData.json"));
        // StartCoroutine(GetRequest(databaseURL + "BetaRankListPublishData.json"));
        // StartCoroutine(GetRequest(databaseURL + "GoldRankListTestData.json"));
        // StartCoroutine(GetRequest(databaseURL + "GoldRankListPublishData.json"));
    }

    public void RepalceData(string json)
    {
        StartCoroutine(PutRequest(databaseURL + "BetaRankListTestData.json", json));
        // StartCoroutine(PutRequest(databaseURL + "BetaRankListPublishData.json", json));
        // StartCoroutine(PutRequest(databaseURL + "GoldRankListTestData.json"));
        // StartCoroutine(PutRequest(databaseURL + "GoldRankListPublishData.json"));
    }


    IEnumerator PostRequest(string url, string json)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                Debug.Log("数据成功上传: " + request.downloadHandler.text);
            else
                Debug.LogError("上传失败: " + request.error);
        }
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Data successfully retrieved: " + request.downloadHandler.text);
                string jsonString = request.downloadHandler.text.Replace("[null,", "[");
                PlayerPrefs.SetString("RankList", jsonString);
                PlayerPrefs.Save();
            }
            else
                Debug.LogError("Request failed: " + request.error);
        }
    }

    IEnumerator PutRequest(string url, string json)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Data successfully uploaded: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Upload failed: " + request.error);
            }
        }
    }
}