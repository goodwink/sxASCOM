from __future__ import print_function, with_statement

import sys, os, tempfile, re
import datetime, time
import subprocess
import errno
import StringIO

from contextlib import nested
from optparse import OptionParser

def runCmd(cmd):
    #print(cmd)
    try:
        cmdProcess = subprocess.Popen(cmd,shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
        (stdoutData, stderrData) = cmdProcess.communicate(None)
        return (cmdProcess.returncode, stdoutData, stderrData)
    except:
        raise

def runCmdOK(cmd):
    (returnCode, stdoutData, stderrData) = runCmd(cmd)
    if returnCode != 0:
        print("{0} returned non-zero status.".format(cmd))
        if len(stderrData):
            print("stderr output was " + stderrData)
        sys.exit(returnCode)
    return stdoutData

def runCmdBool(cmd):
    (returnCode, stdoutData, stderrData) = runCmd(cmd)
    return returnCode == 0

def updateVersion(dirName, fileName, major, minor, revision, build):
    print("processing", os.path.join(dirName, fileName))
    lines = []

    with open(os.path.join(dirName, fileName), "r+b") as file:
        for line in file:
            line = re.sub(r'AssemblyVersion([^)]*)', 'AssemblyVersion("{0}.{1}.{2}.{3}"'.format(major, minor, revision, build), line)
            line = re.sub(r'AssemblyFileVersion([^)]*)', 'AssemblyFileVersion("{0}.{1}.{2}.{3}"'.format(major, minor, revision, build), line)
            line = re.sub(r'APP_VERSION ".*"', 'APP_VERSION "{0}.{1}.{2}.{3}"'.format(major, minor, revision, build), line)
            lines.append(line)
        file.truncate(0)
        file.writelines(lines)


def updateVersions(major, minor, revision, build):
    for root, dirs, files in os.walk("."):
        if ".git" in dirs:
            dirs.remove(".git")

        for name in files:
            if name.lower().endswith("assemblyinfo.cs"):
                updateVersion(root, name, major, minor, revision, build)

    updateVersion(".", "SXAscomInstaller.iss", major, minor, revision, build)
    print('commit with:\ngit commit -a -m "changes for version {0}.{1}.{2}.{3}"'.format(major, minor, revision, build))
    print('tag with:\ngit tag -a -m "tagging version {0}.{1}.{2}.{3}" v{0}.{1}.{2}.{3}'.format(major, minor, revision, build))
    print('zip with:\nc:\progra~1\winzip\wzzip SXAscomInstaller-v{0}.{1}.{2}.{3}.zip SXAscomInstaller-v{0}.{1}.{2}.{3}.exe'.format(major,minor,revision,build))



def main():
    if len(sys.argv) != 1 and len(sys.argv) != 4:
        print("usage: {0} [major minor revision]".format(sys.argv[0]), file=sys.stderr)
        sys.exit(1)

    description = runCmdOK("git describe")

    if len(description.split("-")) == 1:
        version = description
    else:
        (version, commits, hex) = description.split("-")

    now = datetime.datetime.now()
    timestamp = now.strftime("%y%j")

    if len(sys.argv) == 5:
        major = int(sys.argv[1])
        minor = int(sys.argv[2])
        revision = int(sys.argv[3])
        timestamp = int(sys.argv[4])
    elif len(sys.argv) == 4:
        major = int(sys.argv[1])
        minor = int(sys.argv[2])
        revision = int(sys.argv[3])
    else:
        (major, minor, revision, rest) = version[1:].split(".",3)

    updateVersions(major, minor, revision, timestamp)

if __name__ == "__main__":
    main()

