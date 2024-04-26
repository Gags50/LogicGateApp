using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    GameObject canvas;
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
            GameManager.instance.spawnedPuzzle = Instantiate(puzzle, canvas.transform);
        }
        else
        {
            Destroy(GameManager.instance.spawnedPuzzle);
        }
    }

    public void tipToggle(GameObject tip)
    {
        if (activeTip == false)
        {
            GameManager.instance.spawnedTip = Instantiate(tip, Vector3.zero, Quaternion.identity, canvas.transform);
        }

        else
        {
            Destroy(GameManager.instance.spawnedTip);
        }
    }
}
