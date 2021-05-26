using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUINavigation : MonoBehaviour
{
    public Transform[] allChildPages;
    public Stack<Transform> navigationHistory = new Stack<Transform>();
    public CanvasGroup canvasGroup;
    public TransitionState transitionState;
    protected Transform targetPage;
    [SerializeField] float transitionSpeed;

    public enum TransitionState
    {
        none,
        fadeIn,
        fadeTo,
        backOut,
        close,
    }

    public virtual void Init()
    {
        navigationHistory.Clear();
        canvasGroup.alpha = 0;
        ShowPage(GetPage("Menu"));
        transitionState = TransitionState.fadeIn;

        GoSlowMo();

    }

    public void GoSlowMo()
    {
        MiniGameGlobalRef.SetGameState(MiniGameGlobalRef.GameState.shop);
        Time.timeScale = 0.25f;
    }

    public void ResetTimeScale()
    {
        MiniGameGlobalRef.ResetGameState();
        Time.timeScale = 1f;
    }

    public float ConstantTimeScale
    {
        get { return transitionSpeed * (Time.deltaTime * (1 / Time.timeScale)); }
    }

    public virtual void Run()
    {
        if (transitionState != TransitionState.none)
        {
            switch (transitionState)
            {

                case TransitionState.fadeIn:
                    canvasGroup.alpha += ConstantTimeScale;
                    navigationHistory.Peek().transform.position = canvasGroup.transform.position + Vector3.up * Screen.height * 0.2f * (1 - canvasGroup.alpha);
                    if (canvasGroup.alpha >= 1)
                    {
                        transitionState = TransitionState.none;
                    }
                    break;

                case TransitionState.fadeTo:
                    canvasGroup.alpha -= ConstantTimeScale;
                    navigationHistory.Peek().transform.position = canvasGroup.transform.position - Vector3.up * Screen.height * 0.2f * (1 - canvasGroup.alpha);
                    if (canvasGroup.alpha <= 0)
                    {
                        transitionState = TransitionState.fadeIn;
                        ShowPage(targetPage);
                    }
                    break;

                case TransitionState.backOut:
                    canvasGroup.alpha -= ConstantTimeScale;
                    navigationHistory.Peek().transform.position = canvasGroup.transform.position - Vector3.up * Screen.height * 0.2f * (1 - canvasGroup.alpha);
                    if (canvasGroup.alpha <= 0)
                    {
                        transitionState = TransitionState.fadeIn;
                        BackImmediate();
                    }
                    break;

                case TransitionState.close:
                    canvasGroup.alpha -= ConstantTimeScale;
                    navigationHistory.Peek().transform.position = canvasGroup.transform.position - Vector3.up * Screen.height * 0.2f * (1 - canvasGroup.alpha);
                    if (canvasGroup.alpha <= 0)
                    {
                        transitionState = TransitionState.none;
                        Close();
                    }
                    break;

                case TransitionState.none:
                    break;

                default:
                    break;
            }
        }
    }

    public virtual void ButtonGoTo(string page)
    {
        switch (page)
        {
            case "exit":
                canvasGroup.alpha = 1;
                transitionState = TransitionState.close;
                break;

            default:
                targetPage = GetPage(page);
                transitionState = TransitionState.fadeTo;
                break;
        }
    }

    public virtual void ShowPage(Transform page)
    {
        if (navigationHistory.Count > 0) navigationHistory.Peek().gameObject.SetActive(false);

        switch (page.name)
        {
            case "Menu":
                page.gameObject.SetActive(true);
                navigationHistory.Push(page);
                break;

            case "Trade":
                page.gameObject.SetActive(true);
                navigationHistory.Push(page);
                break;

            case "Errands":
                page.gameObject.SetActive(true);
                navigationHistory.Push(page);
                break;

            default:
                break;
        }
    }

    public virtual void GoBack()
    {
        transitionState = TransitionState.backOut;
    }

    public void BackImmediate()
    {
        navigationHistory.Peek().gameObject.SetActive(false);
        navigationHistory.Pop();
        navigationHistory.Peek().gameObject.SetActive(true);
    }

    public virtual void PassData(object data, string id)
    {
        switch (id)
        {

            default:
                break;
        }
    }

    public virtual Transform GetPage(string pageName)
    {
        for (int i = 0; i < allChildPages.Length; i++)
        {
            if (allChildPages[i].gameObject.name == pageName)
            {
                return allChildPages[i];
            }
        }
        return null;
    }

    public virtual void Close()
    {
        while (navigationHistory.Count > 0)
        {
            navigationHistory.Peek().gameObject.SetActive(false);
            navigationHistory.Pop();
        }
        ResetTimeScale();
    }

}