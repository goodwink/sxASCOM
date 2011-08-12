To do a release, be sure to:

- bump the version number 
  - Properties -> Assemblyinfo.cs
  - SXAscomInstaller.iss
    also set the type (Release vs debug)
- change the expiration date (if enabled) in sxGeneric\Driver.cs
- edit SXCamera.Readme
- commit changes
  git commit -a -m "Changes required for V1.3.10"
- create a change log with:
  git log --pretty=format:"%s (%ai by %aN <%aE>)" v2.1.1.2...  > changelog.txt
  bash -c "echo" >> changelog.txt
  bash -c "echo" >> changelog.txt
  git diff -b --shortstat v2.1.1.2..HEAD >> changelog.txt
  git log --pretty=format:"%s (%ai by %aN <%aE>)" > changelog.full.txt
- build
- create a tag
  git tag -a -m "Version 2.1.1.2" v2.1.1.2
- git push
- git push --tags
- create the installer
- Copy the files to 
  start explorer "C:\Users\bretm\Documents\My Web Sites\daddog.com\ascom\sx\driver"
- rename the files
