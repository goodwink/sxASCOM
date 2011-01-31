To do a release, be sure to:

- bump the version number
  - Properties -> Assemblyinfo.cs
  - SXAscomInstaller.iss
- change the expiration date (if enabled)
- create a change log with:
  git log --pretty=format:"%s (%ai by %aN <%aE>)" v1.3.8...  > changelog.txt
  git diff -b --shortstat v1.3.8..HEAD >> changelog.txt
- edit SXCamera.Readme
- commit changes
  git commit -a -m "Changes required for V1.3.9"
- build
- create a tag
- create the installer
- Copy the files to 
  C:\Users\bretm\Documents\My Web Sites\daddog.com\ascom\sx\driver
- rename the files
