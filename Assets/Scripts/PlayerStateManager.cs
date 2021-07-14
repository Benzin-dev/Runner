using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public enum State { Running, Dying, Transcending, Menu }

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] State state = State.Menu;
    [SerializeField] GameObject deathParticle;
    [SerializeField] ParticleSystem winParticle;
    [SerializeField] int numberOfWinParticles = 10;
    [SerializeField] float delayBetweenWinParticles = 0.2f;

    private UnityEvent myEvent;
    private PlayerController pc;
    private RagdollController rc;
    private Animator animator;

    private int currentSceneIndex;

    private void Awake()
    {
        
    }

    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        animator = GetComponent<Animator>();
        rc = GetComponent<RagdollController>();
        pc = GetComponent<PlayerController>();

        if (myEvent == null)
            myEvent = new UnityEvent();

        myEvent.AddListener(GameState);
        myEvent.Invoke();
    }

    
    void Update()
    {
        
    }



    private void GameState()
    {
        switch (state)
        {
            case State.Running:
                StartCoroutine(StartLevelCoroutine(3f));
                break;
            case State.Dying:
                StartDeathSequence();
                break;
            case State.Transcending:
                StartSuccessSequence();
                break;
            case State.Menu:
                StartMenuSequence();
                break;
            default:
                break;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            Instantiate(deathParticle, hit.point, Quaternion.identity);
            
            Debug.Log("Obstacle collider");
            state = State.Dying;
            myEvent.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            StartCoroutine(PlayWinParticles(delayBetweenWinParticles));
            Debug.Log("Finnish trigger");
            state = State.Transcending;
            myEvent.Invoke();
        }
    }

    private IEnumerator PlayWinParticles(float delay)
    {
        Vector3 offset = new Vector3(0f, 2f, 0f);
        for (int i = 0; i < numberOfWinParticles; i++)
        {
            Instantiate(winParticle, transform.position + (Random.insideUnitSphere * 2f) + offset, Quaternion.identity, transform);
            yield return new WaitForSeconds(delay);
        }
    }


    private IEnumerator StartLevelCoroutine(float startInSeconds)
    {
        GameManager.instance.backgroundPanel.SetActive(false);
        //Time.timeScale = 1f;
        yield return new WaitForSeconds(startInSeconds);
        Debug.Log("StartLevelCoroutine");
        pc.isRunning = true;
    }

    private void StartSuccessSequence()
    {
        Debug.Log("Win sequence");
        pc.isRunning = false;
        animator.SetTrigger("win");
        //menuManager.ShowWinMenu();
        Invoke("LoadNextScene", 5f);
    }

    private void StartDeathSequence()
    {
        Debug.Log("Dead");
        pc.isRunning = false;
        rc.DoRagdoll(true);
        Invoke("LoadDefaultScene", 4f);
    }

    private void StartMenuSequence()
    {
        Debug.Log("Menu");
        pc.isRunning = false;
        //Time.timeScale = 0f;
        GameManager.instance.backgroundPanel.SetActive(true);
    }

    private void LoadDefaultScene()
    {
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void LoadNextScene()
    {
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void PlayButtonAction()
    {
        state = State.Running;
        myEvent.Invoke();
    }
}
