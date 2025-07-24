using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] DialogueData[] dialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, 3, LayerMask.GetMask("Player"));

        if (!player) return;
        EventManager.InvokeOnDialogueStarted(dialogue);
    }
}
