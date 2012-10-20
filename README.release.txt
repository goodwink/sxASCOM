To do a release, be sure to:

- merge changes into the develop branch
- change the expiration date (if enabled) in SXAscomInstaller.iss
- edit SXCamera.Readme
- bump the version number 
  - bump_version.py
- commit changes
  git commit -a -m "Changes required for V1.3.10"
- create a tag
  git tag -a -m "Version 2.1.1.2" v2.1.1.2
- git push
- git push --tags
- create a change log with:
  git log --pretty=format:"%s (%ai by %aN <%aE>)" --no-merges releases/2.2/master..HEAD  > changelog.txt
  echo >> changelog.txt
  git diff -b --shortstat origin/releases/2.2/master..HEAD >> changelog.txt
  git log --pretty=format:"%s (%ai by %aN <%aE>)" --no-merges > changelog.full.txt
- build
- create the installer
- create a zip file
  c:\progra~1\winzip\wzzip <release>.zip <release.exe>

