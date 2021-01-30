#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

//Note: Inspired by and uses some code found here: http://forum.unity3d.com/threads/windows-api-calls.127719/

using UnityEngine;
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
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
    private static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);
    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    private const int GWL_STYLE = -16;
    private const uint WS_POPUP = 0x80000000;
    private const uint WS_VISIBLE = 0x10000000;
    private const int HWND_TOPMOST = -1;
    
    void Start()
    {
#if !UNITY_EDITOR   // You really don't want to enable this in the editor..
        var fWidth = Screen.width;
        var fHeight = Screen.height;
        
        var margins = new MARGINS() { cxLeftWidth = -1 };
        var hwnd = GetActiveWindow();
        SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
        // Transparent windows with click through
        SetWindowLong(hwnd, -20, 524288 | 32);
        SetLayeredWindowAttributes(hwnd, 0, 255, 2);
        SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, fWidth, fHeight, 32 | 64);
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
#endif
    }

    void OnRenderImage(Texture from, RenderTexture to)
    {
        Graphics.Blit(from, to, m_Material);
    }

}