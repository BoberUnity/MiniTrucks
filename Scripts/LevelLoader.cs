using System.Collections;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
  [SerializeField] private UILabel loadingText = null;
  [SerializeField] private UILabel loadingProgress = null;
  [SerializeField] private GameObject startButton = null;
  [SerializeField] private GameObject animLoad = null;
  
  private int id = 0;
  private AsyncOperation async;
  private bool loading = false;
  //private bool startButtonActive = false;

  private void Start()
  {
    DontDestroyOnLoad(this);
    StartLoadLevel(1);
  }

  private void OnDestroy()
  {
    
  }
  
  private void StartLoadLevel(int lev)
  {
    if (!loading)
    {
      if (loadingText != null)
        loadingText.gameObject.SetActive(true);
      if (loadingProgress != null)
        loadingProgress.gameObject.SetActive(true);
      id = lev;
      StartCoroutine(Load());
      loading = true;
    }
  }

  private void Update()
  {
    if (loadingProgress != null && async != null)
    {
      loadingProgress.text = (async.progress * 100).ToString("f0") + " %";
      if (async.progress > 0.9999f)
      {
        if (loadingText != null)
          loadingText.gameObject.SetActive(false);
        if (loadingProgress != null)
          loadingProgress.gameObject.SetActive(false);
        if (startButton != null)
          startButton.SetActive(true);
        if (animLoad != null)
          animLoad.SetActive(false);
        Destroy(gameObject);
      }
      //loadAnim[loadAnim.clip.name].time = async.progress;
    }
  }

  private IEnumerator Load()
  {
    async = Application.LoadLevelAsync(id);
    yield return async;
  }
}



  

  

