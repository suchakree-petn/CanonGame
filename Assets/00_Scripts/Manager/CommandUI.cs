using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour
{
    public bool ShowUIOnStart = true;
    [SerializeField]Image downImg;
    [SerializeField]Image leftImg;
    [SerializeField]Image upImg;
    [SerializeField]Image rightImg;

    bool startBlink = false;
    private void Start() {
        downImg.gameObject.SetActive(ShowUIOnStart);
        leftImg.gameObject.SetActive(ShowUIOnStart);
        upImg.gameObject.SetActive(ShowUIOnStart);
        rightImg.gameObject.SetActive(ShowUIOnStart);
        startBlink = ShowUIOnStart;
    }
    private void Update() {
        if(!ShowUIOnStart){
            SetStart();
        }
    }

    public void ChangeUIOnView(){
        if(startBlink){
            if(CameraManager.Instance.IsBirdEyeViewCamActive){
                leftImg.transform.rotation = Quaternion.Euler(0f, 0f,-90f);
                rightImg.transform.rotation = Quaternion.Euler(0f, 0f,-90f);
            }else{
                leftImg.transform.rotation = Quaternion.Euler(0f, 0f,0f);
                rightImg.transform.rotation = Quaternion.Euler(0f, 0f,0f);
            }
        }
        
    }
    void SetStart(){
        if(GameManager.Instance.IsPlayerTurn && !startBlink){
            startBlink = true;
            downImg.gameObject.SetActive(true);
            leftImg.gameObject.SetActive(true);
            upImg.gameObject.SetActive(true);
            rightImg.gameObject.SetActive(true);
        }
    }
}
