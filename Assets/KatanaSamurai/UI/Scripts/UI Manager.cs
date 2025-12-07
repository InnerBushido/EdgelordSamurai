using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public bool m_ControllerSelected = true;
    public SwordFollow m_SwordFollow;
    public GameObject m_ControllerKatana;
    public GameObject m_VRKatanaPrototype;
    public GameObject m_LeftController;
    public GameObject m_RightController;

    public GameObject m_EnableVerticalCut;
    public GameObject m_EnableDiagonalCut;
    public GameObject m_EnableHorizontalCut;
    
    [Header("Audio")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioSource narrationAudioSource;
    [SerializeField] private AudioClip swooshAudioClip;


    [Header("Start Screen")] 
    [SerializeField] private CanvasGroup startScreenCanvas;
    [SerializeField] private CanvasGroup logoImage;
    [SerializeField] private CanvasGroup buttonObject;
    [SerializeField] private CanvasGroup movieObject;
    [SerializeField] private UnityEvent startScreenStartEvent;
    [SerializeField] private UnityEvent startScreenEndEvent;
    
    [Header("Controller Setup Screen")]
    [SerializeField] private CanvasGroup  controllerSetupCanvas;  
    [SerializeField] private UnityEvent controllerSetupStartEvent;
    [SerializeField] private UnityEvent controllerSetupEndEvent;
    
    [Header("Tutorial Screen")]
    [SerializeField] private CanvasGroup tutorialScreenCanvas;
    [SerializeField] private UnityEvent tutorialScreenStartEvent;
    
    [Header("Before Tutorial Screen")]
    [SerializeField] private UnityEvent beforeTutorialStartEvent;
    [SerializeField] private AudioClip welcomeAudioClip;
    [SerializeField] private AudioClip beforeTutorialAudioClip;
    [SerializeField] private CanvasGroup beforeTutorialCanvas;
    [SerializeField] private UnityEvent beforeTutorialEndEvent;
    
    [Header("Vertical Cut Screen")]
    [SerializeField] private UnityEvent verticalCutStartEvent;
    [SerializeField] private CanvasGroup verticalCutCanvas;
    [SerializeField] private AudioClip verticalCutAudioClip;
    [SerializeField] private UnityEvent verticalCutEndEvent;
    
    [Header("Diagonal Cut Screen")]
    [SerializeField] private UnityEvent diagonalCutStartEvent;
    [SerializeField] private CanvasGroup diagonalCutCanvas;
    [SerializeField] private AudioClip diagonalCutAudioClip;
    [SerializeField] private UnityEvent diagonalCutEndEvent;
    
    [Header("Horizontal Cut Screen")]
    [SerializeField] private UnityEvent horizontalCutStartEvent;
    [SerializeField] private CanvasGroup horizontalCutCanvas;
    [SerializeField] private AudioClip horizontalCutAudioClip;
    [SerializeField] private UnityEvent horizontalCutEndEvent;
    
    [Header("Tutorial End Screen")]
    [SerializeField] private UnityEvent tutorialCompleteStartEvent;
    [SerializeField] private UnityEvent tutorialCompleteEndEvent;
    [SerializeField] private UnityEvent tutorialScreenEndEvent;

    public void StartMovie()
    {
        SceneManager.LoadScene("MovieScene");
    }
    
    public void SetControllerEnabled(bool _enabled)
    {
        m_ControllerSelected = _enabled;
        // EnableControllerOrKatana();
    }
    
    public void EnableControllerOrKatana()
    {
        m_LeftController.SetActive(false);
        m_RightController.SetActive(false);
        
        if (m_ControllerSelected)
        {
            m_ControllerKatana.SetActive(true);
            m_VRKatanaPrototype.SetActive(false);
            m_SwordFollow.m_SwordToFollow = m_ControllerKatana.transform;
        }
        else
        {
            m_ControllerKatana.SetActive(false);
            m_VRKatanaPrototype.SetActive(true);
            m_SwordFollow.m_SwordToFollow = m_VRKatanaPrototype.transform;
        }
    }

    public void DisableControllerOrKatana()
    {
        m_LeftController.SetActive(true);
        m_RightController.SetActive(true);
        
        if (m_ControllerSelected)
        {
            m_ControllerKatana.SetActive(false);
            m_VRKatanaPrototype.SetActive(false);
        }
        else
        {
            m_ControllerKatana.SetActive(false);
            m_VRKatanaPrototype.SetActive(false);
        }
    }

    public enum State
    {
        nan,
        StartScreen,
        ControllerSetup,
        TutorialScreen,
        InGame,
        GameWOver
    }

    public enum TutorialState
    {
        nan,
        BeforeTutorial,
        VerticalCut,
        DiagonalCut,
        HorizontalCut,
        TutorialComplete
    }
    
    public State currentState;
    public TutorialState currentTutorialState;
    
    public void SwitchStateInt(int newStateIndex)
    {
        Debug.Log("SwitchStateInt() - Called with state index: " + newStateIndex);
        SwitchState((State)newStateIndex);
    }
    
    public void SwitchTutorialStateInt(int newTutorialStateIndex)
    {
        Debug.Log("SwitchTutorialStateInt() - Called with tutorial state index: " + newTutorialStateIndex);
        SwitchTutorialState((TutorialState)newTutorialStateIndex);
    }


    private void Start()
    {
        SwitchStateInt(1);
    }

    private void SwitchState(State newState)
    {
         if (currentState == newState)
            {
                Debug.Log("SwitchState() - Already in state: " + newState);
                return;
            }
        
        Debug.Log("SwitchState() - Switching from " + currentState + " to " + newState);
        switch (currentState)
        {
            case State.nan:
                break;
            case State.StartScreen:
                HideStartScreen();
                break;
            case State.ControllerSetup:
                HideControllerSetupScreen();
                break;
            case State.TutorialScreen:
                HideTutorialScreen();
                break;
            case State.InGame:
                HideInGame();
                break;
            case State.GameWOver:
                HideGameOver();
                break;
        }

        switch (newState)
        {
            case State.nan:
                break;
            case State.StartScreen:
                ShowStartScreen();
                break;
            case State.ControllerSetup:
                ShowControllerSetupScreen();
                break;
            case State.TutorialScreen:
                ShowTutorialScreen();
                break;
            case State.InGame:
                ShowInGame();
                break;
            case State.GameWOver:
                ShowGameOver();
                break;
        }

        currentState = newState;
    }
    
    private void SwitchTutorialState(TutorialState newTutorialState)
    {
        if (currentTutorialState == newTutorialState)
        {
            Debug.Log("SwitchTutorialState() - Already in state: " + newTutorialState);
            return;
        }
        Debug.Log("SwitchTutorialState() - Switching from " + currentTutorialState + " to " + newTutorialState);
        switch (currentTutorialState)
        {
            case TutorialState.nan:
                break;
            case TutorialState.BeforeTutorial:
                HideBeforeTutorial();
                break;
            case TutorialState.VerticalCut:
                HideVerticalCut();
                break;
            case TutorialState.DiagonalCut:
                HideDiagonalCut();
                break;
            case TutorialState.HorizontalCut:
                HideHorizontalCut();
                break;
            case TutorialState.TutorialComplete:
                HideTutorialComplete();
                break;
        }

        switch (newTutorialState)
        {
            case TutorialState.nan:
                break;
            case TutorialState.BeforeTutorial:
                StartCoroutine(ShowBeforeTutorial());
                break;
            case TutorialState.VerticalCut:
                ShowVerticalCut();
                break;
            case TutorialState.DiagonalCut:
                ShowDiagonalCut();
                break;
            case TutorialState.HorizontalCut:
                ShowHorizontalCut();
                break;
            case TutorialState.TutorialComplete:
                ShowTutorialComplete();
                break;
        }

        currentTutorialState = newTutorialState;
    }
    
    private void ShowStartScreen()
    {
        Debug.Log("ShowStartScreen() executing");
        startScreenCanvas.gameObject.SetActive(true);
        startScreenCanvas.alpha = 1;
        LeanTween.alphaCanvas(buttonObject, 1f, 1f).setDelay(2f);
        LeanTween.alphaCanvas(movieObject, 1f, 1f).setDelay(2f);
        LeanTween.alphaCanvas(logoImage, 1f, 1f);
        LeanTween.moveLocalX(logoImage.gameObject, 0f, 0.25f).setFrom(-100f).setOnComplete(() =>
        {
        uiAudioSource.PlayOneShot(swooshAudioClip);
        });
        startScreenStartEvent.Invoke();
    }
    
    private void HideStartScreen()
    {
        Debug.Log("HideStartScreen() executing");
        LeanTween.alphaCanvas(startScreenCanvas, 0f, 1f).setOnComplete(() =>
        {
            startScreenCanvas.gameObject.SetActive(false);
            startScreenEndEvent.Invoke();
        });
    }
    
    private void ShowControllerSetupScreen()
    {
        Debug.Log("ShowControllerSetupScreen() executing");
        controllerSetupCanvas.gameObject.SetActive(true);
        LeanTween.alphaCanvas(controllerSetupCanvas, 1f, 1f);
        controllerSetupStartEvent.Invoke();
    }

    private void HideControllerSetupScreen()
    {
        Debug.Log("HideControllerSetupScreen() executing");
        LeanTween.alphaCanvas(controllerSetupCanvas, 0f, 1f).setOnComplete(() =>
        {
            controllerSetupCanvas.gameObject.SetActive(false);
            controllerSetupEndEvent.Invoke();
        });
    }

    private void ShowTutorialScreen()
    {
        Debug.Log("ShowTutorialScreen() executing");
        tutorialScreenCanvas.gameObject.SetActive(true);
        tutorialScreenCanvas.alpha = 1;
        tutorialScreenStartEvent.Invoke();
        SwitchTutorialState(TutorialState.BeforeTutorial);
    }
    
    private IEnumerator ShowBeforeTutorial()
    {
        Debug.Log("ShowBeforeTutorial() executing");
        beforeTutorialCanvas.gameObject.SetActive(true);
        narrationAudioSource.clip = welcomeAudioClip;
        narrationAudioSource.Play();
        yield return new WaitForSeconds(welcomeAudioClip.length + 0.5f);
        narrationAudioSource.clip = beforeTutorialAudioClip;
        narrationAudioSource.Play();
        LeanTween.alphaCanvas(beforeTutorialCanvas, 1f, 1f);
        LeanTween.moveLocalY(beforeTutorialCanvas.gameObject, 0f, 0.25f).setFrom(-100f).setOnComplete(() =>
        {
            uiAudioSource.PlayOneShot(swooshAudioClip);
        });
        beforeTutorialStartEvent.Invoke();
    }

    private void HideBeforeTutorial()
    {
        Debug.Log("HideBeforeTutorial() executing");
        LeanTween.alphaCanvas(beforeTutorialCanvas, 0f, 1f).setOnComplete(() =>
        {
            LeanTween.moveLocalY(beforeTutorialCanvas.gameObject, 100f, 0.25f).setOnComplete(() =>
            {
                uiAudioSource.PlayOneShot(swooshAudioClip);
                beforeTutorialCanvas.gameObject.SetActive(false);
            });
        });
        beforeTutorialEndEvent.Invoke();
    }

    private void ShowVerticalCut()
    {
        Debug.Log("ShowVerticalCut() executing");
        verticalCutCanvas.gameObject.SetActive(true);
        narrationAudioSource.clip = verticalCutAudioClip;
        narrationAudioSource.Play();
        LeanTween.alphaCanvas(verticalCutCanvas, 1f, 1f);
        LeanTween.moveLocalY(verticalCutCanvas.gameObject, 0f, 0.25f).setFrom(-100f).setOnComplete(() =>
        {
            uiAudioSource.PlayOneShot(swooshAudioClip);
        });
        verticalCutStartEvent.Invoke();
    }

    public void HideVerticalCut()
    {
        Debug.Log("HideVerticalCut() executing");
        LeanTween.alphaCanvas(verticalCutCanvas, 0f, 1f).setOnComplete(() =>
        {
            LeanTween.moveLocalY(verticalCutCanvas.gameObject, 100f, 0.25f).setOnComplete(() =>
            {
                uiAudioSource.PlayOneShot(swooshAudioClip);
                verticalCutCanvas.gameObject.SetActive(false);
            });
        });
        verticalCutEndEvent.Invoke();
        
        m_EnableVerticalCut.SetActive(true);
        EnableControllerOrKatana();
        StartCoroutine(EnableDiagonalCut());

    }

    IEnumerator EnableDiagonalCut()
    {
        yield return new WaitForSeconds(5);
        DisableControllerOrKatana();
        m_EnableVerticalCut.SetActive(false);
        SwitchTutorialState(TutorialState.DiagonalCut);
    }

    public void ShowDiagonalCut()
    {
        Debug.Log("ShowDiagonalCut() executing");
        diagonalCutCanvas.gameObject.SetActive(true);
        narrationAudioSource.clip = diagonalCutAudioClip;
        narrationAudioSource.Play();
        LeanTween.alphaCanvas(diagonalCutCanvas, 1f, 1f);
        LeanTween.moveLocalY(diagonalCutCanvas.gameObject, 0f, 0.25f).setFrom(-100f).setOnComplete(() =>
        {
            uiAudioSource.PlayOneShot(swooshAudioClip);
        });
        diagonalCutStartEvent.Invoke();
    }

    public void HideDiagonalCut()
    {
        Debug.Log("HideDiagonalCut() executing");
        LeanTween.alphaCanvas(diagonalCutCanvas, 0f, 1f).setOnComplete(() =>
        {
            LeanTween.moveLocalY(diagonalCutCanvas.gameObject, 100f, 0.25f).setOnComplete(() =>
            {
                uiAudioSource.PlayOneShot(swooshAudioClip);
                diagonalCutCanvas.gameObject.SetActive(false);
            });
        });
        diagonalCutEndEvent.Invoke();
        
        m_EnableDiagonalCut.SetActive(true);
        EnableControllerOrKatana();
        StartCoroutine(EnableHorizontalCut());
    }
    
    IEnumerator EnableHorizontalCut()
    {
        yield return new WaitForSeconds(5);
        DisableControllerOrKatana();
        m_EnableDiagonalCut.SetActive(false);
        SwitchTutorialState(TutorialState.HorizontalCut);
    }

    public void ShowHorizontalCut()
    {
        Debug.Log("ShowHorizontalCut() executing");
        horizontalCutCanvas.gameObject.SetActive(true);
        narrationAudioSource.clip = horizontalCutAudioClip;
        narrationAudioSource.Play();
        LeanTween.alphaCanvas(horizontalCutCanvas, 1f, 1f);
        LeanTween.moveLocalY(horizontalCutCanvas.gameObject, 0f, 0.25f).setFrom(-100f).setOnComplete(() =>
        {
            uiAudioSource.PlayOneShot(swooshAudioClip);
        });
        horizontalCutStartEvent.Invoke();
    }

    public void HideHorizontalCut()
    {
        Debug.Log("HideHorizontalCut() executing");
        LeanTween.alphaCanvas(horizontalCutCanvas, 0f, 1f).setOnComplete(() =>
        {
            LeanTween.moveLocalY(horizontalCutCanvas.gameObject, 100f, 0.25f).setOnComplete(() =>
            {
                uiAudioSource.PlayOneShot(swooshAudioClip);
                horizontalCutCanvas.gameObject.SetActive(false);
            });
        });
        horizontalCutEndEvent.Invoke();
        
        m_EnableHorizontalCut.SetActive(true);
        EnableControllerOrKatana();
        StartCoroutine(EnableTutorialComplete());
    }
    
    IEnumerator EnableTutorialComplete()
    {
        yield return new WaitForSeconds(5);
        DisableControllerOrKatana();
        m_EnableHorizontalCut.SetActive(false);
        SwitchTutorialState(TutorialState.TutorialComplete);
        SwitchState(State.InGame);
        yield return new WaitForSeconds(3);
        if (m_ControllerSelected)
        {
            SceneManager.LoadScene("CuttingSceneWithBall");
        }
        else
        {
            SceneManager.LoadScene("CuttingSceneWithBallWithKatana");
        }
    }

    public void ShowTutorialComplete()
    {
        Debug.Log("ShowTutorialComplete() executing");
        tutorialCompleteStartEvent.Invoke();
    }

    private void HideTutorialComplete()
    {
        Debug.Log("HideTutorialComplete() executing");
        tutorialCompleteEndEvent.Invoke();
    }

    private void HideTutorialScreen()
    {
        Debug.Log("HideTutorialScreen() executing");
        LeanTween.alphaCanvas(tutorialScreenCanvas, 0f, 1f).setOnComplete(() =>
        {
            tutorialScreenCanvas.gameObject.SetActive(false);
            tutorialScreenEndEvent.Invoke();
        });
    }

    private void ShowInGame()
    {
        Debug.Log("ShowInGame() executing");
    }

    private void HideInGame()
    {
        Debug.Log("HideInGame() executing");
    }

    private void ShowGameOver()
    {
        Debug.Log("ShowGameOver() executing");
    }

    private void HideGameOver()
    {
        Debug.Log("HideGameOver() executing");
    }
}
