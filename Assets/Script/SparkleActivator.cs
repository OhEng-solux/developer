using UnityEngine;

public class SparkleActivator : MonoBehaviour
{
    [System.Serializable]
    public class SparkleData
    {
        public GameObject sparkleObject;
        public int appearAt;   // 등장 시점
        public int disappearAt; // 사라질 시점 (0이면 무제한)
        [HideInInspector] public bool hasAppeared = false;
        [HideInInspector] public bool hasDisappeared = false;
    }

    public SparkleData[] sparkleList;

    void Update()
    {
        foreach (var sparkle in sparkleList)
        {
            int count = DialogueProgressManager.instance.dialogueCount;

            // 등장
            if (!sparkle.hasAppeared && count >= sparkle.appearAt)
            {
                sparkle.sparkleObject.SetActive(true);
                sparkle.hasAppeared = true;
            }

            // 사라짐
            if (sparkle.disappearAt > 0 && !sparkle.hasDisappeared && count >= sparkle.disappearAt)
            {
                sparkle.sparkleObject.SetActive(false);
                sparkle.hasDisappeared = true;
            }
        }
    }
}
