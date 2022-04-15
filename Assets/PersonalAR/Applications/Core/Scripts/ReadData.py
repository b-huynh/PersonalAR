# Python program to read
# json file
 
 
import json
 

num = 0
filename = "\CollectedData\RecordStudy_Session_" + num + "_1648850746782"
 
# returns JSON object as
# a dictionary
f = open(filename)
data = json.load(f)
 
# Iterating through the json
# list
for i in data['frames']:
    print(i)
 
# Closing file
f.close()