using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject joinScreenUI;
    public GameObject gameplayUI;
    public GameObject[] playerUI;
    public Animator[] playerTurnUI;
    public bool[] playerReady = new bool[4];
    public Animator title;
    public Animator buttonPrompts;

    public OtrioController otrio;

    private void Awake()
    {
        playerTurnUI[0].Play("Base Layer.BubbleInactive");
        playerTurnUI[1].Play("Base Layer.BubbleInactive");
        playerTurnUI[2].Play("Base Layer.BubbleInactive");
        playerTurnUI[3].Play("Base Layer.BubbleInactive");
    }

    private void Update()
    {
        for(int x = 0; x < 4; x++)
        {
            if(x < otrio.numPlayers)
            {
                playerUI[x].SetActive(true);
            }
            else
            {
                playerUI[x].SetActive(false);
            }
        }

        if (otrio.isPlay)
        {
            gameplayUI.SetActive(true);
        }
        else
        {
            gameplayUI.SetActive(false);
        }
    }
}
