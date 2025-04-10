using UnityEngine;

public class TowerGalleryController : MonoBehaviour
{
   public GameObject galleryPanel;
   public GameObject[] towerPanels; // 对应五个塔的介绍 UI

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
   }

   public void ContinueGame(GameObject towerPanel)
   {
      towerPanel.SetActive(false);
      Time.timeScale = 1f;
   }
}
