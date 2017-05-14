#!/bin/bash
for f in *.tmx
do 
   cp -v "$f" "${f%.tmx}".xml
done