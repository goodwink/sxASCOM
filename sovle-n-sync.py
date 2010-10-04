import sys
import win32com.client
from HTMLParser import HTMLParser
import urllib2
import urllib, cookielib, mimetools, mimetypes

#####

cj = cookielib.CookieJar() 
opener = urllib2.build_opener(urllib2.HTTPCookieProcessor(cj)) 
urllib2.install_opener(opener) 
 
def post_multipart(host, selector, fields, files): 
    """ 
    Post fields and files to an http host as multipart/form-data. 
    fields is a sequence of (name, value) elements for regular form fields. 
    files is a sequence of (name, filename, value) elements for data to be uploaded as files 
    Return the server's response page. 
    """ 
    content_type, body = encode_multipart_formdata(fields, files) 
    headers = {'Content-Type': content_type, 
               'Content-Length': str(len(body)),
               'Cookie':'uname=Bret McKee; email=bretm@boneheads.us'} 
    r = urllib2.Request("http://%s%s" % (host, selector), body, headers) 
    return urllib2.urlopen(r).read() 
 
def encode_multipart_formdata(fields, files): 
    """ 
    fields is a sequence of (name, value) elements for regular form fields. 
    files is a sequence of (name, filename, value) elements for data to be uploaded as files 
    Return (content_type, body) ready for httplib.HTTP instance 
    """ 
    BOUNDARY = mimetools.choose_boundary() 
    CRLF = '\r\n' 
    L = [] 
    for (key, value) in fields: 
        L.append('--' + BOUNDARY) 
        L.append('Content-Disposition: form-data; name="%s"' % key) 
        L.append('') 
        L.append(value) 
    for (key, filename, value) in files: 
        L.append('--' + BOUNDARY) 
        L.append('Content-Disposition: form-data; name="%s"; filename="%s"' % (key, filename)) 
        L.append('Content-Type: %s' % get_content_type(filename)) 
        L.append('') 
        L.append(value) 
    L.append('--' + BOUNDARY + '--') 
    L.append('') 
    body = CRLF.join(L) 
    content_type = 'multipart/form-data; boundary=%s' % BOUNDARY 
    return content_type, body 
 
def get_content_type(filename): 
    return mimetypes.guess_type(filename)[0] or 'application/octet-stream' 

#####

class ParseResponse(HTMLParser):
    def __init__(self):
        HTMLParser.__init__(self)
        self.next = False

#    def handle_starttag(self, tag, attrs):
#        print "Encountered the beginning of a %s tag" % tag
#        print "attrs=", attrs

#    def handle_endtag(self, tag):
#        print "Encountered the end of a %s tag" % tag

    def handle_data(self, data):
        if data.find("(RA, Dec) center:") >=0:
            self.next = True
        elif self.next:
            self.next = False
            open = data.rfind("(")
            close = data.rfind(")")
            coorids = data[open+1:close-1]
            fields = coorids.split(",")
            self.ra = float(fields[0])/15.0
            self.dec = float(fields[1])

class ParseFirstScreen(HTMLParser):
    def handle_starttag(self, tag, attrs):
        #print "Encountered the beginning of a %s tag" % tag
        if len(attrs) > 1:
            (name, value) = attrs[0]
            if name == "name" and value == "UPLOAD_IDENTIFIER":
                for (name, value) in attrs[1:]:
                    if name == "value":
                        self.id = value

def getID(url):
    parser = ParseFirstScreen()
    req = urllib2.urlopen(url)
    for line in req:
        parser.feed(line)
    try:
        return parser.id
    except AttributeError:
        raise Exception, "Unable to get an upload identifier"

def readReport(url):
    parser = ParseResponse()
    req = urlopen(url)
    for line in req:
        parser.feed(line)
    return (parser.ra, parser.dec)

def requestReport(host, selector, id, image_file):
#http://live.astrometry.net/index.php?imgfile=frame-103p-Hartley_Light_256.00Sec__18.fits&xysrc=img&fsl=3&fsu=4&fsunit=arcsecperpix&parity=0
#http://live.astrometry.net/index.php?imgfile=m42-plate_Light_32.00Sec__29.fits&xysrc=img&fsl=3&fsu=4&fsunit=arcsecperpix&skippreview=1
    fields = [
                ("UPLOAD_IDENTIFIER", id), 
                ("MAX_FILE_SIZE", "262144000"),
                ("justjobid", ""),
                ("skippreview", ""),
                ("email", "bretm@boneheads.us"), 
                ("uname", "Bret McKee"),
                ("remember", "1"),
                ("imgurl", "http://"),
                ("x_col", "x"),
                ("y_col", "y"),
                ("xysrc", "img"), 
                ("fsunit", "arcsecperpix"), 
                ("fsl", "3"), 
                ("fsu", "4"), 
                ("parity", "0"), 
                ("index", "auto"),
                ("fse", ""),
                ("fsv", ""),
                ("poserr", "1"),
                ("tweak_order", "2"),
                ("submit", "Submit")
                
            ]

    files = [
                ("imgfile", image_file, open(image_file, "rb").read()),
                ("textfile", "", ""),
                ("fitsfile", "", ""),
            ]
    
    data = urllib.urlencode({"xyzsrc":"img", "imgfile":image_file, "fsunit":"arcsecperpix", "fls":"3", "fsu":"4", "parity":"0", "index":"auto"})
    post_multipart(host, selector, fields, files)

def syncScope(scope, ra, dec):
    telescope = win32com.client.Dispatch(scope)
    telescope.Connected = True
    telescope.Tracking = True
    if telescope.Tracking:
        telescope.SyncToCoordinates(ra, dec)
    else:
        print >>sys.stderr, "Telescope is not tracking - sync not possible"
    #print telescope.CanSetDeclinationRate
    #print telescope.CanSetRightAscensionRate
    #print telescope.CanSync

def main():
    url = "http://live.astrometry.net/index.php"
    host = "live.astrometry.net"
    selector = "/index.php"
    id = getID(url)
    #id = "uniqueID"
    requestReport(host, selector, id, "m45-plate_Light_32.00Sec__28.fits")
    sys.exit(1)
    url = "http://live.astrometry.net/status.php?job=alpha-201010-41919321"
    (ra, dec) = readReport(url)
    print ra, dec
    syncScope("POTH.Telescope", ra, dec)

def dummy():
#chooser = win32com.client.Dispatch("DriverHelper.Chooser")
#print chooser
#chooser.DeviceType = "Telescope";
#choice = chooser.Choose("")
    choice = "POTH.Telescope"

if __name__ == "__main__":
    main()
