using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class DisableBitcodePreBuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        // Controlla se la piattaforma di build è iOS
        if (report.summary.platform == BuildTarget.iOS)
        {
            // Disabilita Bitcode
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK; // Assicurati di usare la SDK del dispositivo
            PlayerSettings.SetPropertyBool("EnableBitcode", false, BuildTargetGroup.iOS);
            Debug.Log("Bitcode disabled for iOS build.");
        }
    }
}
