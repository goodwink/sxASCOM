To do a release, be sure to:

- bump the version number
  - SharedVersionNumber.cs
  - SXAscomInstaller.iss
- change the expiration date (if enabled)
- create a change log with:
git log --pretty=format:"%s (%ai by %aN <%aE>)" v1.2.1...  > changelog.txt
- edit SXCamera.Readme
- commit changes
- build
- create a tag
- Copy the files to 
  C:\Users\bretm\Documents\My Web Sites\daddog.com\ascom\sx\driver
- rename the files
