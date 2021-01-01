SET PATH_SAJAT=D:\w0rk\GitHub\unity-novr1
SET RAMDISK_PATH=R:\unity-novr1

mkdir %RAMDISK_PATH%

mklink /D %RAMDISK_PATH%\Assets           %PATH_SAJAT%\Assets
mklink /D %RAMDISK_PATH%\Packages         %PATH_SAJAT%\Packages
mklink /D %RAMDISK_PATH%\ProjectSettings  %PATH_SAJAT%\ProjectSettings
