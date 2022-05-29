namespace AutoVDesktop
{
    internal class Win32
    {
        // 引入控制台
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool AllocConsole();
    }
}
