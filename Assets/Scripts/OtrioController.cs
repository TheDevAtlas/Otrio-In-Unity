using UnityEngine;
using UnityEngine.InputSystem;

public class OtrioController : MonoBehaviour
{
    // Otrio Board In Code //
    public GameObject[,,] otrioBoard = new GameObject[3, 3, 3];
    public int[,] usedPieces = new int[4, 3];
    public bool isPlay = false;

    [Header("Win Data")]
    public bool isWin = false;
    public GameObject[] winPieces = new GameObject[3];
    public float winHeight;

    // Player Data //
    [Header("Player Data")]
    public GameObject[] playerObjects = new GameObject[4];
    public int numPlayers = 0;
    public int playerActive;
    public int playerReady = 0;

    // Piece Data //
    [Header("Piece Data")]
    public Transform ghostTransform;
    public GameObject[] ghostPiece = new GameObject[3];
    public GameObject[] placedPiece = new GameObject[3];
    private Vector2 piecePosition = Vector2.zero;
    public float smooth = 0.125f;
    public float ghostHeight = 1.5f;

    [Header("Piece Settings")]
    public Vector3 scaleFactor;
    public int selectPiece = 0;

    [Header("Piece Mat Settings")]
    public Material[] playerMat = new Material[4];

    [Header("UI")]
    public UIController ui;

    private void Awake()
    {
        playerActive = 0;
    }

    private void FixedUpdate()
    {
        if (isWin && playerReady == numPlayers && numPlayers != 0)
        {
            isWin = false;
            isPlay = false;
            playerReady = 0;
            for(int x = 0; x < 3; x++)
            {
                for(int y = 0; y < 3; y++)
                {
                    for(int z = 0; z < 3; z++)
                    {
                        Destroy(otrioBoard[x, y, z]);
                        otrioBoard[x, y, z] = null;
                    }
                }
            }
            for(int x = 0; x < 4; x++){
                for(int y = 0; y < 3; y++)
                {
                    usedPieces[x, y] = 0;
                }
            }
            ui.title.Play("Base Layer.TitleActive");
            ui.playerTurnUI[0].Play("Base Layer.BubbleInactive");
            ui.playerTurnUI[1].Play("Base Layer.BubbleInactive");
            ui.playerTurnUI[2].Play("Base Layer.BubbleInactive");
            ui.playerTurnUI[3].Play("Base Layer.BubbleInactive");
            ui.playerReady[0] = false;
            ui.playerReady[1] = false;
            ui.playerReady[2] = false;
            ui.playerReady[3] = false;
        }
        if (!isPlay && playerReady == numPlayers && numPlayers != 0)
        {
            isPlay = true;
            ghostTransform.gameObject.SetActive(true);
            ui.playerTurnUI[0].Play("Base Layer.BubbleActive");
            ui.playerTurnUI[1].Play("Base Layer.BubbleInactive");
            ui.playerTurnUI[2].Play("Base Layer.BubbleInactive");
            ui.playerTurnUI[3].Play("Base Layer.BubbleInactive");
            ui.title.Play("Base Layer.TitleInactive");
            ui.buttonPrompts.Play("Base Layer.RulesActive");
        }

        ghostTransform.position = Vector3.Lerp(ghostTransform.position, new Vector3(piecePosition.x * scaleFactor.x, ghostHeight, piecePosition.y * scaleFactor.z), smooth);

        if (isPlay)
        {
            if(ghostTransform.gameObject.activeSelf == false)
            {
                ghostTransform.gameObject.SetActive(true);
            }
        }
        else
        {
            if (ghostTransform.gameObject.activeSelf == true)
            {
                ghostTransform.gameObject.SetActive(false);
            }
        }

        if (isWin)
        {
            ghostTransform.gameObject.SetActive(false);

            winPieces[0].transform.position = Vector3.Lerp(winPieces[0].transform.position, new Vector3(winPieces[0].transform.position.x, winHeight, winPieces[0].transform.position.z), smooth);
            winPieces[1].transform.position = Vector3.Lerp(winPieces[1].transform.position, new Vector3(winPieces[1].transform.position.x, winHeight, winPieces[1].transform.position.z), smooth);
            winPieces[2].transform.position = Vector3.Lerp(winPieces[2].transform.position, new Vector3(winPieces[2].transform.position.x, winHeight, winPieces[2].transform.position.z), smooth);
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        // Set Up Each OtrioPlayer Script On Join //
        playerObjects[numPlayers] = playerInput.gameObject;
        playerObjects[numPlayers].GetComponent<OtrioPlayer>().otrio = this;
        playerObjects[numPlayers].GetComponent<OtrioPlayer>().playerID = playerInput.playerIndex;
        numPlayers++;
    }

    public void IncrementVector(Vector2 amount, int ID)
    {
        if (ID != playerActive || !isPlay)
        {
            return;
        }
        // Increment the x and y values of the Vector2.
        piecePosition.x += (int)amount.x;
        piecePosition.y += (int)amount.y;

        // Clamp the x and y values to the range of -1 and 1.
        piecePosition.x = Mathf.Clamp(piecePosition.x, -1, 1);
        piecePosition.y = Mathf.Clamp(piecePosition.y, -1, 1);
    }

    public void IncrementSelection(float amount, int ID)
    {
        if (ID != playerActive || !isPlay)
        {
            return;
        }
        // Increment the value of the float.
        selectPiece += (int)amount;

        // Clamp the x and y values to the range of 0 and 2.
        if (selectPiece > 2)
        {
            selectPiece = 0;
        }

        // If the current value is less than the minimum value,
        // set it to the maximum value.
        if (selectPiece < 0)
        {
            selectPiece = 2;
        }

        if (selectPiece == 0)
        {
            ghostPiece[0].SetActive(true);
            ghostPiece[1].SetActive(false);
            ghostPiece[2].SetActive(false);
        }
        else if (selectPiece == 1)
        {
            ghostPiece[0].SetActive(false);
            ghostPiece[1].SetActive(true);
            ghostPiece[2].SetActive(false);
        }
        else if (selectPiece == 2)
        {
            ghostPiece[0].SetActive(false);
            ghostPiece[1].SetActive(false);
            ghostPiece[2].SetActive(true);
        }
    }

    public void PlaceSelection(int ID)
    {
        if (isWin)
        {
            ui.playerReady[ID] = true;
            ui.playerTurnUI[ID].Play("Base Layer.BubbleActive");
            playerReady++;
            return;
        }
        if (!isPlay && !ui.playerReady[ID])
        {
            ui.playerReady[ID] = true;
            ui.playerTurnUI[ID].Play("Base Layer.BubbleActive");
            playerReady++;
            return;
        }
        if (!isPlay) {
            return;
        }
        if (ID != playerActive || !isPlay)
        {
            return;
        }

        if (usedPieces[playerActive, selectPiece] >= 3)
        {
            return;
        }

        if (otrioBoard[(int)piecePosition.x + 1, (int)piecePosition.y + 1, (int)selectPiece] == null)
        {
            otrioBoard[(int)piecePosition.x + 1, (int)piecePosition.y + 1, (int)selectPiece] = Instantiate(placedPiece[selectPiece], new Vector3(piecePosition.x * scaleFactor.x, 0, piecePosition.y * scaleFactor.z), transform.rotation);
            otrioBoard[(int)piecePosition.x + 1, (int)piecePosition.y + 1, (int)selectPiece].GetComponent<Renderer>().material = playerMat[playerActive];

            if (ui.playerTurnUI[playerActive].gameObject.activeInHierarchy)
            {
                ui.playerTurnUI[playerActive].Play("Base Layer.BubbleInactive");
            }
            usedPieces[playerActive, selectPiece]++;

            // Check If Someone Won //
            if (CheckWin())
            {
                playerReady = 0;
                ui.playerTurnUI[0].Play("Base Layer.BubbleInactive");
                ui.playerTurnUI[1].Play("Base Layer.BubbleInactive");
                ui.playerTurnUI[2].Play("Base Layer.BubbleInactive");
                ui.playerTurnUI[3].Play("Base Layer.BubbleInactive");
                ui.buttonPrompts.Play("Base Layer.RulesInactive");
                ui.title.Play("Base Layer.TitleActive");
                return;
            }

            playerActive++;
            if (playerActive > numPlayers - 1)
            {
                playerActive = 0;
            }
            if (ui.playerTurnUI[playerActive].gameObject.activeInHierarchy)
            {
                ui.playerTurnUI[playerActive].Play("Base Layer.BubbleActive");
            }
        }
    }

    bool CheckWin()
    {
        // Tower Win Check //
        for(int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (otrioBoard[x,y,0] != null && otrioBoard[x,y,1] != null && otrioBoard[x,y,2] != null)
                {
                    Material one = otrioBoard[x, y, 0].GetComponent<MeshRenderer>().sharedMaterial;
                    Material two = otrioBoard[x, y, 1].GetComponent<MeshRenderer>().sharedMaterial;
                    Material thr = otrioBoard[x, y, 2].GetComponent<MeshRenderer>().sharedMaterial;

                    if (one == two && two == thr)
                    {
                        print("Win by tower");
                        isWin = true;
                        winPieces[0] = otrioBoard[x, y, 0];
                        winPieces[1] = otrioBoard[x, y, 1];
                        winPieces[2] = otrioBoard[x, y, 2];
                        return true;
                    }
                }
            }
        }

        // Row Win Check //
        for(int x = 0; x < 3; x++)
        {
            for(int z = 0; z < 3; z++)
            {
                if(otrioBoard[x, 0, z] != null && otrioBoard[x, 1, z] != null && otrioBoard[x, 2, z] != null)
                {
                    Material one = otrioBoard[x, 0, z].GetComponent<MeshRenderer>().sharedMaterial;
                    Material two = otrioBoard[x, 1, z].GetComponent<MeshRenderer>().sharedMaterial;
                    Material thr = otrioBoard[x, 2, z].GetComponent<MeshRenderer>().sharedMaterial;

                    if (one == two && two == thr)
                    {
                        print("Win by row");
                        isWin = true;
                        winPieces[0] = otrioBoard[x, 0, z];
                        winPieces[1] = otrioBoard[x, 1, z];
                        winPieces[2] = otrioBoard[x, 2, z];
                        return true;
                    }
                }
            }

            if (otrioBoard[x, 0, 0] != null && otrioBoard[x, 1, 1] != null && otrioBoard[x, 2, 2] != null)
            {
                Material one = otrioBoard[x, 0, 0].GetComponent<MeshRenderer>().sharedMaterial;
                Material two = otrioBoard[x, 1, 1].GetComponent<MeshRenderer>().sharedMaterial;
                Material thr = otrioBoard[x, 2, 2].GetComponent<MeshRenderer>().sharedMaterial;

                if (one == two && two == thr)
                {
                    print("Win by row");
                    isWin = true;
                    winPieces[0] = otrioBoard[x, 0, 0];
                    winPieces[1] = otrioBoard[x, 1, 1];
                    winPieces[2] = otrioBoard[x, 2, 2];
                    return true;
                }
            }

            if (otrioBoard[x, 0, 2] != null && otrioBoard[x, 1, 1] != null && otrioBoard[x, 2, 0] != null)
            {
                Material one = otrioBoard[x, 0, 2].GetComponent<MeshRenderer>().sharedMaterial;
                Material two = otrioBoard[x, 1, 1].GetComponent<MeshRenderer>().sharedMaterial;
                Material thr = otrioBoard[x, 2, 0].GetComponent<MeshRenderer>().sharedMaterial;

                if (one == two && two == thr)
                {
                    print("Win by row");
                    isWin = true;
                    winPieces[0] = otrioBoard[x, 0, 2];
                    winPieces[1] = otrioBoard[x, 1, 1];
                    winPieces[2] = otrioBoard[x, 2, 0];
                    return true;
                }
            }
        }

        // Col Win Check //
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                if (otrioBoard[0, y, z] != null && otrioBoard[1, y, z] != null && otrioBoard[2, y, z] != null)
                {
                    Material one = otrioBoard[0, y, z].GetComponent<MeshRenderer>().sharedMaterial;
                    Material two = otrioBoard[1, y, z].GetComponent<MeshRenderer>().sharedMaterial;
                    Material thr = otrioBoard[2, y, z].GetComponent<MeshRenderer>().sharedMaterial;

                    if (one == two && two == thr)
                    {
                        print("Win by col");
                        isWin = true;
                        winPieces[0] = otrioBoard[0, y, z];
                        winPieces[1] = otrioBoard[1, y, z];
                        winPieces[2] = otrioBoard[2, y, z];
                        return true;
                    }
                }
            }

            if (otrioBoard[0, y, 0] != null && otrioBoard[1, y, 1] != null && otrioBoard[2, y, 2] != null)
            {
                Material one = otrioBoard[0, y, 0].GetComponent<MeshRenderer>().sharedMaterial;
                Material two = otrioBoard[1, y, 1].GetComponent<MeshRenderer>().sharedMaterial;
                Material thr = otrioBoard[2, y, 2].GetComponent<MeshRenderer>().sharedMaterial;

                if (one == two && two == thr)
                {
                    print("Win by row");
                    isWin = true;
                    winPieces[0] = otrioBoard[0, y, 0];
                    winPieces[1] = otrioBoard[1, y, 1];
                    winPieces[2] = otrioBoard[2, y, 2];
                    return true;
                }
            }

            if (otrioBoard[0, y, 2] != null && otrioBoard[1, y, 1] != null && otrioBoard[2, y, 0] != null)
            {
                Material one = otrioBoard[0, y, 2].GetComponent<MeshRenderer>().sharedMaterial;
                Material two = otrioBoard[1, y, 1].GetComponent<MeshRenderer>().sharedMaterial;
                Material thr = otrioBoard[2, y, 0].GetComponent<MeshRenderer>().sharedMaterial;

                if (one == two && two == thr)
                {
                    print("Win by row");
                    isWin = true;
                    winPieces[0] = otrioBoard[0, y, 2];
                    winPieces[1] = otrioBoard[1, y, 1];
                    winPieces[2] = otrioBoard[2, y, 0];
                    return true;
                }
            }
        }

        // Diag Win Check //
        for(int z = 0; z < 3; z++)
        {
            // Diag one
            if (otrioBoard[0, 0, z] != null && otrioBoard[1, 1, z] != null && otrioBoard[2, 2, z] != null)
            {
                Material one = otrioBoard[0, 0, z].GetComponent<MeshRenderer>().sharedMaterial;
                Material two = otrioBoard[1, 1, z].GetComponent<MeshRenderer>().sharedMaterial;
                Material thr = otrioBoard[2, 2, z].GetComponent<MeshRenderer>().sharedMaterial;

                if (one == two && two == thr)
                {
                    print("Win by col");
                    isWin = true;
                    winPieces[0] = otrioBoard[0, 0, z];
                    winPieces[1] = otrioBoard[1, 1, z];
                    winPieces[2] = otrioBoard[2, 2, z];
                    return true;
                }
            }

            // Diag two
            if (otrioBoard[0, 2, z] != null && otrioBoard[1, 1, z] != null && otrioBoard[2, 0, z] != null)
            {
                Material one = otrioBoard[0, 2, z].GetComponent<MeshRenderer>().sharedMaterial;
                Material two = otrioBoard[1, 1, z].GetComponent<MeshRenderer>().sharedMaterial;
                Material thr = otrioBoard[2, 0, z].GetComponent<MeshRenderer>().sharedMaterial;

                if (one == two && two == thr)
                {
                    print("Win by col");
                    isWin = true;
                    winPieces[0] = otrioBoard[0, 2, z];
                    winPieces[1] = otrioBoard[1, 1, z];
                    winPieces[2] = otrioBoard[2, 0, z];
                    return true;
                }
            }
        }

        if (otrioBoard[0, 0, 0] != null && otrioBoard[1, 1, 1] != null && otrioBoard[2, 2, 2] != null)
        {
            Material one = otrioBoard[0, 0, 0].GetComponent<MeshRenderer>().sharedMaterial;
            Material two = otrioBoard[1, 1, 1].GetComponent<MeshRenderer>().sharedMaterial;
            Material thr = otrioBoard[2, 2, 2].GetComponent<MeshRenderer>().sharedMaterial;

            if (one == two && two == thr)
            {
                print("Win by col");
                isWin = true;
                winPieces[0] = otrioBoard[0, 0, 0];
                winPieces[1] = otrioBoard[1, 1, 1];
                winPieces[2] = otrioBoard[2, 2, 2];
                return true;
            }
        }

        // Diag two
        if (otrioBoard[0, 2, 2] != null && otrioBoard[1, 1, 1] != null && otrioBoard[2, 0, 0] != null)
        {
            Material one = otrioBoard[0, 2, 2].GetComponent<MeshRenderer>().sharedMaterial;
            Material two = otrioBoard[1, 1, 1].GetComponent<MeshRenderer>().sharedMaterial;
            Material thr = otrioBoard[2, 0, 0].GetComponent<MeshRenderer>().sharedMaterial;

            if (one == two && two == thr)
            {
                print("Win by col");
                isWin = true;
                winPieces[0] = otrioBoard[0, 2, 2];
                winPieces[1] = otrioBoard[1, 1, 1];
                winPieces[2] = otrioBoard[2, 0, 0];
                return true;
            }
        }

        if (otrioBoard[0, 0, 2] != null && otrioBoard[1, 1, 1] != null && otrioBoard[2, 2, 0] != null)
        {
            Material one = otrioBoard[0, 0, 2].GetComponent<MeshRenderer>().sharedMaterial;
            Material two = otrioBoard[1, 1, 1].GetComponent<MeshRenderer>().sharedMaterial;
            Material thr = otrioBoard[2, 2, 0].GetComponent<MeshRenderer>().sharedMaterial;

            if (one == two && two == thr)
            {
                print("Win by col");
                isWin = true;
                winPieces[0] = otrioBoard[0, 0, 2];
                winPieces[1] = otrioBoard[1, 1, 1];
                winPieces[2] = otrioBoard[2, 2, 0];
                return true;
            }
        }

        // Diag two
        if (otrioBoard[0, 2, 0] != null && otrioBoard[1, 1, 1] != null && otrioBoard[2, 0, 2] != null)
        {
            Material one = otrioBoard[0, 2, 0].GetComponent<MeshRenderer>().sharedMaterial;
            Material two = otrioBoard[1, 1, 1].GetComponent<MeshRenderer>().sharedMaterial;
            Material thr = otrioBoard[2, 0, 2].GetComponent<MeshRenderer>().sharedMaterial;

            if (one == two && two == thr)
            {
                print("Win by col");
                isWin = true;
                winPieces[0] = otrioBoard[0, 2, 0];
                winPieces[1] = otrioBoard[1, 1, 1];
                winPieces[2] = otrioBoard[2, 0, 2];
                return true;
            }
        }
        return false;
    }
}
