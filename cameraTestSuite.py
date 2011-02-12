import sys, time, getopt, random, unittest
import win32com.client

# import the ascom python bindings
from win32com.client import gencache
gencache.EnsureModule('{76618F90-032F-4424-A680-802467A55742}', 0, 1, 0)
from win32com.client import constants

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


class ASCOMCameraTest(unittest.TestCase):
    def setUp(self):
        camera.Connected = True

    def tearDown(self):
        camera.Connected = False

    def expose(self,
                binX=1, binY=1, 
                startX=0, startY=0, 
                numX = -1, numY = -1, 
                exposure = defaultExposure):
        try:

            camera.BinX = binX
            camera.BinY = binY

            camera.StartX = startX
            camera.StartY = startY

            if numX == -1:
                camera.NumX = camera.CameraXSize
            else:
                camera.NumX = numX

            if numY == -1:
                camera.NumY = camera.CameraYSize
            else:
                camera.NumY = numY

            camera.StartExposure(exposure, False)

            time.sleep(exposure)

            for retries in xrange(50): # wait up to 5 seconds after the exposure ends
                if camera.CameraState == constants.cameraIdle:
                    break
#            try:
#                err = camera.LastError
#            except win32com.client.pywintypes.com_error:
#                pass
#            else:
#                self.fail("got error {0} during exposure".format(err))
                time.sleep(0.1)

            self.assertTrue(camera.ImageReady, "image not ready")
        except:
            print "exception in expose({0}, {1}, {2}, {3}, {4}, {5}, {6})".format(binX, binY, startX, startY, numX, numY, exposure)
            raise

        return camera.ImageArray

class RegressionTestCases(ASCOMCameraTest):
    def testRegression1(self):
        image = self.expose(1, 1, 462, 128, 100, 101)

class RandomTests(ASCOMCameraTest):
    def runRandomTests(self):
        for i in xrange(1000):
            xBin = random.randint(1,camera.MaxBinX)
            if camera.CanAsymmetricBin:
                xBin = random.randint(1,camera.MaxBinY)
            else:
                yBin = xBin

            binnedXSize = camera.CameraXSize/xBin
            binnedYSize = camera.CameraYSize/yBin

            startX = random.randint(0, binnedXSize - 2)
            startY = random.randint(0, binnedYSize - 2)

            numX = random.randint(1, (binnedXSize - 1 -startX))
            numY = random.randint(1, (binnedYSize - 1 -startY))

            self.expose(xBin, yBin, startX, startY, numX, numY)
    def repeatableRandomTests(self):
        random.seed(0)
        self.runRandomTests()
    def randomTests(self):
        random.seed()
        self.runRandomTests()

class SubImageTestCases(ASCOMCameraTest):
    def testFullSubImage(self):
        pass
    def testOnePixelSubImage(self):
        pass

class BinnedTestCases(ASCOMCameraTest):
    def testBinnedModes(self):
        if camera.CanAsymmetricBin:
            pass
        else:
            self.assertEqual(camera.MaxBinX, camera.MaxBinY)
            for i in xrange(camera.MaxBinX):
                bin = i + 1
                image = self.expose (bin, bin, 0, 0, camera.CameraXSize/bin, camera.CameraYSize/bin)

class BinnedSubImageTestCases(ASCOMCameraTest):
    def testBinnedModes(self):
        if camera.CanAsymmetricBin:
            pass
        else:
            self.assertEqual(camera.MaxBinX, camera.MaxBinY)
            for i in xrange(camera.MaxBinX):
                bin = i + 1

                for start in xrange(bin):
                    for height in xrange(1, camera.CameraYSize/bin - start):
                        image = self.expose(bin, bin, 0,  start, bin, height)

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
    allTests.addTest(unittest.makeSuite(RegressionTestCases,'test'))
    allTests.addTest(RandomTests("repeatableRandomTests"))
    allTests.addTest(RandomTests("randomTests"))
    #allTests.addTest(unittest.makeSuite(SubImageTestCases,'test'))
    #allTests.addTest(unittest.makeSuite(BinnedTestCases,'test'))
    #allTests.addTest(unittest.makeSuite(BinnedSubImageTestCases,'test'))

    runner = unittest.TextTestRunner()
    runner.run(allTests)


if __name__ == "__main__":
    main()
