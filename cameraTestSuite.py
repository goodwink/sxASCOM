import sys, time, getopt
import win32com.client
from HTMLParser import HTMLParser
import urllib2
import urllib, cookielib, mimetools, mimetypes
import re
import unittest

### Global variables
verbose = False
util = None
camera = None
defaultExposure = 0.100

### Classes

class ConnectionTestCases(unittest.TestCase):
    def setUp(self):
        if camera.Connected:
            camera.Connected = False
        self.assertFalse(camera.Connected)

    def tearDown(self):
        if camera.Connected:
            camera.Connected = False
        self.assertFalse(camera.Connected)

    def testConnetDisconnect(self):
        camera.Connected = True
        self.assertTrue(camera.Connected)
        camera.Connected = False
        self.assertFalse(camera.Connected)

    def testUnconnectedDisconnet(self):
        self.assertFalse(camera.Connected)
        camera.Connected = False
        self.assertFalse(camera.Connected)

    def testConnectedConnet(self):
        camera.Connected = True
        self.assertTrue(camera.Connected)

        # make sure we can't connect twice
        try:
            camera.Connected = True
        except win32com.client.pywintypes.com_error:
            pass

        self.assertTrue(camera.Connected)

        camera.Connected = False
        self.assertFalse(camera.Connected)

    def testDoubleDisconnect(self):
        camera.Connected = True
        self.assertTrue(camera.Connected)
        # Now disconnnect
        camera.Connected = False
        self.assertFalse(camera.Connected)

        # And make sure we can't disconnect twice
        try:
            camera.Connected = False
        except win32com.client.pywintypes.com_error:
            pass

        self.assertFalse(camera.Connected)

class SubImageTestCases(unittest.TestCase):
    def setUp(self):
        camera.Connected = True
    def tearDown(self):
        camera.Connected = False
    def testFullSubImage(self):
        pass
    def testOnePixelSubImage(self):
        pass

class BinnedTestCases(unittest.TestCase):
    def setUp(self):
        camera.Connected = True
    def tearDown(self):
        camera.Connected = False
    def testBinnedModes(self):
        if camera.CanAsymmetricBin:
            pass
        else:
            self.assertEqual(camera.MaxBinX, camera.MaxBinY)
            for i in xrange(camera.MaxBinX):
                bin = i + 1
                camera.BinX = bin
                camera.BinY = bin
                camera.NumX = camera.CameraXSize/bin
                camera.NumY = camera.CameraYSize/bin
                camera.StartExposure(defaultExposure, False)
                time.sleep(defaultExposure)
                for retries in xrange(50): # wait up to 5 seconds after the exposure ends
                    if camera.ImageReady:
                        break
                    try:
                        err = camera.LastError
                    except win32com.client.pywintypes.com_error:
                        pass
                    else:
                        self.fail("got error {0} binning {1}".format(err, bin))
                    time.sleep(0.1)
                self.assertTrue(camera.ImageReady, "image not ready for bin value {0}".format(bin))
                image = camera.ImageArray

class BinnedSubImageTestCases(unittest.TestCase):
    def setUp(self):
        camera.Connected = True
    def tearDown(self):
        camera.Connected = False
    def testBinnedModes(self):
        if camera.CanAsymmetricBin:
            pass
        else:
            self.assertEqual(camera.MaxBinX, camera.MaxBinY)
            for i in xrange(camera.MaxBinX):
                bin = i + 1

                for start in xrange(bin):
                    for height in xrange(1, camera.CameraYSize/bin - start):

                        camera.BinX = bin
                        camera.BinY = bin

                        camera.StartX = 0
                        camera.StartY = start

                        #camera.NumX = camera.CameraXSize/bin - (start + num)
                        #camera.NumY = camera.CameraYSize/bin - (start + num)

                        camera.NumX = bin
                        camera.NumY = height

                        camera.StartExposure(defaultExposure, False)
                        time.sleep(defaultExposure)
                        for retries in xrange(50): # wait up to 5 seconds after the exposure ends
                            if camera.ImageReady:
                                break
                            try:
                                err = camera.LastError
                            except win32com.client.pywintypes.com_error:
                                pass
                            else:
                                self.fail("got error {0} binning {1}, start={2}, height={3}".format(err, bin, start, height))
                            time.sleep(0.1)
                        self.assertTrue(camera.ImageReady, "image not ready for bin value {0}".format(bin))
                        image = camera.ImageArray

def usage():
    print >>sys.stderr, "usage: %s [<camera_name>]"
    print >>sys.stderr, "where options can be:"
    print >>sys.stderr, "\t-v\t\t-> verbose output"

def main():
    global camera

    cameraName = None

    try:
        opts, args = getopt.getopt(sys.argv[1:], "hv", ["help"])
    except getopt.GetoptError, err:
        # print help information and exit:
        print str(err) # will print something like "option -a not recognized"
        usage()
        sys.exit(1)
    output = None
    global verbose
    for o, a in opts:
        if o == "-v":
            verbose = True
        elif o in ("-h", "--help"):
            usage()
            sys.exit()
        else:
            assert False, "unhandled option"

    if len(args) > 1:
        usage()
        sys.exit(1)
    elif len(args) == 1:
        cameraName = args[0]
    else: # len(args) == 0
        utilChooser = win32com.client.Dispatch("ASCOM.Utilities.Chooser")
        utilChooser.DeviceType = "Camera"
        cameraName = utilChooser.Choose("")
        print "chose ", cameraName

    utilProfile = win32com.client.Dispatch("ASCOM.Utilities.Profile")
    utilProfile.DeviceType = "Camera"

    if not utilProfile.IsRegistered(cameraName):
        print "camera {0} is not registered".format(cameraName)
        sys.exit()

    camera = win32com.client.Dispatch(cameraName)

    allTests = unittest.TestSuite()

    allTests.addTest(unittest.makeSuite(ConnectionTestCases,'test'))
    allTests.addTest(unittest.makeSuite(SubImageTestCases,'test'))
    allTests.addTest(unittest.makeSuite(BinnedTestCases,'test'))
    allTests.addTest(unittest.makeSuite(BinnedSubImageTestCases,'test'))

    runner = unittest.TextTestRunner()
    runner.run(allTests)


if __name__ == "__main__":
    main()
