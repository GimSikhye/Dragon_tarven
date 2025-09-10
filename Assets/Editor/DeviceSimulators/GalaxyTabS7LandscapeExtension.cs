//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEditor.DeviceSimulation;

//public class GalaxyTabS7LandscapeExtension : IDeviceSimulatorExtension
//{
//    public string extensionName => "Galaxy Tab S7+ Landscape Auto-Rotate";

//    public void OnCreate(IDeviceSimulator simulator)
//    {
//        if (simulator.deviceInfo != null &&
//            simulator.deviceInfo.friendlyName == "Galaxy Tab S7+")
//        {
//            simulator.rotation = ScreenOrientation.LandscapeLeft;
//        }
//    }

//    public void OnDestroy() { }
//}
//#endif