import cv2
import numpy
import os
import imghdr
import sys
AcceptableImages = ['jpeg','png','gif','bmp']

#Take in a image and look for images that are very similar or exactly the same as the original image
#Does this by naively just calculating the difference norm of the images and check if it is below a threshold
#Can navigate recursively through directories
#The images normalize the scale to 1080 width, because if the image has been resized, then we'd still want
#to see that one

ImageToFind = "I:\\Pictures\\S5\\20140614_161829.jpg"
LocationToLook = "I:\\Pictures\\S5\\cache\\"
Recurse = True

DifferenceValue = 0.05
NormalizedScale = 1080

if len(sys.argv)==3:
    ImageToFind = sys.argv[1]
    LocationToLook = sys.argv[2]
elif len(sys.argv)==4:
    if sys.argv[1] == '-r':
        Recurse = True
    ImageToFind = sys.argv[2]
    LocationToLook = sys.argv[3]

def GetScale(img, size):
    (h,w) = img.shape[:2]
    scaleW = size/w
    img = cv2.resize(img,(int(w*scaleW),int(h*scaleW)))
    return (img,int(w*scaleW),int(h*scaleW))

def FindImages(dir, recurse):
    topDirs = os.listdir(dir)
    for fd in topDirs:
        if os.path.isdir(dir+fd):
            if recurse:
                FindImages(dir+fd+"\\", recurse)
                continue
            else:
                continue
        imgPath = dir+fd;
        type = imghdr.what(imgPath)
        if type in AcceptableImages:
            compare = cv2.imread(imgPath,0)
            (compare,cW,cH) = GetScale(compare,NormalizedScale)
            if cW==oW and cH==oH:
                normVal = cv2.norm(OrigImage,compare)/(cW*cH)
                if normVal < DifferenceValue:
                    print("FOUND: " + imgPath)
                    print("\tDiff:" + str(normVal))

OrigImage = cv2.imread(ImageToFind,0) #GreyScale
(OrigImage,oW,oH) = GetScale(OrigImage,NormalizedScale)
cv2.imshow("test",OrigImage)
cv2.waitKey()
topDirs = os.listdir(LocationToLook)
FindImages(LocationToLook, Recurse)


