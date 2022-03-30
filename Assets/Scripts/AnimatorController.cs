using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public static bool isBall0 = false;
    public static bool isBall1 = false;
    public static bool isBall2 = false;
    public static bool isBall3 = false;
    public static bool isBall4 = false;
    public static bool _isBall0 = false;
    public static bool _isBall1 = false;
    public static bool _isBall2 = false;
    public static bool _isBall3 = false;
    public static bool _isBall4 = false;
    public static bool iskeeper0 = false;
    public static bool iskeeper1 = false;
    public static bool iskeeper2 = false;
    public static bool iskeeper3 = false;
    public static bool iskeeper4 = false;
    Animator animators;
    // Start is called before the first frame update
    void Start()
    {
        animators = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animators.SetBool("isBall0", isBall0);
        animators.SetBool("isBall1", isBall1);
        animators.SetBool("isBall2", isBall2);
        animators.SetBool("isBall3", isBall3);
        animators.SetBool("isBall4", isBall4);
        animators.SetBool("_isBall0", _isBall0);
        animators.SetBool("_isBall1", _isBall1);
        animators.SetBool("_isBall2", _isBall2);
        animators.SetBool("_isBall3", _isBall3);
        animators.SetBool("_isBall4", _isBall4);
        animators.SetBool("iskeeper0", iskeeper0);
        animators.SetBool("iskeeper1", iskeeper1);
        animators.SetBool("iskeeper2", iskeeper2);
        animators.SetBool("iskeeper3", iskeeper3);
        animators.SetBool("iskeeper4", iskeeper4);
    }
}
