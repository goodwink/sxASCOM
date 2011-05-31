To do a release, be sure to:

- bump the version number
  - Properties -> Assemblyinfo.cs
  - SXAscomInstaller.iss
- change the expiration date (if enabled) in sxGeneric\Driver.cs
- create a change log with:
  git log --pretty=format:"%s (%ai by %aN <%aE>)" v1.3.9...  > changelog.txt
  git diff -b --shortstat v1.3.9..HEAD >> changelog.txt
  git log --pretty=format:"%s (%ai by %aN <%aE>)" > changelog.full.txt
- edit SXCamera.Readme
- commit changes
  git commit -a -m "Changes required for V1.3.10"
- build
- create a tag
- git push
- git push --tags
- create the installer
- Copy the files to 
  "C:\Users\bretm\Documents\My Web Sites\daddog.com\ascom\sx\driver"
- rename the files
