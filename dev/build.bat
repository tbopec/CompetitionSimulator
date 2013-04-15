call "C:\Program Files\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86
msbuild Solution.sln /V:M /t:Rebuild /p:"Platform=Mixed Platforms" /p:Configuration=Debug