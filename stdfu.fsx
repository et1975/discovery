/// Convert an image into STM DFU-friendly format.
/// Works on windows only, as it imports the functionality from the windows-targeted unmanaged DLLs
module StDfu =
    open System
    open System.Runtime.InteropServices

    [<DllImport("STDFUFiles.dll", EntryPoint = "STDFUFILES_ImageFromFile", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)>]
    extern uint32 ImageFromFile([<MarshalAs(UnmanagedType.LPStr)>] string pathName, IntPtr& pImage, byte nAlternate)

    [<DllImport("STDFUFiles.dll", EntryPoint = "STDFUFILES_SetImageName", CallingConvention = CallingConvention.StdCall)>]
    extern uint32 SetImageName(IntPtr PImage, [<MarshalAs(UnmanagedType.LPStr)>] string pathName)

    [<DllImport("STDFUFiles.dll", EntryPoint = "STDFUFILES_CreateNewDFUFile", CallingConvention = CallingConvention.StdCall)>]
    extern uint32 CreateNewDFUFile([<MarshalAs(UnmanagedType.LPStr)>] string pathName, IntPtr& handle, uint32 vid, uint32 pid, uint32 bcd)

    [<DllImport("STDFUFiles.dll", EntryPoint = "STDFUFILES_AppendImageToDFUFile", CallingConvention = CallingConvention.StdCall)>]
    extern uint32 AppendImageToDFUFile(IntPtr handle, IntPtr image)

    [<DllImport("STDFUFiles.dll", EntryPoint = "STDFUFILES_CloseDFUFile", CallingConvention = CallingConvention.StdCall)>]
    extern uint32 CloseDFUFile(IntPtr handle)

    [<DllImport("STDFUFiles.dll", EntryPoint = "STDFUFILES_DestroyImage", CallingConvention = CallingConvention.StdCall)>]
    extern uint32 DestroyImage(IntPtr& image)


    module DFUResult =
        [<Literal>]
        let STDFU_NOERROR = 0x12340000u

    [<EntryPoint>]
    let main [|src; dst; bcd|] =
        let pid = 0xdf11u
        let vid = 0x0483u
        let bcd = Convert.ToUInt32(bcd,16)
        let alter = 0uy
        let mutable ih = IntPtr.Zero
        let mutable fh = IntPtr.Zero
        let r = ImageFromFile(src, &ih, alter)
        let r = match r with 
                | DFUResult.STDFU_NOERROR -> SetImageName(ih,"Firmware")
                | _ -> failwithf "Error: %A\n" r
        let r = CreateNewDFUFile(dst,&fh,vid,pid,bcd)
        let r = AppendImageToDFUFile(fh,ih)
        let r = CloseDFUFile(fh)
        let r = DestroyImage(&ih)
        0