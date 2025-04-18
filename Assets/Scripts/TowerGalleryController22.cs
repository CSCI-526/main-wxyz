using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class TowerGalleryController : MonoBehaviour
{
   public GameObject galleryPanel;
   public GameObject[] towerPanels; // 对应五个塔的介绍 UI

   public VideoPlayer sharedVideoPlayer;

   private readonly Dictionary<int, string> videoPaths = new Dictionary<int, string>()
    {
        { 0, "Videos/Cannon.mp4" },
        { 1, "Videos/EnegyTower.mp4" },
        { 2,  "Videos/Gold.mp4"},
        { 3, "Videos/burning.mp4" },
        { 4, "Videos/Frozen.mp4" }
    };

   public void OpenGallery()
   {
      galleryPanel.SetActive(true);
      Time.timeScale = 0f;
   }
   public void CloseGallery()
   {
      galleryPanel.SetActive(false);
      Time.timeScale = 1f;
   }

   public void OpenTowerPanel(int index)
   {
      galleryPanel.SetActive(false);
      towerPanels[index].SetActive(true);

      PlaySharedVideo(index);
   }


   public void ContinueGame(GameObject towerPanel)
   {
      towerPanel.SetActive(false);

      sharedVideoPlayer.Stop();
      ClearVideoRenderTexture();

      Time.timeScale = 1f;
   }

   private void PlaySharedVideo(int index)
    {
        if (!videoPaths.ContainsKey(index)) return;

        string filePath;

#if UNITY_WEBGL && !UNITY_EDITOR
        filePath = Application.streamingAssetsPath + "/" + videoPaths[index];
#else
        filePath = "file://" + Application.streamingAssetsPath + "/" + videoPaths[index];
#endif

        sharedVideoPlayer.Stop();
        ClearVideoRenderTexture();
        sharedVideoPlayer.source = VideoSource.Url;
        sharedVideoPlayer.url = filePath;

        sharedVideoPlayer.Prepare();
        sharedVideoPlayer.prepareCompleted += vp => vp.Play();
    }


   private void ClearVideoRenderTexture()
    {
        if (sharedVideoPlayer != null && sharedVideoPlayer.targetTexture != null)
        {
            RenderTexture rt = sharedVideoPlayer.targetTexture;
            sharedVideoPlayer.targetTexture = null;
            RenderTexture.active = rt;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
            sharedVideoPlayer.targetTexture = rt;
        }
    }
}