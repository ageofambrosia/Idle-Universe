using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseSideBarController : MonoBehaviour {

    private Animator sideBarAnim;

    void Start() {

        sideBarAnim = GetComponent<Animator>();

    }

    public void OpenCloseBar() {

        sideBarAnim.SetBool("Open", !sideBarAnim.GetBool("Open"));

    }
}
