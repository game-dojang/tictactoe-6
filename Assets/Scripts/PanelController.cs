using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class PanelController : MonoBehaviour
{
    // 팝업 패널의 RectTransform 참조
    [SerializeField] private RectTransform panelTransform;

    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // 팝업 표시
    public void Show()
    {
        Debug.Log("Show panel");

        // 패널 일단 숨기기
        _canvasGroup.alpha = 0;
        panelTransform.localScale = Vector3.zero;

        _canvasGroup.DOFade(1, 0.3f).SetEase(Ease.Linear);
        panelTransform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    // 팝업 숨기기
    public void Hide()
    {
        Debug.Log("Hide panel");

        _canvasGroup.DOFade(0, 0.3f).SetEase(Ease.Linear);
        panelTransform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(gameObject);        
        });
    }
}
