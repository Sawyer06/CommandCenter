using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpenNote : MonoBehaviour
{
    [SerializeField] private GameObject promptText;
    [SerializeField] private GameObject interactable;
    [SerializeField] private TextMeshProUGUI noteText;

    [TextAreaAttribute]
    public string myMessage;

    public bool canOpenMenu;
    private bool menuState;

    void OnTriggerStay(Collider other)
    {
        if(other.transform.tag.Equals("Player"))
        {
            canOpenMenu = true;
        }
        promptText.SetActive(canOpenMenu);
        interactable.SetActive(menuState);
    }

    void OnTriggerExit(Collider other)
    {
        if(other.transform.tag.Equals("Player"))
        {
            canOpenMenu = false;
            menuState = false;
        }
        promptText.SetActive(canOpenMenu);
        interactable.SetActive(menuState);
    }

    void Update()
    {
        if(canOpenMenu){
            if(Input.GetKeyDown(KeyCode.E)){
                menuState = !menuState;
            }
            noteText.text = myMessage;
        }
    }
}
