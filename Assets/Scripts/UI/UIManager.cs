using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIManger : MonoBehaviour
{
    private UIDocument _document;
    private EventCallback<ClickEvent> _playButtonClick;

    protected virtual void Awake()
    {
        _document = GetComponent<UIDocument>();
    }

    public void ShowUI()
    {
        _document.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void HideUI()
    {
        _document.rootVisualElement.style.display = DisplayStyle.None;
    }

    protected T GetElement<T>(string elementName) where T : VisualElement // equivalent of T extends VisualElement
    {
        return _document.rootVisualElement.Q<T>(elementName);
    }

    protected void ShowElement(VisualElement element)
    {
        element.style.display = DisplayStyle.Flex;
    }
    
    protected void HideElement(VisualElement element)
    {
        element.style.display = DisplayStyle.None;
    }

    protected void ElementFadeOut<T>(T element, float time) where T : VisualElement
    {
        StartCoroutine(ElementFadeOutRoutine(element, time));
    }
    
    private IEnumerator ElementFadeOutRoutine<T>(T element, float time) where T : VisualElement
    {
        element.style.opacity = 1f; // make sure it's fully opaque to start
        for (float i = 100; i > -1; i--)
        {
            element.style.opacity = i / 100f;
            yield return new WaitForSecondsRealtime(time / 101);
        }
        HideElement(element);
    }
    
    protected void ElementFadeIn<T>(T element, float time) where T  : VisualElement
    {
        StartCoroutine(ElementFadeInRoutine(element, time));
    }

    private IEnumerator ElementFadeInRoutine<T>(T element, float time) where T : VisualElement
    {
        ShowElement(element);
        element.style.opacity = 0f; // make sure it's fully transparent to start
        for (float i = 0; i < 101; i++)
        {
            element.style.opacity = i / 100f;
            yield return new WaitForSecondsRealtime(time / 101);
        }
    }
}
