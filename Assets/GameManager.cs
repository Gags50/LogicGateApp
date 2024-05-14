using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject spawnedPuzzle;
    public GameObject spawnedTip;
    public bool activePuzzle;

    void Awake()
    {
        instance = this;
        activePuzzle = false;
    }
}
