REM Use this file to ignore trash in the repository
REM 1)add ignored file name to .svnignore
REM 2)run svnignore.bat
REM 3) commit
svn propset svn:ignore -F .svnignore . --recursive