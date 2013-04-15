call "C:\Program Files\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86
msbuild Dependencies/Robocop/Libraries/RoboCop.Plus.Common/RoboCoP.Plus.Common.csproj /V:M
msbuild Dependencies/Robocop/Libraries/Switch/Switch.csproj /V:M
mkdir "Dependencies/Robocop/bin"

xcopy Dependencies\Robocop\Libraries\Switch\bin\Debug    Dependencies\Robocop\bin\  /Y
xcopy Dependencies\Robocop\Libraries\Robocop.Plus.Common\bin\Debug    Dependencies\Robocop\bin\  /Y