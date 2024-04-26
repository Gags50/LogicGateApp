using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    GameObject canvas;
    GameObject spawnedPuzzle;
    GameObject spawnedTip;
    bool activePuzzle = false;
    bool activeTip = false;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas");
    }
    public void puzzleToggle(GameObject puzzle)
    {
        if (activePuzzle == false)
        {
            spawnedPuzzle = Instantiate(puzzle, Vector3.zero, Quaternion.identity, canvas.transform);
        }

        else
        {
            Destroy(spawnedPuzzle);
        }
    }

    public void tipToggle(GameObject tip)
    {
        if (activeTip == false)
        {
            spawnedTip = Instantiate(tip, Vector3.zero, Quaternion.identity, canvas.transform);
        }

        else
        {
            Destroy(spawnedTip);
        }
    }
}
