using System;
using Code.Managers;
using UnityEngine;
using UnityEngine.UI;

public class TestBattle : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private Button thisBtn;
    private bool isPlaying = false;


    private void Awake()
    {
        thisBtn.onClick.AddListener(PlayGame);
    }

    public void PlayGame()
    {
        if (!isPlaying)
        {
            turnManager.StartBattle();
            isPlaying = true;
            thisBtn.gameObject.SetActive(false);
        }
    }
}
