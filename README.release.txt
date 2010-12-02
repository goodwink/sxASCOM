To do a release, be sure to:

- bump the version number
- change the expiration date (if enabled)
- edit SXCamera.Readme
- commit changes
- create a change log with:
git log --pretty=format:"%s (%ai by %aN <%aE>)" v1.2.1...  > changelog
- build
- create a tag
- Copy the files to 
  C:\Users\bretm\Documents\My Web Sites\daddog.com\ascom\sx\driver
- rename the files
