//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.DeviceInfo;

//public class GalaxyTabS7LandscapeExtension : IDeviceSimulatorExtension
//{
//    public string extensionName => "Galaxy Tab S7+ Landscape Auto-Rotate";

//    public void OnCreate(ISimulator simulator)
//    {
//        if (simulator.deviceInfo != null &&
//            simulator.deviceInfo.friendlyName == "Galaxy Tab S7+")
//        {
//            simulator.rotation = DeviceRotation.LandscapeLeft;
//        }
//    }

//    public void OnDestroy() { }
//}
//#endif