using UnityEngine;

public class ForeviewBlockChanger : MonoBehaviour
{
    [SerializeField]
    private GameObject I, O, Z, S, J, L, T;

    public void ChangeBlock(string name)
    {
        I.SetActive(false); O.SetActive(false); Z.SetActive(false); S.SetActive(false); J.SetActive(false); L.SetActive(false); T.SetActive(false);
        switch(name)
        {
            case "I":
                I.SetActive(true);
                break;
            case "O":
                O.SetActive(true);
                break;
            case "Z":
                Z.SetActive(true);
                break;
            case "S":
                S.SetActive(true);
                break;
            case "J":
                J.SetActive(true);
                break;
            case "L":
                L.SetActive(true);
                break;
            case "T":
                T.SetActive(true);
                break;
            case "N":
                break;
            default:
                Debug.Log("에바" + name);
                return;
        }
    }
}
