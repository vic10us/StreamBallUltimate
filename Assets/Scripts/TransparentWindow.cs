//Note: Inspired by and uses some code found here: http://forum.unity3d.com/threads/windows-api-calls.127719/

using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices; // Pro and Free!!!

//WARNING!! Running this code inside Unity when not in a build will make the entire development environment transparent
//Restarting Unity would however resolve

public class TransparentWindow : MonoBehaviour
{
    [SerializeField] private Material m_Material;

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }
    
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")]
    static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
    static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);
    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    const int GWL_STYLE = -16;
    const uint WS_POPUP = 0x80000000;
    const uint WS_VISIBLE = 0x10000000;
    const int HWND_TOPMOST = -1;

//     void AltStart()
//     {
// #if !UNITY_EDITOR   // You really don't want to enable this in the editor..

//         IntPtr handle = GetActiveWindow();
//         int fWidth = Screen.width;
//         int fHeight = Screen.height;

//         //Remove title bar
//         uint lCurStyle = GetWindowLong(handle, -16);     // GWL_STYLE=-16
//         int a = 12582912;   //WS_CAPTION = 0x00C00000L
//         int b = 1048576;    //WS_HSCROLL = 0x00100000L
//         int c = 2097152;    //WS_VSCROLL = 0x00200000L
//         int d = 524288;     //WS_SYSMENU = 0x00080000L
//         int e = 16777216;   //WS_MAXIMIZE = 0x01000000L

//         lCurStyle &= ~(a | b | c | d);
//         lCurStyle &= e;
//         SetWindowLong(handle, -16, lCurStyle);// GWL_STYLE=-16

//         // Transparent windows with click through
//         SetWindowLong(handle, -20, 524288 | 32);//GWL_EXSTYLE=-20; WS_EX_LAYERED=524288=&h80000, WS_EX_TRANSPARENT=32=0x00000020L
//         //SetLayeredWindowAttributes(handle, 0xFF000000, 0, 1); // handle = current window, 0x0000FF00 = full Green (RGB 0, 255, 0), Opacity = 0 (transparent - used with LWA_ALPHA), LWA_COLORKEY = 1
//         SetLayeredWindowAttributes(handle, 0, 255, 2);// Transparency=51=20%, LWA_ALPHA=2

//         SetWindowPos(handle, 0, 0, 0, fWidth, fHeight, 32 | 64); //SWP_FRAMECHANGED = 0x0020 (32); //SWP_SHOWWINDOW = 0x0040 (64)
//         ShowWindowAsync(handle, 3); //Forces window to show in case of unresponsive app    // SW_SHOWMAXIMIZED(3)

// #endif
//     }

    void Start()
    {
        int fWidth = Screen.width;
        Debug.LogWarning($"Width: {fWidth}");
        int fHeight = Screen.height;
        Debug.LogWarning($"Width: {fHeight}");

#if !UNITY_EDITOR   // You really don't want to enable this in the editor..
        
        var margins = new MARGINS() { cxLeftWidth = -1 };
        var hwnd = GetActiveWindow();
        SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
        // Transparent windows with click through
        SetWindowLong(hwnd, -20, 524288 | 32);//GWL_EXSTYLE=-20; WS_EX_LAYERED=524288=&h80000, WS_EX_TRANSPARENT=32=0x00000020L
        SetLayeredWindowAttributes(hwnd, 0, 255, 2);// Transparency=51=20%, LWA_ALPHA=2
        SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, fWidth, fHeight, 32 | 64); //SWP_FRAMECHANGED = 0x0020 (32); //SWP_SHOWWINDOW = 0x0040 (64)
        DwmExtendFrameIntoClientArea(hwnd, ref margins);

#endif
 }

    void OnRenderImage(RenderTexture from, RenderTexture to)
    {
        Graphics.Blit(from, to, m_Material);
    }

}