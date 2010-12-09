from __future__ import print_function, with_statement

import os, sys, getopt

def printList(label, nameList):
    if len(nameList) > 0:
        first = True
        print(label, end="")
        for fileName in nameList:
            if not first:
                print(",", end="")
            print("{0}".format(fileName), end="")
            first = False
    print("")

def process(startDir):
    dos = []
    unix = []
    mixed = []
    for root, dirs, files in os.walk(startDir):
        for fileName in files:
            if fileName.lower().endswith(".cs"):
                fullPath = os.path.join(root, fileName)
                with open(fullPath, "rb") as f:
                    dosEnding = None
                    mixedEnding = False
                    for line in f:
                        #print ("dos={0} mixed={1} ".format(dosEnding, mixedEnding), end="")
                        #if len(line) > 1:
                        #    print("{0} {1} {2} {3}".format(len(line), ord(line[-2]), ord(line[-1]), line.strip()))
                        #else:
                        #    print("{0} {1}".format(len(line), line))
                        if dosEnding == None:
                            if len(line) > 1 and ord(line[-2]) == 13:
                                dosEnding = True
                            else:
                                dosEnding = False
                        else:
                            if dosEnding and (len(line) < 2 or ord(line[-2]) != 13):
                                mixedEnding = True
                                break
                    if mixedEnding:
                        mixed.append(fullPath)
                    elif dosEnding:
                        dos.append(fullPath)
                    else:
                        unix.append(fullPath)

    printList("dos:", dos)
    printList("unix:", unix)
    printList("mixed:", mixed)

def usage():
    print("usage: {0}: [options] <diretory> [<directory>...]".format(sys.argv[0]), file=sys.stderr);

def main():
    try:
        opts, args = getopt.getopt(sys.argv[1:], "ho:v", ["help", "output="])
    except getopt.GetoptError, err:
        # print help information and exit:
        print(str(err), file=sys.stderr) # will print something like "option -a not recognized"
        usage()
        sys.exit(2)
    output = None
    verbose = False
    for o, a in opts:
        if o == "-v":
            verbose = True
        elif o in ("-h", "--help"):
            usage()
            sys.exit()
        elif o in ("-o", "--output"):
            output = a
        else:
            assert False, "unhandled option"
    process("")

if __name__ == "__main__":
    main()

