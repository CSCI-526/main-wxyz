using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class FirebaseManager : MonoBehaviour
{
    private string databaseURL = "https://mergedefense-260f0-default-rtdb.firebaseio.com/";

    public void SaveData(string json)
    {
        // string json = "{\"name\":\"Player1\",\"score\":100}"; // 你可以改成动态数据
        StartCoroutine(PostRequest(databaseURL + "AlphaPlayTestData.json", json));
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
}
