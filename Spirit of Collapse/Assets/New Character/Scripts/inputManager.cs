using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class inputManager : MonoBehaviour
{
    private PlayerInput input;
    private InputAction direction;
    private GameObject changeObject;
    private GameObject saveButton;
    public GameObject inputUI;
    public GameObject keybrd;
    public GameObject Cont;
    private Dictionary<string, int> keyNameToNum = new Dictionary<string, int>();

    private void defineDictionary()
    {
        keyNameToNum.Add("UpKey", 1);
        keyNameToNum.Add("DownKey", 2);
        keyNameToNum.Add("LeftKey", 3);
        keyNameToNum.Add("RightKey", 4);
        keyNameToNum.Add("UpCon", 6);
        keyNameToNum.Add("DownCon", 7);
        keyNameToNum.Add("LeftCon", 8);
        keyNameToNum.Add("RightCon", 9);
        keyNameToNum.Add("JumpKey", 1);
        keyNameToNum.Add("JumpCon", 0);
        keyNameToNum.Add("AttackKey", 1);
        keyNameToNum.Add("AttackCon", 0);
    }

    private void Start()
    {
        defineDictionary();
    }

    public void setInputs()
    {
        changeObject = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        TextMeshProUGUI text = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if (changeObject != null)
        {
            switch (changeObject.name)
            {
                case "Default Screen":
                    SetDefault();
                    break;
                case "Up":
                    if (changeObject.transform.parent.name == "Content (Keyboard)")
                    {
                        rebind(direction, 1, true, text);
                    }
                    else
                    {
                        rebind(direction, 6, false, text);
                    }                    
                    break;
                case "Down":
                    if (changeObject.transform.parent.name == "Content (Keyboard)")
                    {
                        rebind(direction, 2, true, text);
                    }
                    else
                    {
                        rebind(direction, 7, false, text);
                    }
                    break;
                case "Left":
                    if (changeObject.transform.parent.name == "Content (Keyboard)")
                    {
                        rebind(direction, 3, true, text);
                    }
                    else
                    {
                        rebind(direction, 7, false, text);
                    }
                    break;
                case "Right":
                    if (changeObject.transform.parent.name == "Content (Keyboard)")
                    {
                        rebind(direction, 4, true, text);
                    }
                    else
                    {
                        rebind(direction, 8, false, text);
                    }
                    break;
                case "Jump":
                    if (changeObject.transform.parent.name == "Content (Keyboard)")
                    {
                        rebind(input.actions["Jump"], 1, true, text);
                    }
                    else
                    {
                        rebind(input.actions["Jump"], 0, false, text);
                    }
                    break;
                case "Attack":
                    if (changeObject.transform.parent.name == "Content (Keyboard)")
                    {
                        rebind(input.actions["Attack"], 0, true, text);
                    }
                    else
                    {
                        rebind(input.actions["Attack"], 1, false, text);
                    }
                    break;
                case "Input Setting Button":
                    input = GetComponent<PlayerInput>();
                    direction = input.actions["Direction"];
                    saveButton = GameObject.Find("Input UI").transform.Find("Input Save Button").gameObject;
                    break;
            }
        }
        else
        {
            loadInputs();
        }
    }

    private void rebind(InputAction action, int indexnum, bool keyboard, TextMeshProUGUI text)
    {
        action.Disable();
        GameObject waitScreen = inputUI.transform.Find("Screens").transform.Find("Wait For Input Screen").gameObject;
        waitScreen.SetActive(true);

        void cleanUp(InputActionRebindingExtensions.RebindingOperation operation)
        {
            changeObject = null;
            operation?.Dispose();
            waitScreen.SetActive(false);
            action.Enable();
        }

        if (keyboard == true)
        {
            waitScreen.transform.Find("Waiting (Keyboard)").gameObject.SetActive(true);
            action.PerformInteractiveRebinding(indexnum).WithoutIgnoringNoisyControls().WithCancelingThrough("<Gamepad>/*").WithControlsHavingToMatchPath("Keyboard").WithControlsExcluding("<Keyboard>/escape").WithControlsExcluding("<Keyboard>/anyKey").WithControlsHavingToMatchPath("Mouse").Start()
            .OnComplete(operation =>
            {
                waitScreen.transform.Find("Waiting (Keyboard)").gameObject.SetActive(false);
                
                text.text = action.bindings[indexnum].ToDisplayString();
                saveButton.SetActive(true);
                cleanUp(operation);
            })
            .OnCancel(operation =>
            {
                waitScreen.transform.Find("Waiting (Keyboard)").gameObject.SetActive(false);
                cleanUp(operation);
            });
        }
        else
        {
            waitScreen.transform.Find("Waiting (Con)").gameObject.SetActive(true);
            action.PerformInteractiveRebinding(indexnum).WithoutIgnoringNoisyControls().WithCancelingThrough("<Keyboard>/*").WithControlsExcluding("Keyboard").WithControlsExcluding("Mouse").WithControlsExcluding("<Gamepad>/start").Start()
            .OnComplete(operation =>
            {
                waitScreen.transform.Find("Waiting (Con)").gameObject.SetActive(false);
                text.text = action.bindings[indexnum].ToDisplayString();
                saveButton.SetActive(true);
                cleanUp(operation);
            })
            .OnCancel(operation =>
            {
                waitScreen.transform.Find("Waiting (Con)").gameObject.SetActive(false);
                cleanUp(operation);
            });
        }
    }

    public void saveInputs()
    {
        PlayerPrefs.SetString("Input", input.actions.SaveBindingOverridesAsJson());
        loadInputs();
    }

    private void loadInputs()
    {
        input.actions.Disable();

        if (PlayerPrefs.GetString("Input") != "" && PlayerPrefs.GetString("Input") != "Default")
        {
            input.actions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("Input"));
        }
        else if(PlayerPrefs.GetString("Input") == "Default")
        {
            Cont.SetActive(true);
            keybrd.SetActive(true);
            PlayerPrefs.SetString("Input", "");
        }

        GameObject[] buttonText = GameObject.FindGameObjectsWithTag("Button Text");

        foreach (GameObject item in buttonText)
        {
            if (item.transform.IsChildOf(keybrd.transform))
            {
                string connectName = item.transform.parent.gameObject.transform.parent.name + "Key";

                if (connectName.Contains("Up") || connectName.Contains("Down") || connectName.Contains("Left") || connectName.Contains("Right"))
                {
                    item.GetComponent<TextMeshProUGUI>().text = direction.bindings[keyNameToNum[connectName]].ToDisplayString();
                }
                else
                {
                    item.GetComponent<TextMeshProUGUI>().text = input.actions[item.transform.parent.transform.parent.name].bindings[keyNameToNum[connectName]].ToDisplayString();
                }
            }
            else
            {
                string connectName = item.transform.parent.gameObject.transform.parent.name + "Con";

                if (connectName.Contains("Up") || connectName.Contains("Down") || connectName.Contains("Left") || connectName.Contains("Right"))
                {
                    item.GetComponent<TextMeshProUGUI>().text = direction.bindings[keyNameToNum[connectName]].ToDisplayString();
                }
                else
                {
                    item.GetComponent<TextMeshProUGUI>().text = input.actions[item.transform.parent.transform.parent.name].bindings[keyNameToNum[connectName]].ToDisplayString();
                }
            }
        }

        input.actions.Enable();        
    }

    private void SetDefault()
    {     
        PlayerPrefs.SetString("Input", "Default");
        input.actions.RemoveAllBindingOverrides();
        loadInputs();
    }
}