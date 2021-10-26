using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
/// <summary>
/// 建立一個類別，用這個類別來存檔，將資料都包進去
/// </summary>

/// 
/// 
[System.Serializable]
public class ScreenSize {
    //public string PlayerName = "";
    public bool IsShowMouse = true;
    public int Width = 1920;
    public int Height = 1080;
    public bool isFullScreen = true;
    public int ScreenLayer = 0;
    public bool Popupwindow = true;
    public int PopupwindowPosX = 0;
    public int PopupwindowPosY = 0;
}
/// <summary>
/// Save manger.
/// </summary>
public class SaveManager : MonoBehaviour {
    /// <summary>
    /// Window attribute
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    const uint SWP_SHOWWINDOW = 0x0040;
    const int GWL_EXSTYLE = -20;
    const int GWL_STYLE = -16;
    const int WS_BORDER = 2;
    const int WS_EX_LAYERED = 0x80000;
    public const int LWA_ALPHA = 0x2;
    public const int LWA_COLORKEY = 0x1;


    /// <summary>
    /// Mouse
    public KeyCode ShowMouseKey = KeyCode.M;
    public bool DebugMode = false;
    public KeyCode DebugKey = KeyCode.D;
    /// </summary>//



    public ScreenSize ScreenController = new ScreenSize(); //建立存檔的類別
    string path;
    ScreenSize CurrentScreenSize = new ScreenSize();
    // Use this for initialization
    void Start() {
        path = Application.dataPath + "/../" + "Save.json";
        Load();
        SetWindow();
    }

    void SetWindow() {
        if (!Application.isEditor) {
            if (CurrentScreenSize.IsShowMouse) {
                Cursor.visible = true;
            }
            else {
                Cursor.visible = false;
            }

            if (CurrentScreenSize.isFullScreen) {
                Screen.SetResolution(CurrentScreenSize.Width, CurrentScreenSize.Height, CurrentScreenSize.isFullScreen);
            }
            else {
                if (CurrentScreenSize.Popupwindow)
                    SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_BORDER);  //去邊框 

                Screen.SetResolution(CurrentScreenSize.Width, CurrentScreenSize.Height, CurrentScreenSize.isFullScreen);
                bool result = SetWindowPos(GetForegroundWindow(), CurrentScreenSize.ScreenLayer/* 視窗前後順序 -1(MostTop) to 2(MostNotTop) */, CurrentScreenSize.PopupwindowPosX /* 螢幕位置_x */, CurrentScreenSize.PopupwindowPosY /* 螢幕位置_y */, CurrentScreenSize.Width/* 解析度_x*/, CurrentScreenSize.Height /*  解析度_y */, SWP_SHOWWINDOW);
            }
        }
    }
    void Load() {
        //if (PlayerPrefs.HasKey("DATA"))
        //{
        //	string s = PlayerPrefs.GetString("DATA");
        if (File.Exists(path)) {
            UnityEngine.Debug.Log("load data");
            string s = File.ReadAllText(path);
            //已經存檔過的把資料讀出來，再用Json將字串轉回類別
            ScreenController = (ScreenSize)JsonUtility.FromJson<ScreenSize>(s);
            CurrentScreenSize.IsShowMouse = ScreenController.IsShowMouse;
            CurrentScreenSize.Width = ScreenController.Width;
            CurrentScreenSize.Height = ScreenController.Height;
            CurrentScreenSize.isFullScreen = ScreenController.isFullScreen;
            CurrentScreenSize.ScreenLayer = ScreenController.ScreenLayer;
            CurrentScreenSize.Popupwindow = ScreenController.Popupwindow;
            CurrentScreenSize.PopupwindowPosX = ScreenController.PopupwindowPosX;
            CurrentScreenSize.PopupwindowPosY = ScreenController.PopupwindowPosY;
        }
        else {
            Save();
        }
        //}
    }

    void Save() {
        using (StreamWriter wr = new StreamWriter(path)) {
            ScreenController.IsShowMouse = CurrentScreenSize.IsShowMouse;
            ScreenController.Width = CurrentScreenSize.Width;
            ScreenController.Height = CurrentScreenSize.Height;
            ScreenController.isFullScreen = CurrentScreenSize.isFullScreen;
            ScreenController.ScreenLayer = CurrentScreenSize.ScreenLayer;
            ScreenController.Popupwindow = CurrentScreenSize.Popupwindow;
            ScreenController.PopupwindowPosX = CurrentScreenSize.PopupwindowPosX;
            ScreenController.PopupwindowPosY = CurrentScreenSize.PopupwindowPosY;
            //將類別轉換為json格式的文字，再存檔
            string s = JsonUtility.ToJson(ScreenController);
            wr.Write(s);
            wr.Close();
        }
        //PlayerPrefs.SetString("DATA",s);
    }
    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(ShowMouseKey)) {
            if (CurrentScreenSize.IsShowMouse) {
                CurrentScreenSize.IsShowMouse = false;
            }
            else {
                CurrentScreenSize.IsShowMouse = true;
            }
            Cursor.visible = CurrentScreenSize.IsShowMouse;
        }
        if (Input.GetKeyDown(DebugKey) && Input.GetKey(KeyCode.LeftControl)) {
            if (DebugMode) {
                DebugMode = false;
                Cursor.visible = CurrentScreenSize.IsShowMouse;
            }
            else {
                DebugMode = true;
            }
        }
    }

    void OnGUI() {
        if (DebugMode) {
            GUI.Window(0, new Rect(0, 0, 500, 500), Window, "解析度");

            Cursor.visible = true;
        }
    }

    string sWidht, sHeight;
    void Window(int id) {
        GUILayout.Label("螢幕寬度");
        sWidht = CurrentScreenSize.Width.ToString();
        sWidht = GUILayout.TextField(sWidht);
        int.TryParse(sWidht, out CurrentScreenSize.Width);

        GUILayout.Label("螢幕高度");
        sHeight = CurrentScreenSize.Height.ToString();
        sHeight = GUILayout.TextField(sHeight);
        int.TryParse(sHeight, out CurrentScreenSize.Height);

        GUILayout.Label("螢幕位置X");
        sHeight = CurrentScreenSize.PopupwindowPosX.ToString();
        sHeight = GUILayout.TextField(sHeight);
        int.TryParse(sHeight, out CurrentScreenSize.PopupwindowPosX);

        GUILayout.Label("螢幕位置Y");
        sHeight = CurrentScreenSize.PopupwindowPosY.ToString();
        sHeight = GUILayout.TextField(sHeight);
        int.TryParse(sHeight, out CurrentScreenSize.PopupwindowPosY);

        GUILayout.Label("視窗Layer(-1 ~ 2 , 預設0)");
        sHeight = CurrentScreenSize.ScreenLayer.ToString();
        sHeight = GUILayout.TextField(sHeight);
        int.TryParse(sHeight, out CurrentScreenSize.ScreenLayer);

        CurrentScreenSize.IsShowMouse = GUILayout.Toggle(CurrentScreenSize.IsShowMouse, "顯示滑鼠");
        CurrentScreenSize.isFullScreen = GUILayout.Toggle(CurrentScreenSize.isFullScreen, "全螢幕");
        CurrentScreenSize.Popupwindow = GUILayout.Toggle(CurrentScreenSize.Popupwindow, "去邊框");

        if (GUILayout.Button("存檔"))
            Save();

    }
}
