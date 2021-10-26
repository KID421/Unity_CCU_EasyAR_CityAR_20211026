using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class ScreenShot : MonoBehaviour
{
    public Texture2D PrintTexture;
    public RenderTexture rt;
    public string SavePath;
    public bool IsPicDone;
    public float CountTime=3;
    public float CountTimeNow=3;
    public GameObject Flash;
    public bool IsRunCount = true;
    // Start is called before the first frame update
    void Start()
    {
    }
    void OnEnable()
    {
        CountTimeNow = CountTime;
        IsPicDone = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (IsRunCount)
        {
            if (!IsPicDone)
            {
                if (CountTimeNow > 0)
                {
                    CountTimeNow -= Time.deltaTime;
                }
                else
                {
                    CapPic();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CapPic();
            }
        }

    }

    void CapPic()
    {
        IsPicDone = true;
        Flash.SetActive(false);
        Flash.SetActive(true);
        PrintTexture = getTexture2DFromRenderTexture(rt);
        byte[] allbytes = PrintTexture.EncodeToJPG();
        //Save Texutre
        //SavePath = Application.dataPath + "/../photo/" + DateTime.Now.Year + DateTime.Now.Month.ToString("f2") + DateTime.Now.Day.ToString("f2") + DateTime.Now.Hour.ToString("f2") + DateTime.Now.Minute.ToString("f2") + DateTime.Now.Second.ToString("f2") + ".jpg";
        SavePath = Application.dataPath + "/../photo/" + "AR_"+DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
        try
        {
            File.WriteAllBytes(SavePath, allbytes);
        }
        catch (IOException e)
        {
            Debug.Log(e.Message);
        }
    }

    public Texture2D getTexture2DFromRenderTexture(RenderTexture rTex)
    {
        Texture2D texture2D = new Texture2D(rTex.width, rTex.height);
        RenderTexture.active = rTex;
        texture2D.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        texture2D.Apply();
        return texture2D;
    }
}
