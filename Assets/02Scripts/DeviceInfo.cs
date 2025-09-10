using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class DeviceInfo
{
    public string friendlyName;             // [Required] Name that will be shown in UI.
    public int version;                     // [Required] Device definition file version. Currently version is 1.
    public ScreenData[] Screens;            // [Required] Screen related data. Must Contain at least one screen. Support for multiple screens is not yet implemented.
    public SystemInfoData SystemInfo;       // [Required] Values returned by UnityEngine.SystemInfo. Contains a single required field: operatingSystem.
}

[Serializable]
public class ScreenPresentation
{
    public string overlayPath;            // [Optional] Relative path from *.device.json file to an image that will be used as device overlay.
    public Vector4 borderSize;            // [Optional] Pixel distance from overlay image border to where the screen begins.
}

[Serializable]
public class ScreenData
{
    public int width;                                // [Required] Value returned by UnityEngine.Screen.width in portrait orientation.
    public int height;                               // [Required] Value returned by UnityEngine.Screen.height in portrait orientation.
    public int navigationBarHeight;                  // [Optional] Pixel height of the on-screen Android navigation bar, which appears on some devices in non-fullscreen mode.
    public float dpi;                                // [Required] Value returned by UnityEngine.Screen.dpi.
    public OrientationData[] orientations;           // [Optional] Defines which orientations are supported on the device. If this field is missing, all orientations will be supported.
    public ScreenPresentation presentation;          // [Optional] Data for drawing an overlay with device borders, notches and other irregularities baked in.
}

[Serializable]
public class OrientationData
{
    public ScreenOrientation orientation;            // [Required in OrientationData] Supported orientation
    public Rect safeArea;                            // [Optional] Value returned by UnityEngine.Screen.safeArea in full resolution. Is this field is missing, assuming that entire screen is safe.
    public Rect[] cutouts;                           // [Optional] Value returned by UnityEngine.Screen.cutouts in full resolution.
}

[Serializable]
public class SystemInfoData                                  // Fields map to UnityEngine.SystemInfo. All fields are optional except operatingSystem.
{
    public string deviceModel;
    public DeviceType deviceType;
    public string operatingSystem;                           // [Required] Must contain either Android or iOS (case-insensitive) somewhere in the string.
    public OperatingSystemFamily operatingSystemFamily;
    public int processorCount;
    public int processorFrequency;
    public string processorType;
    public bool supportsAccelerometer;
    public bool supportsAudio;
    public bool supportsGyroscope;
    public bool supportsLocationService;
    public bool supportsVibration;
    public int systemMemorySize;
    public string unsupportedIdentifier;
    public GraphicsSystemInfoData[] graphicsDependentData;   // [Optional] Defines which graphics APIs are supported on the device.
}

[Serializable]
public class GraphicsSystemInfoData                          // Fields map to UnityEngine.SystemInfo.
{
    public GraphicsDeviceType graphicsDeviceType;            // [Required in GraphicsSystemInfoData] Supported graphics API.
    public int graphicsMemorySize;
    public string graphicsDeviceName;
    public string graphicsDeviceVendor;
    public int graphicsDeviceID;
    public int graphicsDeviceVendorID;
    public bool graphicsUVStartsAtTop;
    public string graphicsDeviceVersion;
    public int graphicsShaderLevel;
    public bool graphicsMultiThreaded;
    public RenderingThreadingMode renderingThreadingMode;
    public bool hasHiddenSurfaceRemovalOnGPU;
    public bool hasDynamicUniformArrayIndexingInFragmentShaders;
    public bool supportsShadows;
    public bool supportsRawShadowDepthSampling;
    public bool supportsMotionVectors;
    public bool supports3DTextures;
    public bool supports2DArrayTextures;
    public bool supports3DRenderTextures;
    public bool supportsCubemapArrayTextures;
    public CopyTextureSupport copyTextureSupport;
    public bool supportsComputeShaders;
    public bool supportsGeometryShaders;
    public bool supportsTessellationShaders;
    public bool supportsInstancing;
    public bool supportsHardwareQuadTopology;
    public bool supports32bitsIndexBuffer;
    public bool supportsSparseTextures;
    public int supportedRenderTargetCount;
    public bool supportsSeparatedRenderTargetsBlend;
    public int supportedRandomWriteTargetCount;
    public int supportsMultisampledTextures;
    public bool supportsMultisampleAutoResolve;
    public int supportsTextureWrapMirrorOnce;
    public bool usesReversedZBuffer;
    public NPOTSupport npotSupport;
    public int maxTextureSize;
    public int maxCubemapSize;
    public int maxComputeBufferInputsVertex;
    public int maxComputeBufferInputsFragment;
    public int maxComputeBufferInputsGeometry;
    public int maxComputeBufferInputsDomain;
    public int maxComputeBufferInputsHull;
    public int maxComputeBufferInputsCompute;
    public int maxComputeWorkGroupSize;
    public int maxComputeWorkGroupSizeX;
    public int maxComputeWorkGroupSizeY;
    public int maxComputeWorkGroupSizeZ;
    public bool supportsAsyncCompute;
    public bool supportsGraphicsFence;
    public bool supportsAsyncGPUReadback;
    public bool supportsRayTracing;
    public bool supportsSetConstantBuffer;
    public bool minConstantBufferOffsetAlignment;
    public bool hasMipMaxLevel;
    public bool supportsMipStreaming;
    public bool usesLoadStoreActions;
}
