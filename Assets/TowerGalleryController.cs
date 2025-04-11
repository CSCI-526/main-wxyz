using UnityEngine;
using UnityEngine.Video;

public class TowerGalleryController : MonoBehaviour
{
   public GameObject galleryPanel;
   public GameObject[] towerPanels; // 对应五个塔的介绍 UI

   public VideoPlayer sharedVideoPlayer;

   public VideoClip[] towerVideoClips;

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

      sharedVideoPlayer.Stop();
      ClearVideoRenderTexture();
      sharedVideoPlayer.clip = towerVideoClips[index];
      sharedVideoPlayer.time = 0;
      sharedVideoPlayer.Play();
   }


   public void ContinueGame(GameObject towerPanel)
   {
      towerPanel.SetActive(false);

      sharedVideoPlayer.Stop();
      ClearVideoRenderTexture();

      Time.timeScale = 1f;
   }

   private void ClearVideoRenderTexture()
   {
      RenderTexture rt = sharedVideoPlayer.targetTexture;
      sharedVideoPlayer.targetTexture = null;
      RenderTexture.active = rt;
      GL.Clear(true, true, Color.clear);
      RenderTexture.active = null;
      sharedVideoPlayer.targetTexture = rt;
   }
}