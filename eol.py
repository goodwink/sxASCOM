from __future__ import print_function, with_statement

import os, sys, getopt

def process(startDir):
    for root, dirs, files in os.walk(startDir):
        for fileName in files:
            if fileName.lower().endswith(".cs"):
                mixed = False
                fullPath = os.path.join(root, fileName)
                with open(fullPath, "rb") as f:
                    eol = None
                    for line in f:
                        if eol == None:
                            eol = ord(line[-1])
                        elif not mixed:
                            if eol == 10:
                                mixed = (eol != ord(line[-1]))
                            else:
                                mixed = (ord(line[-1]) == 1)
                if mixed:
                    print("{0}".format(fullPath))

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

