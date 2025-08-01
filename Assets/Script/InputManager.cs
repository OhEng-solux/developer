using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private bool inputLocked = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public bool IsInputLocked()
    {
        return inputLocked;
    }

    public void LockInput(float lockDuration = 0.3f)
    {
        if (!inputLocked)
            StartCoroutine(InputLockCoroutine(lockDuration));
    }

    private IEnumerator InputLockCoroutine(float duration)
    {
        inputLocked = true;
        yield return new WaitForSeconds(duration);
        inputLocked = false;
    }
}