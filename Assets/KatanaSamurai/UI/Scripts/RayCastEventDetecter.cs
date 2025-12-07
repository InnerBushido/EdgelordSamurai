using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class RayCastEventDetecter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private RectTransform _rectTransform;

    private Button btn;

    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;
    [SerializeField] float hoverScaleFactor;
    [SerializeField] float pressedScaleFactor;
    [SerializeField] AudioClip hover;
    [SerializeField] AudioClip pressed;

    private Vector3 _initialScale;
    private int _tweenIndex;


    private void Awake()
    {
        btn = GetComponent<Button>();   
        _rectTransform = GetComponent<RectTransform>();
        _initialScale  = _rectTransform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (btn != null && !btn.interactable) return; 
        if(pressed) AudioSource.PlayClipAtPoint(pressed, Vector3.one, 0.75f);
        LeanTween.cancel(_tweenIndex);
        _tweenIndex = LeanTween.scale(gameObject, _initialScale + Vector3.one * pressedScaleFactor, tweenTime).setEase(tweenType).id;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btn != null && !btn.interactable) return;
        if (hover) AudioSource.PlayClipAtPoint(hover, Vector3.one, 0.75f);
        LeanTween.cancel(_tweenIndex);
        _tweenIndex = LeanTween.scale(gameObject, _initialScale +  Vector3.one * hoverScaleFactor, tweenTime).setEase(tweenType).id;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (btn != null && !btn.interactable) return;
        LeanTween.cancel(_tweenIndex);
        _tweenIndex = LeanTween.scale(gameObject, _initialScale, tweenTime).setEase(tweenType).id;
    }
}
