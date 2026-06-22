//using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;

//public class HideHandOnGrab : MonoBehaviour
//{
//    public GameObject rightHand;
//    public GameObject leftHand;

//    private XRGrabInteractable grabInteractable;

//    void Start()
//    {
//        grabInteractable = GetComponent<XRGrabInteractable>();

//        grabInteractable.selectEntered.AddListener(HideHand);
//        grabInteractable.selectExited.AddListener(ShowHand);
//    }

//    void HideHand(SelectEnterEventArgs args)
//    {
//        string handName = args.interactorObject.transform.name;

//        if (handName.Contains("Right"))
//        {
//            rightHand.SetActive(false);
//        }

//        if (handName.Contains("Left"))
//        {
//            leftHand.SetActive(false);
//        }
//    }

//    void ShowHand(SelectExitEventArgs args)
//    {
//        rightHand.SetActive(true);
//        leftHand.SetActive(true);
//    }
//}