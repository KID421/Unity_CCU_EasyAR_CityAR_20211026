using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class MainController : MonoBehaviour {

    public GameObject showPic;
    DirectoryInfo dir;
    string pcPath;
    public GameObject Flash;
    public GameObject UIs;
    public GameObject arCam;

    public GameObject[] Building;
    [HideInInspector]
    public GameObject InteractiveObj;
    public Vector2 FOVRange;
    public float ZoomSpeed = 0.2f;

    private void Start() {
        Permission.RequestUserPermission(Permission.ExternalStorageRead);
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        AndroidRuntimePermissions.Permission result = AndroidRuntimePermissions.RequestPermission("android.permission.WRITE_EXTERNAL_STORAGE");
        AndroidRuntimePermissions.Permission result2 = AndroidRuntimePermissions.RequestPermission("android.permission.READ_EXTERNAL_STORAGE");
    }

    private void Update() {
        if (Input.GetKey(KeyCode.W)) {
            if (Camera.main.fieldOfView > FOVRange.x)
                Camera.main.fieldOfView -= ZoomSpeed;
        }
        if (Input.GetKey(KeyCode.S)) {
            if (Camera.main.fieldOfView < FOVRange.y)
                Camera.main.fieldOfView += ZoomSpeed;
        }
    }

    public void ScreenShot() {
        Permission.RequestUserPermission(Permission.ExternalStorageRead);
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        AndroidRuntimePermissions.Permission result = AndroidRuntimePermissions.RequestPermission("android.permission.WRITE_EXTERNAL_STORAGE");
        AndroidRuntimePermissions.Permission result2 = AndroidRuntimePermissions.RequestPermission("android.permission.READ_EXTERNAL_STORAGE");

        Flash.SetActive(true);
        UIs.SetActive(false);
        arCam.SetActive(false);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        string folderPath;
        folderPath = Directory.GetCurrentDirectory() + @"/ARBuildScreenShots";

        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
        var screenshotName = "ARScreenShots_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";
        pcPath = System.IO.Path.Combine(folderPath, screenshotName);
        ScreenCapture.CaptureScreenshot(pcPath,2);
        ShowPic();
#endif

#if UNITY_ANDROID
        StartCoroutine("Co_ScreenShot");
 
#endif
      
    }

    IEnumerator Co_ScreenShot() {
        string timeStamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        yield return new WaitForEndOfFrame();
        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string myPath;
        myPath = GetAndroidExternalStoragePath() + "/ARBuildScreenShots";
        if (!Directory.Exists(myPath)) {
            Directory.CreateDirectory(myPath);
        }
        var screenshotName = "ARScreenShots_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";
        string filePath = Path.Combine(myPath, screenshotName);
        if (Directory.Exists(myPath)) {
            File.WriteAllBytes(filePath, ss.EncodeToPNG());
        }
        Destroy(ss);
        yield return new WaitUntil(() => File.Exists(filePath));
        string[] paths = new string[1];
        paths[0] = filePath;
        ScanFile(paths);
        ShowPic();
        yield return null;
    }


    private string GetAndroidExternalStoragePath() {
        if (Application.platform != RuntimePlatform.Android) {
            return Application.persistentDataPath;
        }

        var jc = new AndroidJavaClass("android.os.Environment");
        var path = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory",
            jc.GetStatic<string>("DIRECTORY_DCIM")).Call<string>("getAbsolutePath");
        return path;
    }

    void ScanFile(string[] path) {
        using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");
            using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", playerActivity, null)) {
                Conn.CallStatic("scanFile", playerActivity, path, null, null);
            }
        }
    }

    public void ShowPic() {
        StartCoroutine("DelayShowPic");

    }

    IEnumerator DelayShowPic() {
        yield return new WaitForSeconds(0.3f);
#if UNITY_EDITOR || UNITY_STANDALONE
        string folderPath = Directory.GetCurrentDirectory() + "/ARBuildScreenShots";
        if (Directory.Exists(folderPath)) {
            dir = new DirectoryInfo(folderPath);
            StartCoroutine("GetT");
        }
#endif
#if UNITY_ANDROID
        string myPath = GetAndroidExternalStoragePath() + "/ARBuildScreenShots";
        if (Directory.Exists(myPath)) {
            dir = new DirectoryInfo(myPath);
            StartCoroutine("GetT");
        }
#endif
        showPic.SetActive(true);
        UIs.SetActive(true);
        arCam.SetActive(true); 
        yield return null;
    }


    IEnumerator GetT() {
        List<FileInfo> info = dir.GetFiles("*.*").ToList();
        info.OrderByDescending(x => x);
        int i = info.Count - 1;
        if (i<=0) {
            i = 0;
        }
        string filePath = info[info.Count - 1].FullName;
        byte[] fileData;
        fileData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(1024, 1024);
        tex.LoadImage(fileData);
        Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        showPic.GetComponent<Image>().sprite = s;
        yield return null;
    }



    public void DiscoverMarker(int n) {
        InteractiveObj.SetActive(true);
        Building[n - 1].SetActive(true);
    }

    public void MissMarker(int n) {
        InteractiveObj.SetActive(false);
        Building[n - 1].SetActive(false);
    }

}//End
