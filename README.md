# YYProject.XXHash
A pure C# library provides the implementations of [xxHash](https://cyan4973.github.io/xxHash/) algorithm.

The minimum requirements for YYProject.XXHash are .NET Framework 3.5, and C# 7.0,
it means that these code also apply to .NET Core 1.0 or later, Mono 4.6 or later and so on. If necessary, you can also rewrite these
library to .NET Framework 2.0 with just a little work.

Since all code (XXHash32 and XXHash64) are inside these [file](https://raw.githubusercontent.com/differentrain/YYProject.XXHash/master/XXHash/YYProject.XXHash/XXHash.cs)
independently, I don't recommend using compiled library in your project, instead, 
you can just copy the useful parts to your code, this is the benefit of MIT License. P:)

If you are using .NET4.5 (or higher) or sibling frameworks, you can add conditional compilation
symbol "HIGHER_VERSIONS" to optimize static-short-methods.

## Getting Started

It's very easy to use this library. Both [XXHash32 Class](https://github.com/differentrain/YYProject.XXHash/wiki/cb2be3a3-5621-b343-992c-8a2af7fbe6df)
and [XXHash64 Class](https://github.com/differentrain/YYProject.XXHash/wiki/1f2e7168-1f3f-c493-7e7a-6d566f315fd9)
in [YYProject.XXHash Namespace](https://github.com/differentrain/YYProject.XXHash/wiki/2e5d6292-64c7-8d52-f77f-7d3314e71172) are 
derived from [HashAlgorithm Class](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm).

So if you know how to use 
[MD5CryptoServiceProvider Class](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.md5cryptoserviceprovider), you
can master this library directly.

In addition, the only important thing you need to know is the output hash value of these implementations. 
By [convention](https://github.com/Cyan4973/xxHash/blob/dev/doc/xxhash_spec.md#step-7-output), if the hash value output as byte array,
it's layout should be big-endian format. So I added [XXHash32.HashUInt32 Property](https://github.com/differentrain/YYProject.XXHash/wiki/41a8b660-545c-4567-75e9-57cc8ed88cbf)
and [XXHash64.HashUInt64 Property](https://github.com/differentrain/YYProject.XXHash/wiki/3d8fa3a8-53ce-8e95-6789-ac0e15244a1b) to get the corresponding original-unsigned-integer-value.

Furthermore, the "seed" value can be initialized through the constructor, and set through "Seed" property before and after hash computing.
And because the impact of [HashAlgorithm](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm) base class,
The instance members in [XXHash32 Class](https://github.com/differentrain/YYProject.XXHash/wiki/cb2be3a3-5621-b343-992c-8a2af7fbe6df) and
[XXHash64 Class](https://github.com/differentrain/YYProject.XXHash/wiki/1f2e7168-1f3f-c493-7e7a-6d566f315fd9) are NOT thread safe.

For more, see this repo's [wiki](https://github.com/differentrain/YYProject.XXHash/wiki) and source code.

## Test
I tested my implementations with official commandline tool [xxhsum](https://github.com/Cyan4973/xxHash/releases/download/v0.6.2/xxhsum-windows-v0.6.2.zip), and my implementations can get the correct result.

So the only question is, Why and when we should use [xxHash](https://cyan4973.github.io/xxHash/) algorithm. I compared this library with
the other hash algorithms' pure C# implementation: 
[MD5CryptoServiceProvider](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.md5cryptoserviceprovider),
and two [implementations](https://raw.githubusercontent.com/differentrain/YYProject.XXHash/master/XXHash/ConsoleSample/Crc32C.cs)
of [crc32C](https://tools.ietf.org/html/rfc3385) I wrote for this test, one is implemented by software code, and the other one
is implemented with SSE4.2 instruction set. All this preparation has a reason: to know what advantages does xxHash have.

(This repo includes all test code that was mentioned in these section, but note that the hardware accelerated Crc32C implementation can ONLY be run in windows, and all Crc32C implementations are apply only to little endian.)

``` 
File hash test on x86 debug

Min size=1024, max size=104857601

Total size: 5464052094
Total count: 100
xxHash32 total cost: 45091.0087
xxHash64 total cost: 29088.0494
MD5 total cost: 16979.3283
CRC32C normal total cost: 29000.9803
CRC32C SSE42 total cost: 8678.2587

Average size: 54640520.94
xxHash32 average cost: 450.910087
xxHash64 average cost: 290.880494
MD5 average cost: 169.793283
CRC32C normal average cost: 290.009803
CRC32C SSE42 average cost: 86.782587

All Test Complete.
**********************************************************
Bytes hash test on x86 debug

Min size=1024, max size=104857601

Total size: 5309359803
Total count: 100
xxHash32 total cost: 29280.5077
xxHash64 total cost: 21343.6379
MD5 total cost: 9067.8353
CRC32C normal total cost: 20834.9335
CRC32C SSE42 total cost: 1127.8227

Average size: 53093598.03
xxHash32 average cost: 292.805077
xxHash64 average cost: 213.436379
MD5 average cost: 90.678353
CRC32C normal average cost: 208.349335
CRC32C SSE42 average cost: 11.278227

All Test Complete.
**********************************************************
File hash test on x64 debug

Min size=1024, max size=104857601

Total size: 5248425279
Total count: 100
xxHash32 total cost: 31659.1375
xxHash64 total cost: 15637.0539
MD5 total cost: 16887.3522
CRC32C normal total cost: 53677.5133
CRC32C SSE42 total cost: 7815.3350

Average size: 52484252.79
xxHash32 average cost: 316.591375
xxHash64 average cost: 156.370539
MD5 average cost: 168.873522
CRC32C normal average cost: 536.775133
CRC32C SSE42 average cost: 78.15335

All Test Complete.
**********************************************************
Bytes hash test on x64 debug

Min size=1024, max size=104857601

Total size: 4908722148
Total count: 100
xxHash32 total cost: 16263.4691
xxHash64 total cost: 7992.6281
MD5 total cost: 8863.2781
CRC32C normal total cost: 43616.9906
CRC32C SSE42 total cost: 605.6425

Average size: 49087221.48
xxHash32 average cost: 162.634691
xxHash64 average cost: 79.926281
MD5 average cost: 88.632781
CRC32C normal average cost: 436.169906
CRC32C SSE42 average cost: 6.056425

All Test Complete.
**********************************************************
File hash test on x86 release

Min size=1024, max size=104857601

Total size: 5321375759
Total count: 100
xxHash32 total cost: 20084.2577
xxHash64 total cost: 13998.6729
MD5 total cost: 16943.4082
CRC32C normal total cost: 27478.3475
CRC32C SSE42 total cost: 8780.3455

Average size: 53213757.59
xxHash32 average cost: 200.842577
xxHash64 average cost: 139.986729
MD5 average cost: 169.434082
CRC32C normal average cost: 274.783475
CRC32C SSE42 average cost: 87.803455

All Test Complete.
**********************************************************
Bytes hash test on x86 release

Min size=1024, max size=104857601

Total size: 5687433138
Total count: 100
xxHash32 total cost: 5646.0605
xxHash64 total cost: 6853.2696
MD5 total cost: 9775.4242
CRC32C normal total cost: 21378.2090
CRC32C SSE42 total cost: 1215.4363

Average size: 56874331.38
xxHash32 average cost: 56.460605
xxHash64 average cost: 68.532696
MD5 average cost: 97.754242
CRC32C normal average cost: 213.78209
CRC32C SSE42 average cost: 12.154363

All Test Complete.
**********************************************************
File hash test on x64 release

Min size=1024, max size=104857601

Total size: 5538377140
Total count: 100
xxHash32 total cost: 19667.6662
xxHash64 total cost: 9744.4208
MD5 total cost: 17763.1438
CRC32C normal total cost: 29067.5917
CRC32C SSE42 total cost: 8186.3137

Average size: 55383771.4
xxHash32 average cost: 196.676662
xxHash64 average cost: 97.444208
MD5 average cost: 177.631438
CRC32C normal average cost: 290.675917
CRC32C SSE42 average cost: 81.863137

All Test Complete.
**********************************************************
Bytes hash test on x64 release

Min size=1024, max size=104857601

Total size: 5552347329
Total count: 100
xxHash32 total cost: 4588.0376
xxHash64 total cost: 2205.6407
MD5 total cost: 10006.7685
CRC32C normal total cost: 21596.4126
CRC32C SSE42 total cost: 682.4378

Average size: 55523473.29
xxHash32 average cost: 45.880376
xxHash64 average cost: 22.056407
MD5 average cost: 100.067685
CRC32C normal average cost: 215.964126
CRC32C SSE42 average cost: 6.824378

All Test Complete.
```
From this test, we can easily found that ~~the hardware accelerated CRC32C is winner, cheers!~~ on production environments, 
xxHash algorithm has remarkable advantage in speed, particularly in calculating the whole buffer directly. xxHash should be a good choose
for date verification or other small-data-scenes.

Deserved to be mentioned, xxHash32 has no significant advantage in x86, it seems that CLR makes efforts behind the scenes, hence xxH32 is the best choice. And I have
tried to compute the ["stripes"](https://github.com/Cyan4973/xxHash/blob/dev/doc/xxhash_spec.md) in parallel, but the effect was not ideal, the overhead of which are related to thread is high in this scenes.
