using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace UnityChan.Editor {


public static class AutoPackageInstaller {


    [InitializeOnLoadMethod]
    static void AutoPackageInstaller_OnLoad() {
        EditorUtility.ClearProgressBar();
        m_packageListRequest = Client.List( /*offlineMode= */true, /*includeIndirectDependencies= */ true);

        EditorApplication.update += UpdateRequestJobs;

    }


//----------------------------------------------------------------------------------------------------------------------    
    static void UpdateRequestJobs() {

        if (StatusCode.Failure == m_packageListRequest.Status) {
            CancelAutoInstall();
            return;
        }

        if (StatusCode.InProgress == m_packageListRequest.Status) {
            return;
        }

        //Remove already installed packages
        Dictionary<string, string> remainingRequiredPackages = new Dictionary<string, string>(m_requiredPackages);
        PackageCollection results = m_packageListRequest.Result;
        foreach (UnityEditor.PackageManager.PackageInfo packageInfo in results) {
            remainingRequiredPackages.Remove(packageInfo.name);
        }

        //install and wait for recompile
        foreach (KeyValuePair<string, string> packageVersion in remainingRequiredPackages) {
            string packageId = $"{packageVersion.Key}@{packageVersion.Value}";
            EditorUtility.DisplayProgressBar(DIALOG_TITLE, "Installing " + packageVersion.Key, 0);
            Client.Add(packageId);
            EndAutoInstall();
            return;
        }

        EndAutoInstall();
        
    }

//----------------------------------------------------------------------------------------------------------------------    

    static void CancelAutoInstall() {
        EndAutoInstall();
        EditorUtility.DisplayDialog(DIALOG_TITLE, "Failed to install", "ok");
    }

    static void EndAutoInstall() {
        EditorApplication.update -= UpdateRequestJobs;
        EditorUtility.ClearProgressBar();     
    }


//----------------------------------------------------------------------------------------------------------------------

    private static Dictionary<string, string> m_requiredPackages = new Dictionary<string, string>() {
        { "com.unity.toonshader", "0.13.0-preview" },
    };

    static ListRequest m_packageListRequest = null;


    const string DIALOG_TITLE = "UnityChan";
}

} //namespace