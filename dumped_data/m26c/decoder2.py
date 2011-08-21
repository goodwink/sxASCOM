
def addrToCoorids(addr):
    x = int(addr/linebytes)
    y = (addr - x * linebytes)/2
    return (x,y)

def printPixel(dest, src, color):
    destX, destY = addrToCoorids(dest)
    print "({0:4}, {1:4}) [{2:8}]= {3} {4}".format(destX, destY, dest, src, color)

imptr1 = 0
imptr2 = 100000000
imptr3 = 200000000
imptr4 = 0

linelength=2616
linecount = 3900

# ----------------

linebytes = linelength * 2

lbx3 = linebytes * 3
lbx5 = linebytes * 5

linecount_4 = linecount/4

linelengthx2 = linelength*2
linelengthx4 = linelength*4
linelengthx8 = linelength*8

imptr4 = imptr4 + 10464

for frame in xrange(0,2):
    if frame == 0:
        impt5 = imptr2
        color = "r"
    else:
        impt5 = imptr3
        color = "b"

    for y in xrange(0,3):
        impt1 = imptr4 + (2*y+1-frame) * linebytes
        impt2 = imptr4 + (linecount-2*y-1-frame) * linebytes
        impt3 = impt1 + 2*linebytes
        impt4 = impt2 - 2*linebytes

        for z in xrange(0,4,2):
            printPixel(impt3, impt5, 'g')
            impt5 += 2
            printPixel(impt4, impt5, color)
            impt5 += 2
            printPixel(impt1, impt5, 'g')
            impt5 += 2
            printPixel(impt2, impt5, color)
            print

            impt1 += 4
            impt2 += 4
            impt3 += 4
            impt4 += 4
