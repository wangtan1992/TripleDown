
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Image m_logo;

    [SerializeField]
    private CanvasGroup m_logoCanvas;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_progress;

    [SerializeField]
    private Slider m_slider;

    void Awake()
    {
        m_slider.value = 0;
    }

    void Start()
    {
        var tween = DOTween.To(()=> m_logoCanvas.alpha, x => m_logoCanvas.alpha = x, 0.3f, .5f);
        tween.SetEase(Ease.Linear);
        tween.SetLoops(-1, LoopType.Yoyo);

        var tweenSlider = DOTween.To(()=> m_slider.value, x => m_slider.value = x, 1, 2);
        tweenSlider.onUpdate += ()=>
        {
            m_progress.text = string.Format("{0}%", Mathf.CeilToInt(m_slider.value * 100));
        };

        tweenSlider.OnComplete(LoadMainScene);
    }

    void LoadMainScene()
    {
       SceneManager.LoadSceneAsync("Game"); 
    }
}
