using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text coinsText;
    [SerializeField] Slider levelProgressBar;
    public GameObject backgroundPanel;

    public static GameManager instance;

    private State state;
    private int coins;
    private Transform playerTransform;
    private Transform finishTransform;
    private float originalDist;
    private Vector3 finishVector3;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;
        finishTransform = GameObject.FindGameObjectWithTag("Finish").transform;
        finishVector3 = finishTransform.GetComponent<Collider>().ClosestPoint(playerTransform.position);

        originalDist = Vector3.Distance(playerTransform.position, finishVector3);
    }

    // Update is called once per frame
    void Update()
    {
        IncrementLevelProgress();
    }

    public void IncrementCoinsScore()
    {
        coins++;
        coinsText.text = coins.ToString();
    }

    public void IncrementLevelProgress()
    {
        float dist = Vector3.Distance(playerTransform.position, finishVector3);
        levelProgressBar.value = 1 - (dist / originalDist);
    }

    
}
