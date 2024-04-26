using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject spawnedPuzzle;
    public GameObject spawnedTip;

    void Awake()
    {
        instance = this;
    }
}
