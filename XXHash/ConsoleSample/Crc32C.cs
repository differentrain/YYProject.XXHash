using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ConsoleSample
{
    public sealed class Crc32C : HashAlgorithm
    {
        private static readonly uint[] _Crc32CTable = new uint[] {
                         0x00000000U, 0xF26B8303U, 0xE13B70F7U, 0x1350F3F4U,
                         0xC79A971FU, 0x35F1141CU, 0x26A1E7E8U, 0xD4CA64EBU,
                         0x8AD958CFU, 0x78B2DBCCU, 0x6BE22838U, 0x9989AB3BU,
                         0x4D43CFD0U, 0xBF284CD3U, 0xAC78BF27U, 0x5E133C24U,
                         0x105EC76FU, 0xE235446CU, 0xF165B798U, 0x030E349BU,
                         0xD7C45070U, 0x25AFD373U, 0x36FF2087U, 0xC494A384U,
                         0x9A879FA0U, 0x68EC1CA3U, 0x7BBCEF57U, 0x89D76C54U,
                         0x5D1D08BFU, 0xAF768BBCU, 0xBC267848U, 0x4E4DFB4BU,
                         0x20BD8EDEU, 0xD2D60DDDU, 0xC186FE29U, 0x33ED7D2AU,
                         0xE72719C1U, 0x154C9AC2U, 0x061C6936U, 0xF477EA35U,
                         0xAA64D611U, 0x580F5512U, 0x4B5FA6E6U, 0xB93425E5U,
                         0x6DFE410EU, 0x9F95C20DU, 0x8CC531F9U, 0x7EAEB2FAU,
                         0x30E349B1U, 0xC288CAB2U, 0xD1D83946U, 0x23B3BA45U,
                         0xF779DEAEU, 0x05125DADU, 0x1642AE59U, 0xE4292D5AU,
                         0xBA3A117EU, 0x4851927DU, 0x5B016189U, 0xA96AE28AU,
                         0x7DA08661U, 0x8FCB0562U, 0x9C9BF696U, 0x6EF07595U,
                         0x417B1DBCU, 0xB3109EBFU, 0xA0406D4BU, 0x522BEE48U,
                         0x86E18AA3U, 0x748A09A0U, 0x67DAFA54U, 0x95B17957U,
                         0xCBA24573U, 0x39C9C670U, 0x2A993584U, 0xD8F2B687U,
                         0x0C38D26CU, 0xFE53516FU, 0xED03A29BU, 0x1F682198U,
                         0x5125DAD3U, 0xA34E59D0U, 0xB01EAA24U, 0x42752927U,
                         0x96BF4DCCU, 0x64D4CECFU, 0x77843D3BU, 0x85EFBE38U,
                         0xDBFC821CU, 0x2997011FU, 0x3AC7F2EBU, 0xC8AC71E8U,
                         0x1C661503U, 0xEE0D9600U, 0xFD5D65F4U, 0x0F36E6F7U,
                         0x61C69362U, 0x93AD1061U, 0x80FDE395U, 0x72966096U,
                         0xA65C047DU, 0x5437877EU, 0x4767748AU, 0xB50CF789U,
                         0xEB1FCBADU, 0x197448AEU, 0x0A24BB5AU, 0xF84F3859U,
                         0x2C855CB2U, 0xDEEEDFB1U, 0xCDBE2C45U, 0x3FD5AF46U,
                         0x7198540DU, 0x83F3D70EU, 0x90A324FAU, 0x62C8A7F9U,
                         0xB602C312U, 0x44694011U, 0x5739B3E5U, 0xA55230E6U,
                         0xFB410CC2U, 0x092A8FC1U, 0x1A7A7C35U, 0xE811FF36U,
                         0x3CDB9BDDU, 0xCEB018DEU, 0xDDE0EB2AU, 0x2F8B6829U,
                         0x82F63B78U, 0x709DB87BU, 0x63CD4B8FU, 0x91A6C88CU,
                         0x456CAC67U, 0xB7072F64U, 0xA457DC90U, 0x563C5F93U,
                         0x082F63B7U, 0xFA44E0B4U, 0xE9141340U, 0x1B7F9043U,
                         0xCFB5F4A8U, 0x3DDE77ABU, 0x2E8E845FU, 0xDCE5075CU,
                         0x92A8FC17U, 0x60C37F14U, 0x73938CE0U, 0x81F80FE3U,
                         0x55326B08U, 0xA759E80BU, 0xB4091BFFU, 0x466298FCU,
                         0x1871A4D8U, 0xEA1A27DBU, 0xF94AD42FU, 0x0B21572CU,
                         0xDFEB33C7U, 0x2D80B0C4U, 0x3ED04330U, 0xCCBBC033U,
                         0xA24BB5A6U, 0x502036A5U, 0x4370C551U, 0xB11B4652U,
                         0x65D122B9U, 0x97BAA1BAU, 0x84EA524EU, 0x7681D14DU,
                         0x2892ED69U, 0xDAF96E6AU, 0xC9A99D9EU, 0x3BC21E9DU,
                         0xEF087A76U, 0x1D63F975U, 0x0E330A81U, 0xFC588982U,
                         0xB21572C9U, 0x407EF1CAU, 0x532E023EU, 0xA145813DU,
                         0x758FE5D6U, 0x87E466D5U, 0x94B49521U, 0x66DF1622U,
                         0x38CC2A06U, 0xCAA7A905U, 0xD9F75AF1U, 0x2B9CD9F2U,
                         0xFF56BD19U, 0x0D3D3E1AU, 0x1E6DCDEEU, 0xEC064EEDU,
                         0xC38D26C4U, 0x31E6A5C7U, 0x22B65633U, 0xD0DDD530U,
                         0x0417B1DBU, 0xF67C32D8U, 0xE52CC12CU, 0x1747422FU,
                         0x49547E0BU, 0xBB3FFD08U, 0xA86F0EFCU, 0x5A048DFFU,
                         0x8ECEE914U, 0x7CA56A17U, 0x6FF599E3U, 0x9D9E1AE0U,
                         0xD3D3E1ABU, 0x21B862A8U, 0x32E8915CU, 0xC083125FU,
                         0x144976B4U, 0xE622F5B7U, 0xF5720643U, 0x07198540U,
                         0x590AB964U, 0xAB613A67U, 0xB831C993U, 0x4A5A4A90U,
                         0x9E902E7BU, 0x6CFBAD78U, 0x7FAB5E8CU, 0x8DC0DD8FU,
                         0xE330A81AU, 0x115B2B19U, 0x020BD8EDU, 0xF0605BEEU,
                         0x24AA3F05U, 0xD6C1BC06U, 0xC5914FF2U, 0x37FACCF1U,
                         0x69E9F0D5U, 0x9B8273D6U, 0x88D28022U, 0x7AB90321U,
                         0xAE7367CAU, 0x5C18E4C9U, 0x4F48173DU, 0xBD23943EU,
                         0xF36E6F75U, 0x0105EC76U, 0x12551F82U, 0xE03E9C81U,
                         0x34F4F86AU, 0xC69F7B69U, 0xD5CF889DU, 0x27A40B9EU,
                         0x79B737BAU, 0x8BDCB4B9U, 0x988C474DU, 0x6AE7C44EU,
                         0xBE2DA0A5U, 0x4C4623A6U, 0x5F16D052U, 0xAD7D5351U
};

        public Crc32C() => Initialize();

        private uint _Hash;

        public override void Initialize()
        {
            _Hash = uint.MaxValue;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            unsafe
            {
                fixed (uint* table = _Crc32CTable)
                {
                    fixed (byte* buf = array)
                    {
                        var curPtr = buf;
                        while (cbSize-- > 0)
                        {
                            _Hash = (_Hash >> 8) ^ table[(_Hash ^ *curPtr++) & 0xFF];
                        }
                    }
                }
            }
        }

        protected override byte[] HashFinal()
        {
            _Hash = ~_Hash;
            return BitConverter.GetBytes(_Hash);
        }

    }

    public sealed class Crc32CSSE42 : HashAlgorithm
    {
        /*MIT License

        Copyright (c) 2018 differentrain

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE. */


        /*
        X64 
        uint AsmFunc(byte* array, int startIndex, int length, uint hash_init)
           mov eax,r9d
          //r8=length,rdx=startIndex
          sub r8d,edx //r8=size
          mov r9d,r8d //r9=r8=size
          add rcx,rdx //rcx=array,  array ptr added.
        part_Switch:
          xor rdx,rdx //rdx= loop index
          test r9d,r9d
          jz part_Crc32_Finished
          cmp r9d,8
          jb loop_Crc32_8
        part_Crc32_64:
          and r9d,7  //r9=residue
          shr r8d,3  //R8=max loop count
        loop_Crc32_64:
          crc32 rax,qword ptr [rcx+rdx*8]
          inc rdx
          cmp rdx,r8
          jb loop_Crc32_64
          lea rcx,[rcx+rdx*8] //rcx=array,  array ptr added.
          jmp part_Switch
        loop_Crc32_8:
          crc32 rax, byte ptr [rcx+rdx]
          inc rdx
          cmp rdx,r9d
          jb loop_Crc32_8
        part_Crc32_Finished:
          ret
        */
        private static readonly byte[] _AsmCodeCrc32C_64 = new byte[] { 0x41, 0x8B, 0xC1, 0x41, 0x29, 0xD0, 0x45, 0x8B, 0xC8, 0x48, 0x01, 0xD1, 0x48, 0x31, 0xD2, 0x45, 0x85, 0xC9, 0x74, 0x32, 0x41, 0x83, 0xF9, 0x08, 0x72, 0x1D, 0x41, 0x83, 0xE1, 0x07, 0x41, 0xC1, 0xE8, 0x03, 0xF2, 0x48, 0x0F, 0x38, 0xF1, 0x04, 0xD1, 0x48, 0xFF, 0xC2, 0x4C, 0x39, 0xC2, 0x72, 0xF1, 0x48, 0x8D, 0x0C, 0xD1, 0xEB, 0xD5, 0xF2, 0x48, 0x0F, 0x38, 0xF0, 0x04, 0x11, 0x48, 0xFF, 0xC2, 0x4C, 0x39, 0xCA, 0x72, 0xF1, 0xC3 };
        /*
        X86 
        uint AsmFunc(byte* array, int startIndex, int length, uint hash_init)
          mov edx,[esp+c] //eax=length
          sub edx,[esp+8] //[esp+8]=startIndex, edx=size
          mov ecx,[esp+4] //ecx=array
          add ecx,[esp+8] //ecx=array, array ptr added.
          mov eax,[esp+10] //[esp+10]=hash_init
          push esi
          push edi
          mov edi,edx //edx=edi=size
        part_Switch:
          xor esi,esi //rdx= loop index
          test edx,edx
          jz part_Crc32_Finished
          cmp edx,4
          jb loop_Crc32_8
        part_Crc32_32:
          and edx,3  //edx=residue
          shr edi,2  //edi=max loop count
        loop_Crc32_32:
          crc32 eax,dword ptr[ecx+esi*4]
          inc esi
          cmp esi,edi
          jb loop_Crc32_32
          lea ecx,[ecx+esi*4] //rcx=array,  array ptr added.
          jmp part_Switch
        loop_Crc32_8:
          crc32 eax, byte ptr[ecx+esi]
          inc esi
          cmp esi,edx
          jb loop_Crc32_8
        part_Crc32_Finished:
          pop edi
          pop esi
          ret 10
        */
        private static readonly byte[] _AsmCodeCrc32C_32 = new byte[] { 0x8B, 0x54, 0x24, 0x0C, 0x2B, 0x54, 0x24, 0x08, 0x8B, 0x4C, 0x24, 0x04, 0x03, 0x4C, 0x24, 0x08, 0x8B, 0x44, 0x24, 0x10, 0x56, 0x57, 0x8B, 0xFA, 0x31, 0xF6, 0x85, 0xD2, 0x74, 0x26, 0x83, 0xFA, 0x04, 0x72, 0x16, 0x83, 0xE2, 0x03, 0xC1, 0xEF, 0x02, 0xF2, 0x0F, 0x38, 0xF1, 0x04, 0xB1, 0x46, 0x39, 0xFE, 0x72, 0xF5, 0x8D, 0x0C, 0xB1, 0xEB, 0xDF, 0xF2, 0x0F, 0x38, 0xF0, 0x04, 0x31, 0x46, 0x39, 0xD6, 0x72, 0xF5, 0x5F, 0x5E, 0xC2, 0x10, 0x00 };


        private delegate uint GetCRC32CDel(byte[] array, int start, int length, uint hash_Init);

        private static readonly GetCRC32CDel AsmCrc32;

        private uint _Hash;

        private static readonly IntPtr FuncAddr;
        static unsafe Crc32CSSE42()
        {
            var asmCrc = IntPtr.Size == 4 ? _AsmCodeCrc32C_32 : _AsmCodeCrc32C_64;
            FuncAddr = Marshal.AllocCoTaskMem(asmCrc.Length);
            Marshal.Copy(asmCrc, 0, FuncAddr, asmCrc.Length);
            var q = VirtualProtectEx(GetCurrentProcess(), FuncAddr.ToPointer(), (UIntPtr)asmCrc.Length, PAGE_EXECUTE_READWRITE, out var lastProtect);
            AsmCrc32 = (GetCRC32CDel)Marshal.GetDelegateForFunctionPointer(FuncAddr, typeof(GetCRC32CDel));
        }

        [DllImport("kernel32.dll", SetLastError = false, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = false, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private unsafe static extern bool VirtualProtectEx([In] IntPtr hProcess, [In] void* lpAddress, [In, MarshalAs(UnmanagedType.SysUInt)] UIntPtr dwSize, [In] uint flNewProtect, [Out] out uint lpflOldProtect);

        private const uint PAGE_EXECUTE_READWRITE = 0x40;

        public Crc32CSSE42() => Initialize();

        public override void Initialize() => _Hash = uint.MaxValue;

        protected override void HashCore(byte[] array, int ibStart, int cbSize) => _Hash = AsmCrc32(array, ibStart, cbSize, _Hash);

        protected override byte[] HashFinal()
        {
            _Hash = ~_Hash;
            return BitConverter.GetBytes(_Hash);
        }

    }

}
