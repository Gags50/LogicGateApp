using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    GameObject canvas;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas");
    }
    public void puzzleToggle(GameObject puzzle)
    {
        if (GameManager.instance.activePuzzle == false)
        {
            GameManager.instance.spawnedPuzzle = Instantiate(puzzle, canvas.transform);
            GameManager.instance.activePuzzle = true;
        }
        else
        {
            Destroy(GameManager.instance.spawnedPuzzle);
            GameManager.instance.activePuzzle = false;
        }
    }

    public void tipToggle(GameObject tip)
    {
        if (GameManager.instance.activeTip == false)
        {
            GameManager.instance.spawnedTip = Instantiate(tip, canvas.transform);
            GameManager.instance.activeTip = true;
        }
        else
        {
            Destroy(GameManager.instance.spawnedTip);
            GameManager.instance.activeTip = false;
        }
    }
}
