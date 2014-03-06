To do a release, be sure to:

- merge changes into the develop branch
- change the expiration date (if enabled) in SXAscomInstaller.iss
- edit SXCamera.Readme
- bump the version number 
- bump_version.py
  - commit changes using printed git commands
  - create a tag using the printed git commands
- create a change log with:
  git log --pretty=format:"%s (%ai by %aN <%aE>)" --no-merges releases/2.2/master..HEAD  > changelog.txt
  echo >> changelog.txt
  git diff -b --shortstat origin/releases/2.2/master..HEAD >> changelog.txt
  git log --pretty=format:"%s (%ai by %aN <%aE>)" --no-merges > changelog.full.txt
- git push
- git push --tags
- create the installer

- create a zip file
  c:\progra~1\winzip\wzzip <release>.zip <release.exe>

- Copy the files to 
  start explorer "C:\Users\bretm\Documents\MyWebSites\daddog.com\ascom\sx\driver"
- rename the files
