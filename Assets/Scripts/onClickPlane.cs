using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class onClickPlane : MonoBehaviour
{

    Color m_MouseOverColor = Color.white;


    Color m_OriginalColor;

    MeshRenderer m_Renderer;
    GameObject GameManager;

    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_OriginalColor = m_Renderer.material.color;
        m_Renderer.material.color = m_OriginalColor;
        GameManager = GameObject.Find("GameManager");
    }

    void OnMouseOver()
    {
        m_MouseOverColor.a = 0.6f;
        m_Renderer.material.color = m_MouseOverColor;
    }

    void OnMouseUp()
    {
        switch(m_Renderer.name){
            case "button0":
                    GameManager.GetComponent<Manager>().right_up();
                    break;
            case "button1":
                    GameManager.GetComponent<Manager>().middle();
                    break;
            case "button2":
                    GameManager.GetComponent<Manager>().left_up();
                    break;
            case "button3":
                    GameManager.GetComponent<Manager>().left_down();
                    break;
            case "button4":
                    GameManager.GetComponent<Manager>().right_down();
                    break;        
        }
    }

    void OnMouseExit()
    {
        m_Renderer.material.color = m_OriginalColor;
    }

}
